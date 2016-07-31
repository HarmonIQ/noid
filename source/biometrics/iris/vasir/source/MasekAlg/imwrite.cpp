/* from http://source.macgimp.org/plug-ins/bmp/bmpwrite.c*/

/* bmpwrite.c	Writes Bitmap files. Even RLE encoded ones.	 */
/*		(Windows (TM) doesn't read all of those, but who */
/*		cares? ;-)					 */
/*              I changed a few things over the time, so perhaps */
/*              it dos now, but now there's no Windows left on   */
/*              my computer...                                   */

/* Alexander.Schulz@stud.uni-karlsruhe.de			 */

/*
 * The GIMP -- an image manipulation program
 * Copyright (C) 1995 Spencer Kimball and Peter Mattis
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.
 * ----------------------------------------------------------------------------
 */

//#include "config.h"

#include <errno.h>
#ifdef HAVE_UNISTD_H
#include <unistd.h>
#endif

#include "imread.h"
#include "Masek.h"

#define MAXCOLORS       256
#define GIMP_MIN_RESOLUTION  5e-3      /*  shouldn't display as 0.000  */
#define GIMP_MAX_RESOLUTION  65536.0

//#include <gtk/gtk.h>

//#include <libgimp/gimp.h>
//#include <libgimp/gimpui.h>

//#include "bmp.h"

//#include "libgimp/stdplugins-intl.h"




static int encoded      = 0;

void WriteImage (FILE   *,
	    unsigned char *src,
	    int    width,
	    int    height,
	    int    encoded,
	    int    channels,
	    int    bpp,
	    int    spzeile,
	    int    MapSize);




static void
FromL (int  wert,//int32
       unsigned char *bopuffer)
{
  bopuffer[0] = (wert & 0x000000ff)>>0x00;
  bopuffer[1] = (wert & 0x0000ff00)>>0x08;
  bopuffer[2] = (wert & 0x00ff0000)>>0x10;
  bopuffer[3] = (wert & 0xff000000)>>0x18;
}

static void
FromS (int  wert,//int16
       unsigned char *bopuffer)
{
  bopuffer[0] = (wert & 0x00ff)>>0x00;
  bopuffer[1] = (wert & 0xff00)>>0x08;
}

static void
WriteColorMap (FILE *f,
	       int  red[MAXCOLORS],
	       int  green[MAXCOLORS],
	       int  blue[MAXCOLORS],
	       int  size)
{
  char trgb[4];
  int i;

  size /= 4;
  trgb[3] = 0;
  for (i = 0; i < size; i++)
    {
      trgb[0] = (unsigned char) blue[i];
      trgb[1] = (unsigned char) green[i];
      trgb[2] = (unsigned char) red[i];
      fwrite(trgb, 1, 4, f);
      //      WriteND (f, trgb, 4);
    }
}

void WriteND(FILE *f, void *bufferND, int lenND)
{
	fwrite(bufferND, 1, lenND, f);
}

