﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using SourceAFIS.General;

namespace SourceAFIS.Templates
{
    public sealed class IsoFormat : TemplateFormatBase<byte[]>
    {
        // References:
        // http://www.italdata-roma.com/PDF/Norme%20ISO-IEC%20Minutiae%20Data%20Format%2019794-2.pdf
        // https://biolab.csr.unibo.it/fvcongoing/UI/Form/Download.aspx (ISO section, sample ZIP, ISOTemplate.pdf)
        //
        // Format (all numbers are big-endian):
        // 4B magic "FMR\0"
        // 4B version (ignored, set to " 20\0"
        // 4B total length (including header)
        // 2B rubbish (zeroed)
        // 2B image size in pixels X
        // 2B image size in pixels Y
        // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
        // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
        // 1B rubbish (number of fingerprints, set to 1)
        // 1B rubbish (zeroed)
        // 1B rubbish (finger position, zeroed)
        // 1B rubbish (zeroed)
        // 1B rubbish (fingerprint quality, set to 100)
        // 1B minutia count
        // N*6B minutiae
        //      2B minutia position X in pixels
        //          2b (upper) minutia type (01 ending, 10 bifurcation, 00 other)
        //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
        //      1B direction, compatible with SourceAFIS angles
        //      1B quality (ignored, zeroed)
        // 2B rubbish (extra data length, zeroed)
        // N*1B rubbish (extra data)

        public override byte[] Export(TemplateBuilder builder)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8);

            checked
            {
                // 4B magic "FMR\0"
                writer.Write("FMR\0".ToCharArray());

                // 4B version (ignored, set to " 20\0"
                writer.Write(" 20\0".ToCharArray());

                // 4B total length (including header, will be updated later)
                writer.Write(0);

                // 2B rubbish (zeroed)
                writer.Write((short)0);

                // 2B image size in pixels X
                writer.Write(IPAddress.HostToNetworkOrder((short)builder.StandardDpiWidth));
            
                // 2B image size in pixels Y
                writer.Write(IPAddress.HostToNetworkOrder((short)builder.StandardDpiHeight));
            
                // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
                writer.Write(IPAddress.HostToNetworkOrder((short)196));

                // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
                writer.Write(IPAddress.HostToNetworkOrder((short)196));

                // 1B rubbish (number of fingerprints, set to 1)
                writer.Write((byte)1);

                // 1B rubbish (zeroed)
                writer.Write((byte)0);

                // 1B rubbish (finger position, zeroed)
                writer.Write((byte)0);

                // 1B rubbish (zeroed)
                writer.Write((byte)0);

                // 1B rubbish (fingerprint quality, set to 100)
                writer.Write((byte)100);

                // 1B minutia count
                writer.Write((byte)builder.Minutiae.Count);

                // N*6B minutiae
                foreach (var minutia in builder.Minutiae)
                {
                    //      2B minutia position X in pixels
                    //          2b (upper) minutia type (01 ending, 10 bifurcation, 00 other (considered ending))
                    int x = minutia.Position.X;
                    AssertException.Check(x <= 0x3fff, "X position is out of range");
                    int type;
                    switch (minutia.Type)
                    {
                        case TemplateBuilder.MinutiaType.Ending: type = 0x4000; break;
                        case TemplateBuilder.MinutiaType.Bifurcation: type = 0x8000; break;
                        case TemplateBuilder.MinutiaType.Other: type = 0; break;
                        default: throw new ApplicationException();
                    }
                    writer.Write(IPAddress.HostToNetworkOrder(unchecked((short)(x | type))));

                    //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
                    int y = builder.StandardDpiHeight - minutia.Position.Y - 1;
                    AssertException.Check(y <= 0x3fff, "Y position is out of range");
                    writer.Write(IPAddress.HostToNetworkOrder((short)y));

                    //      1B direction, compatible with SourceAFIS angles
                    writer.Write(minutia.Direction);

                    //      1B quality (ignored, zeroed)
                    writer.Write((byte)0);
                }

                // 2B rubbish (extra data length, zeroed)
                // N*1B rubbish (extra data)
                writer.Write((short)0);
            }