//GimpPDBStatusType
void Masek::imwrite (const char *filename,
                     Masek::IMAGE *image)//, //int32
	  //int       drawable_ID) //int32
{
  FILE *outfile;
  int Red[MAXCOLORS];
  int Green[MAXCOLORS];
  int Blue[MAXCOLORS];
  int rows, cols, Spcols, channels, MapSize, SpZeile;
  long BitsPerPixel;
  int colors;
  Bitmap_File_Head_Struct_ND Bitmap_File_Head_ND;
  Bitmap_Head_Struct_ND Bitmap_Head_ND;

  //unsigned char *pixels;
 // GimpPixelRgn pixel_rgn;
//  GimpDrawable *drawable;
  //GimpImageType drawable_type;
  unsigned char puffer[50];
  int i;

  colors       = 256;
  BitsPerPixel = 8;
  MapSize      = 1024;
  channels     = 1;
  for (i = 0; i < colors; i++)
  {
	  Red[i]   = i;
	  Green[i] = i;
	  Blue[i]  = i;
  }
  
  
  

  /* Perhaps someone wants RLE encoded Bitmaps */
  encoded = 0;

  /* Let's take some file */
  outfile = fopen (filename, "wb");
  if (!outfile)
    {
      printf("Could not open '%s' for writing\n",
                 filename);
      return;
    }

  /* fetch the image */
  //pixels = (unsigned char*)malloc (sizeof(unsigned char)* image->hsize[0]*image->hsize[1] * channels);

  /* Now, we need some further information ... */
  cols = image->hsize[1];
  rows = image->hsize[0];

  /* ... that we write to our headers. */
  if ((BitsPerPixel != 24) &&
      (cols % (8/BitsPerPixel)))
    Spcols = (((cols / (8 / BitsPerPixel)) + 1) * (8 / BitsPerPixel));
  else
    Spcols = cols;

  if ((((Spcols * BitsPerPixel) / 8) % 4) == 0)
    SpZeile = ((Spcols * BitsPerPixel) / 8);
  else
    SpZeile = ((int) (((Spcols * BitsPerPixel) / 8) / 4) + 1) * 4;

  Bitmap_File_Head_ND.bfSize    = 14;//0x36 + MapSize + (rows * SpZeile);
  Bitmap_File_Head_ND.zzHotX    = 0;
  Bitmap_File_Head_ND.zzHotY    = 0;
  Bitmap_File_Head_ND.bfOffs    = 0x36 + MapSize;
  Bitmap_File_Head_ND.biSize    = 40;

  Bitmap_Head_ND.biWidth  = cols;
  Bitmap_Head_ND.biHeight = rows;
  Bitmap_Head_ND.biPlanes = 1;
  Bitmap_Head_ND.biBitCnt = (unsigned short)BitsPerPixel;

  if (encoded == 0)
    Bitmap_Head_ND.biCompr = 0;
  else if (BitsPerPixel == 8)
    Bitmap_Head_ND.biCompr = 1;
  else if (BitsPerPixel == 4)
    Bitmap_Head_ND.biCompr = 2;
  else
    Bitmap_Head_ND.biCompr = 0;

  Bitmap_Head_ND.biSizeIm = SpZeile * rows;

  Bitmap_Head_ND.biXPels = (long int) 0;//xresolution * 100.0 / 2.54;
  Bitmap_Head_ND.biYPels = (long int) 0;//yresolution * 100.0 / 2.54;

  if (BitsPerPixel < 24)
    Bitmap_Head_ND.biClrUsed = colors;
  else
    Bitmap_Head_ND.biClrUsed = 0;

  Bitmap_Head_ND.biClrImp = 0;//Bitmap_Head.biClrUsed;


  /* And now write the header and the colormap (if any) to disk */
  unsigned char BMtemp[2];
  BMtemp[0] = 'B';
  BMtemp[1] = 'M';
  WriteND (outfile, BMtemp, 2);

  FromL (Bitmap_File_Head_ND.bfSize, &puffer[0x00]);
  FromS (Bitmap_File_Head_ND.zzHotX, &puffer[0x04]);
  FromS (Bitmap_File_Head_ND.zzHotY, &puffer[0x06]);
  FromL (Bitmap_File_Head_ND.bfOffs, &puffer[0x08]);
  FromL (Bitmap_File_Head_ND.biSize, &puffer[0x0C]);

  WriteND (outfile, puffer, 16);

  FromL (Bitmap_Head_ND.biWidth, &puffer[0x00]);
  FromL (Bitmap_Head_ND.biHeight, &puffer[0x04]);
  FromS (Bitmap_Head_ND.biPlanes, &puffer[0x08]);
  FromS (Bitmap_Head_ND.biBitCnt, &puffer[0x0A]);
  FromL (Bitmap_Head_ND.biCompr, &puffer[0x0C]);
  FromL (Bitmap_Head_ND.biSizeIm, &puffer[0x10]);
  FromL (Bitmap_Head_ND.biXPels, &puffer[0x14]);
  FromL (Bitmap_Head_ND.biYPels, &puffer[0x18]);
  FromL (Bitmap_Head_ND.biClrUsed, &puffer[0x1C]);
  FromL (Bitmap_Head_ND.biClrImp, &puffer[0x20]);

  WriteND (outfile, puffer, 36);
  WriteColorMap (outfile, Red, Green, Blue, MapSize);

  /* After that is done, we write the image ... */

  WriteImage (outfile,
	      image->data, cols, rows,
	      encoded, channels, BitsPerPixel, SpZeile, MapSize);

  /* ... and exit normally */

  fclose (outfile);
//  gimp_drawable_detach (drawable);
//  free (pixels);

 // return GIMP_PDB_SUCCESS;
}