            writer.Close();

            // update length
            byte[] template = stream.ToArray();
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(template.Length)).CopyTo(template, 8);

            return template;
        }

        public override TemplateBuilder Import(byte[] template)
        {
            TemplateBuilder builder = new TemplateBuilder();
            builder.OriginalDpi = 500;

            MemoryStream stream = new MemoryStream(template);
            BinaryReader reader = new BinaryReader(stream, Encoding.UTF8);

            // 4B magic "FMR\0"
            AssertException.Check(new String(reader.ReadChars(4)) == "FMR\0", "This is not an ISO template.");

            // 4B version (ignored, set to " 20\0"
            reader.ReadChars(4);

            // 4B total length (including header)
            AssertException.Check(IPAddress.NetworkToHostOrder(reader.ReadInt32()) == template.Length, "Invalid template length.");

            // 2B rubbish (zeroed)
            reader.ReadInt16();

            // 2B image size in pixels X
            builder.StandardDpiWidth = IPAddress.NetworkToHostOrder(reader.ReadInt16());

            // 2B image size in pixels Y
            builder.StandardDpiHeight = IPAddress.NetworkToHostOrder(reader.ReadInt16());

            // 2B rubbish (pixels per cm X, set to 196 = 500dpi)
            reader.ReadInt16();

            // 2B rubbish (pixels per cm Y, set to 196 = 500dpi)
            reader.ReadInt16();

            // 1B rubbish (number of fingerprints, set to 1)
            AssertException.Check(reader.ReadByte() == 1, "Only single-fingerprint ISO templates are supported.");

            // 1B rubbish (zeroed)
            reader.ReadByte();

            // 1B rubbish (finger position, zeroed)
            reader.ReadByte();

            // 1B rubbish (zeroed)
            reader.ReadByte();

            // 1B rubbish (fingerprint quality, set to 100)
            reader.ReadByte();

            // 1B minutia count
            int minutiaCount = reader.ReadByte();

            // N*6B minutiae
            for (int i = 0; i < minutiaCount; ++i)
            {
                TemplateBuilder.Minutia minutia = new TemplateBuilder.Minutia();

                //      2B minutia position X in pixels
                //          2b (upper) minutia type (01 ending, 10 bifurcation, 00 other (considered ending))
                ushort xPacked = (ushort)IPAddress.NetworkToHostOrder(reader.ReadInt16());
                minutia.Position.X = xPacked & (ushort)0x3fff;
                switch (xPacked & (ushort)0xc000)
                {
                    case 0x4000: minutia.Type = TemplateBuilder.MinutiaType.Ending; break;
                    case 0x8000: minutia.Type = TemplateBuilder.MinutiaType.Bifurcation; break;
                    case 0: minutia.Type = TemplateBuilder.MinutiaType.Other; break;
                    default: throw new ApplicationException();
                }

                //      2B minutia position Y in pixels (upper 2b ignored, zeroed)
                minutia.Position.Y = builder.StandardDpiHeight - 1 - ((ushort)IPAddress.NetworkToHostOrder(reader.ReadInt16()) & (ushort)0x3fff);

                //      1B direction, compatible with SourceAFIS angles
                minutia.Direction = reader.ReadByte();

                //      1B quality (ignored, zeroed)
                reader.ReadByte();

                builder.Minutiae.Add(minutia);
            }

            // 2B rubbish (extra data length, zeroed)
            // N*1B rubbish (extra data)

            return builder;
        }

        public override void Serialize(Stream stream, byte[] template)
        {
            stream.Write(template, 0, template.Length);
        }

        public override byte[] Deserialize(Stream stream)
        {
            byte[] header = new byte[12];
            stream.Read(header, 0, 12);

            int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(header, 8));

            byte[] template = new byte[length];
            header.CopyTo(template, 0);
            stream.Read(template, 12, length - 12);
            return template;
        }
    }
}