void
WriteImage (FILE   *f,
	    unsigned char *src,
	    int    width,
	    int    height,
	    int    encoded,
	    int    channels,
	    int    bpp,
	    int    spzeile,
	    int    MapSize)
{
  unsigned char buf[16]={0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0};
  unsigned char puffer[8];
  unsigned char *temp, v;
  unsigned char *Zeile, *ketten;
  int xpos, ypos, i, j, rowstride, laenge, thiswidth;
  int breite, n, k;

  xpos = 0;
  rowstride = width * channels;

  /* We'll begin with the 24 bit Bitmaps, they are easy :-) */

  if (bpp == 24)
  {
      for (ypos = height - 1; ypos >= 0; ypos--)  /* for each row   */
	  {
		for (i = 0; i < width; i++)  /* for each pixel */
	    {
	      temp = src + (ypos * rowstride) + (xpos * channels);
	      buf[2] = (unsigned char) *temp;
	      temp++;
	      buf[1] = (unsigned char) *temp;
	      temp++;
	      buf[0] = (unsigned char) *temp;
	      xpos++;
	      WriteND (f, buf, 3);
	    }
	  WriteND (f, &buf[3], spzeile - (width * 3));


	  xpos = 0;
	}
  }
  else
  {
      switch (encoded)  /* now it gets more difficult  */
	{               /* uncompressed 1,4 and 8 bit */
	case 0:
	  
	    thiswidth = (width / (8 / bpp));
	    if (width % (8 / bpp))
	      thiswidth++;

	    for (ypos = height - 1; ypos >= 0; ypos--) /* for each row */
	    {
		  for (xpos = 0; xpos < width;)  /* for each _byte_ */
		  {
		    v = 0;
		    for (i = 1; (i <= (8 / bpp)) && (xpos < width);	 i++, xpos++)  /* for each pixel */
		    {
				temp = src + (ypos * rowstride) + (xpos * channels);
				v=v | ((unsigned char) *temp << (8 - (i * bpp)));
		     }
		    WriteND (f, &v, 1);
		  }
		  WriteND (f, &buf[3], spzeile - thiswidth);
		  xpos = 0;
		}
	    break;
	  
	default:
	  		 /* Save RLE encoded file, quite difficult */
	    laenge = 0;
	    buf[12] = 0;
	    buf[13] = 1;
	    buf[14] = 0;
	    buf[15] = 0;
	    Zeile = (unsigned char *) malloc (width / (8 / bpp) + 10);
	    ketten = (unsigned char *) malloc (width / (8 / bpp) + 10);
	    for (ypos = height - 1; ypos >= 0; ypos--)
	    {	/* each row separately */
			j = 0;
			/* first copy the pixels to a buffer,
			* making one byte from two 4bit pixels
			*/
			for (xpos = 0; xpos < width;)
			{
				v = 0;
				for (i = 1;	 (i <= (8 / bpp)) && (xpos < width); i++, xpos++)
		       {	/* for each pixel */
					temp = src + (ypos * rowstride) + (xpos * channels);
					v = v | ((unsigned char) * temp << (8 - (i * bpp)));
		       }
				Zeile[j++] = v;
		    }
			breite = width / (8 / bpp);
			if (width % (8 / bpp))
				breite++;
			/* then check for strings of equal bytes */
			for (i = 0; i < breite;)
		    {
				j = 0;
				while ((i + j < breite) &&   (j < (255 / (8 / bpp))) &&  (Zeile[i + j] == Zeile[i]))
					j++;

				ketten[i] = j;
				i += j;
			}

		/* then write the strings and the other pixels to the file */
			for (i = 0; i < breite;)
			{
				if (ketten[i] < 3)
				/* strings of different pixels ... */
				{
					j = 0;
					while ((i + j < breite) && (j < (255 / (8 / bpp))) &&  (ketten[i + j] < 3))
						j += ketten[i + j];

			/* this can only happen if j jumps over
			 * the end with a 2 in ketten[i+j]
			 */
					if (j > (255 / (8 / bpp)))
						j -= 2;
			/* 00 01 and 00 02 are reserved */
					if (j > 2)
					{
						WriteND (f, &buf[12], 1);
						n = j * (8 / bpp);
						if (n + i * (8 / bpp) > width)
							n--;
						WriteND (f, &n, 1);
						laenge += 2;
						WriteND (f, &Zeile[i], j);
						laenge += j;
						if ((j) % 2)
						{
							WriteND (f, &buf[12], 1);
							laenge++;
						}
					}
					else
					{
						for (k = i; k < i + j; k++)
						{
							n = (8 / bpp);
							if (n + i * (8 / bpp) > width)
								n--;
							WriteND (f, &n, 1);
							WriteND (f, &Zeile[k], 1);
							/*printf("%i.#|",n); */
							laenge += 2;
						}
					}
					i += j;
				}
				else
		      /* strings of equal pixels */
				{
					n = ketten[i] * (8 / bpp);
					if (n + i * (8 / bpp) > width)
						n--;
					WriteND (f, &n, 1);
					WriteND (f, &Zeile[i], 1);
					i += ketten[i];
					laenge += 2;
				}
			}
			WriteND (f, &buf[14], 2);		 /* End of row */
			laenge += 2;

		
		    fseek (f, -2, SEEK_CUR);	 /* Overwrite last End of row ... */
		    WriteND (f, &buf[12], 2);	 /* ... with End of file */

			fseek (f, 0x22, SEEK_SET);            /* Write length of image */
			FromL (laenge, puffer);
			WriteND (f, puffer, 4);
			fseek (f, 0x02, SEEK_SET);            /* Write length of file */
			laenge += (0x36 + MapSize);
			FromL (laenge, puffer);
			WriteND (f, puffer, 4);
			free (ketten);
			free (Zeile);
			break;
	 }
	
	
	}
	}
	
	
}

