/* from http://source.macgimp.org/plug-ins/bmp/bmpread.c*/
/* bmpread.c	reads any bitmap I could get for testing */
/* Alexander.Schulz@stud.uni-karlsruhe.de                */

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

#include <errno.h>
#include <string.h>
#include "imread.h"

#include "Masek.h"

#include <iostream>
#include <fstream>
#include <string> // Lee: added
using namespace std;


#define ReadOK(file,buffer,len) (fread(buffer, len, 1, file) != 0)
#define	MAXCOLORMAPSIZE		256

#define	TRUE	1
#define	FALSE	0

#define CM_RED		0
#define CM_GREEN	1
#define CM_BLUE		2

static long
ToL (unsigned char *puffer)
{
  return (puffer[0] | puffer[1]<<8 | puffer[2]<<16 | puffer[3]<<24);
}

static short
ToS (unsigned char *puffer)
{
  return (puffer[0] | puffer[1]<<8);
}

//next two functions from http://www.tug.org/tex-archive/graphics/sam2p/input-bmp.ci

static int
ReadColorMap (FILE   *fd,
	      unsigned char  buffer[256][3],
	      int    number,
	      int    size,
	      int   *grey)
{
  int i;
  unsigned char rgb[4];

  *grey=(number>2);
  for (i = 0; i < number ; i++)
    {
      if (!ReadOK (fd, rgb, size))
	  {
          printf ("BMP: Bad colormap\n");
		  return 0;
	  }

      /* Bitmap save the colors in another order! But change only once! */

      buffer[i][0] = rgb[2];
      buffer[i][1] = rgb[1];
      buffer[i][2] = rgb[0];
      *grey = ((*grey) && (rgb[0]==rgb[1]) && (rgb[1]==rgb[2]));
    }
  return 1;
}

static unsigned char*
ReadImage (FILE   *fd,
	   int    width,
	   int    height,
	   unsigned char  cmap[256][3],
	   int    bpp,
	   int    compression,
	   int    rowbytes,
	   int    grey)
{
  unsigned char v,howmuch;
  int xpos = 0, ypos = 0;
  unsigned char *image;
  unsigned char *temp, *buffer;
  long rowstride, channels;
  unsigned short rgb;
  int i, j, notused;



  if (bpp >= 16) /* color image */
    {
//      XMALLOCT (image, unsigned char*, width * height * 3 * sizeof (unsigned char));
		image = (unsigned char*) malloc(width * height * 3 * sizeof (unsigned char));
      channels = 3;
    }
  else if (grey) /* grey image */
    {
      //XMALLOCT (image, unsigned char*, width * height * 1 * sizeof (unsigned char));
		image = (unsigned char*)malloc(width * height * 1 * sizeof (unsigned char));
	  channels = 1;
	}
  else /* indexed image */
	{
      //XMALLOCT (image, unsigned char*, width * height * 1 * sizeof (unsigned char));
		image = (unsigned char*) malloc(width * height * 1 * sizeof (unsigned char));
	  channels = 1;
	}

  //XMALLOCT (buffer, unsigned char*, rowbytes); 
	buffer = (unsigned char*)malloc(rowbytes); 
  rowstride = width * channels;

  ypos = height - 1;  /* Bitmaps begin in the lower left corner */

  switch (bpp) {

  case 32:
    {
      while (ReadOK (fd, buffer, rowbytes))
        {
          temp = image + (ypos * rowstride);
          for (xpos= 0; xpos < width; ++xpos)
            {
               *(temp++)= buffer[xpos * 4 + 2];
               *(temp++)= buffer[xpos * 4 + 1];
               *(temp++)= buffer[xpos * 4];
            }
          --ypos; /* next line */
        }
    }
	break;

  case 24:
    {
      while (ReadOK (fd, buffer, rowbytes))
        {
          temp = image + (ypos * rowstride);
          for (xpos= 0; xpos < width; ++xpos)
            {
               *(temp++)= buffer[xpos * 3 + 2];
               *(temp++)= buffer[xpos * 3 + 1];
               *(temp++)= buffer[xpos * 3];
            }
          --ypos; /* next line */
        }
	}
    break;

  case 16:
    {
      while (ReadOK (fd, buffer, rowbytes))
        {
          temp = image + (ypos * rowstride);
          for (xpos= 0; xpos < width; ++xpos)
            {
               rgb= ToS(&buffer[xpos * 2]);
               *(temp++)= (unsigned char)(((rgb >> 10) & 0x1f) * 8);
               *(temp++)= (unsigned char)(((rgb >> 5)  & 0x1f) * 8);
               *(temp++)= (unsigned char)(((rgb)       & 0x1f) * 8);
            }
          --ypos; /* next line */
        }
    }
	break;

  case 8:
  case 4:
  case 1:
    {
      if (compression == 0)
	  {
	    while (ReadOK (fd, &v, 1))
	      {
		for (i = 1; (i <= (8 / bpp)) && (xpos < width); i++, xpos++)
		  {
		    temp = (unsigned char*) (image + (ypos * rowstride) + (xpos * channels));
		    *temp= (unsigned char)(( v & ( ((1<<bpp)-1) << (8-(i*bpp)) ) ) >> (8-(i*bpp)));
		  }
		if (xpos == width)
		  {
		    notused = ReadOK (fd, buffer, rowbytes - 1 -
                                                (width * bpp - 1) / 8);
		    ypos--;
		    xpos = 0;

		  }
		if (ypos < 0)
		  break;
	      }
	    break;
	  }
	else
	  {
	    while (ypos >= 0 && xpos <= width)
	      {
		notused = ReadOK (fd, buffer, 2);
		if ((unsigned char) buffer[0] != 0) 
		  /* Count + Color - record */
		  {
		    for (j = 0; ((unsigned char) j < (unsigned char) buffer[0]) && (xpos < width);)
		      {
#ifdef DEBUG2
			printf("%u %u | ",xpos,width);
#endif
			for (i = 1;
			     ((i <= (8 / bpp)) &&
			      (xpos < width) &&
			      ((unsigned char) j < (unsigned char) buffer[0]));
			     i++, xpos++, j++)
			  {
			    temp = image + (ypos * rowstride) + (xpos * channels);
			    *temp = (unsigned char) ((buffer[1] & (((1<<bpp)-1) << (8 - (i * bpp)))) >> (8 - (i * bpp)));
			  }
		      }
		  }
		if (((unsigned char) buffer[0] == 0) && ((unsigned char) buffer[1] > 2))
		  /* uncompressed record */
		  {
		    howmuch = buffer[1];
		    for (j = 0; j < howmuch; j += (8 / bpp))
		      {
			notused = ReadOK (fd, &v, 1);
			i = 1;
			while ((i <= (8 / bpp)) && (xpos < width))
			  {
			    temp = image + (ypos * rowstride) + (xpos * channels);
			    *temp = (unsigned char) ((v & (((1<<bpp)-1) << (8-(i*bpp)))) >> (8-(i*bpp)));
			    i++;
			    xpos++;
			  }
		      }

		    if ((howmuch % 2) && (bpp==4))
		      howmuch++;

		    if ((howmuch / (8 / bpp)) % 2)
		      notused = ReadOK (fd, &v, 1);
		    /*if odd(x div (8 div bpp )) then blockread(f,z^,1);*/
		  }
		if (((unsigned char) buffer[0] == 0) && ((unsigned char) buffer[1]==0))
		  /* Line end */
		  {
		    ypos--;
		    xpos = 0;
		  }
		if (((unsigned char) buffer[0]==0) && ((unsigned char) buffer[1]==1))
		  /* Bitmap end */
		  {
		    break;
		  }
		if (((unsigned char) buffer[0]==0) && ((unsigned char) buffer[1]==2))
		  /* Deltarecord */
		  {
		    notused = ReadOK (fd, buffer, 2);
		    xpos += (unsigned char) buffer[0];
		    ypos -= (unsigned char) buffer[1];
		  }
	      }
	    break;
	  }
    }
    break;
  default:
    /* This is very bad, we should not be here */
	;
  }

  fclose (fd);
  if (bpp <= 8)
    {
      unsigned char *temp2, *temp3;
      unsigned char index;
      temp2 = temp = image;
      //XMALLOCT (image, unsigned char*, width * height * 3 * sizeof (unsigned char));
	  image = (unsigned char*)malloc( width * height * 3 * sizeof (unsigned char));
      temp3 = image;
      for (ypos = 0; ypos < height; ypos++)
        {
          for (xpos = 0; xpos < width; xpos++)
             {
               index = *temp2++;
               *temp3++ = cmap[index][0];
			   if (!grey)
			     {
                   *temp3++ = cmap[index][1];
                   *temp3++ = cmap[index][2];
			     }
           }
        }
      free (temp);
  }

  free (buffer);
  return image;
}


Masek::IMAGE* Masek::imread(const char *name)
{
  FILE *fd;
  unsigned char buffer[64];
  int ColormapSize, rowbytes, Maps, Grey;
  unsigned char ColorMap[256][3];
  unsigned char * image_ID;
  unsigned char magick[2];
  IMAGE *m_image;
  //struct bmpfile_header	Bitmap_File_Head_ND;
  //struct bmp_header Bitmap_Head_ND;

  Bitmap_File_Head_Struct_ND Bitmap_File_Head_ND;
  Bitmap_Head_Struct_ND Bitmap_Head_ND;

  //LEE Test
  //name = "C:/Iris2008/Develop/YooMasek/bin/eyeImage6.bmp";
  
  fd = fopen (name, "rb");

  if (!fd)
    {
      printf ("Could not open '%s' for reading: %s\n",
                 name, strerror (errno));
      return NULL;
    }

/*  temp_buf = g_strdup_printf (_("Opening '%s'..."), name);
  gimp_progress_init (temp_buf);
  g_free (temp_buf);
*/
  /* It is a File. Now is it a Bitmap? Read the shortest possible header */
  bool readFlag = ReadOK(fd, magick, 2);
  const string BA ="BA";
  string magickCPP = "xx";
  magickCPP[0] = magick[0];
  magickCPP[1] = magick[1];

  printf("magick=|%s|\n",magick);
  cout << "magickCPP=|"<<magickCPP<<"|"<<endl;
  if (!readFlag || 
      !((magickCPP=="BA") ||
	magickCPP=="BM" || magickCPP=="IC" ||
	magickCPP=="PI" || magickCPP=="CI" ||
	magickCPP=="CP")) 
    {
      printf ("1st: '%s' is not a valid BMP file\n", name);
      return NULL;
    }
	
  cout<<"here 3"<<endl;


  while ((magickCPP=="BA"))
    {
      if (!ReadOK(fd, buffer, 12))
	  {
		printf("2nd: '%s' is not a valid BMP file", name);
          return NULL;
        }
      if (!ReadOK(fd, magick, 2))
	{
          printf("3rd: '%s' is not a valid BMP file", name);
          return NULL;
        }
      magickCPP[0] = magick[0];
      magickCPP[1] = magick[1];
      printf("magick=|%s|\n",magick);
      cout << "magickCPP=|"<<magickCPP<<"|"<<endl;
     
    }
  cout<<"here 4"<<endl;

  if (!ReadOK(fd, buffer, 12))
    {
      printf("4th: '%s' is not a valid BMP file", name);
      return NULL;
    }

  /* bring them to the right byteorder. Not too nice, but it should work */

  Bitmap_File_Head_ND.bfSize    = ToL (&buffer[0x00]);
  Bitmap_File_Head_ND.zzHotX    = ToS (&buffer[0x04]);
  Bitmap_File_Head_ND.zzHotY    = ToS (&buffer[0x06]);
  Bitmap_File_Head_ND.bfOffs    = ToL (&buffer[0x08]);

  if (!ReadOK(fd, buffer, 4))
    {
      printf("5th: '%s' is not a valid BMP file", name);
      return NULL;
    }

  Bitmap_File_Head_ND.biSize    = ToL (&buffer[0x00]);

  /* What kind of bitmap is it? */
  cout<<"here 1"<<endl;
  if (Bitmap_File_Head_ND.biSize == 12) /* OS/2 1.x ? */
    {
      if (!ReadOK (fd, buffer, 8))
        {
          printf("Error reading BMP file header from '%s'", name);
          return NULL;
        }

      Bitmap_Head_ND.biWidth   =ToS (&buffer[0x00]);       /* 12 */
      Bitmap_Head_ND.biHeight  =ToS (&buffer[0x02]);       /* 14 */
      Bitmap_Head_ND.biPlanes  =ToS (&buffer[0x04]);       /* 16 */
      Bitmap_Head_ND.biBitCnt  =ToS (&buffer[0x06]);       /* 18 */
      Bitmap_Head_ND.biCompr = 0;
      Bitmap_Head_ND.biSizeIm = 0;
      Bitmap_Head_ND.biXPels = Bitmap_Head_ND.biYPels = 0;
      Bitmap_Head_ND.biClrUsed = 0;
      Maps = 3;
    }
  else if (Bitmap_File_Head_ND.biSize == 40) /* Windows 3.x */
    {
      if (!ReadOK (fd, buffer, Bitmap_File_Head_ND.biSize - 4))
        {
          printf("Error reading BMP file header from '%s'", name);
          return NULL;
        }
      Bitmap_Head_ND.biWidth   =ToL (&buffer[0x00]);	/* 12 */
      Bitmap_Head_ND.biHeight  =ToL (&buffer[0x04]);	/* 16 */
      Bitmap_Head_ND.biPlanes  =ToS (&buffer[0x08]);       /* 1A */
      Bitmap_Head_ND.biBitCnt  =ToS (&buffer[0x0A]);	/* 1C */
      Bitmap_Head_ND.biCompr   =ToL (&buffer[0x0C]);	/* 1E */
      Bitmap_Head_ND.biSizeIm  =ToL (&buffer[0x10]);	/* 22 */
      Bitmap_Head_ND.biXPels   =ToL (&buffer[0x14]);	/* 26 */
      Bitmap_Head_ND.biYPels   =ToL (&buffer[0x18]);	/* 2A */
      Bitmap_Head_ND.biClrUsed =ToL (&buffer[0x1C]);	/* 2E */
      Bitmap_Head_ND.biClrImp  =ToL (&buffer[0x20]);	/* 32 */
    					                /* 36 */
      Maps = 4;
    }
  else if (Bitmap_File_Head_ND.biSize <= 64) /* Probably OS/2 2.x */
    {
      if (!ReadOK (fd, buffer, Bitmap_File_Head_ND.biSize - 4))
        {
          printf("Error reading BMP file header from '%s'", name);
          return NULL;
        }
      Bitmap_Head_ND.biWidth   =ToL (&buffer[0x00]);       /* 12 */
      Bitmap_Head_ND.biHeight  =ToL (&buffer[0x04]);       /* 16 */
      Bitmap_Head_ND.biPlanes  =ToS (&buffer[0x08]);       /* 1A */
      Bitmap_Head_ND.biBitCnt  =ToS (&buffer[0x0A]);       /* 1C */
      Bitmap_Head_ND.biCompr   =ToL (&buffer[0x0C]);       /* 1E */
      Bitmap_Head_ND.biSizeIm  =ToL (&buffer[0x10]);       /* 22 */
      Bitmap_Head_ND.biXPels   =ToL (&buffer[0x14]);       /* 26 */
      Bitmap_Head_ND.biYPels   =ToL (&buffer[0x18]);       /* 2A */
      Bitmap_Head_ND.biClrUsed =ToL (&buffer[0x1C]);       /* 2E */
      Bitmap_Head_ND.biClrImp  =ToL (&buffer[0x20]);       /* 32 */
                                                        /* 36 */
      Maps = 3;
    }
  else
    {
      printf("Error reading BMP file header from '%s'", name);
      return NULL;
    }
  cout<<"here 2"<<endl;

  /* Valid bitpdepthis 1, 4, 8, 16, 24, 32 */
  /* 16 is awful, we should probably shoot whoever invented it */

  /* There should be some colors used! */

  ColormapSize = (Bitmap_File_Head_ND.bfOffs - Bitmap_File_Head_ND.biSize - 14) / Maps;

  if ((Bitmap_Head_ND.biClrUsed == 0) && (Bitmap_Head_ND.biBitCnt <= 8))
    ColormapSize = Bitmap_Head_ND.biClrUsed = 1 << Bitmap_Head_ND.biBitCnt;


  /* Sanity checks */

  if (Bitmap_Head_ND.biHeight == 0 || Bitmap_Head_ND.biWidth == 0) {
      printf("Error reading BMP file header from '%s'", name);
      return NULL;
  }

  if (Bitmap_Head_ND.biPlanes != 1) {
      printf("Error reading BMP file header from '%s'", name);
      return NULL;
  }

  if (ColormapSize > 256 || Bitmap_Head_ND.biClrUsed > 256) {
      printf("Error reading BMP file header from '%s'", name);
      return NULL;
  }

  /* Windows and OS/2 declare filler so that rows are a multiple of
   * word length (32 bits == 4 bytes)
   */

  rowbytes= ( (Bitmap_Head_ND.biWidth * Bitmap_Head_ND.biBitCnt - 1) / 32) * 4 + 4;

#ifdef DEBUG
  printf("\nSize: %u, Colors: %u, Bits: %u, Width: %u, Height: %u, Comp: %u, Zeile: %u\n",
          Bitmap_File_Head_ND.bfSize,Bitmap_Head_ND.biClrUsed,Bitmap_Head_ND.biBitCnt,Bitmap_Head_ND.biWidth,
          Bitmap_Head_ND.biHeight, Bitmap_Head_ND.biCompr, rowbytes);
#endif

  /* Get the Colormap */

  if (!ReadColorMap (fd, ColorMap, ColormapSize, Maps, &Grey))
    return NULL;

#ifdef DEBUG
  printf("Colormap read\n");
#endif

  fseek(fd, Bitmap_File_Head_ND.bfOffs, SEEK_SET);
  cout << "before from ReadImage"<<endl;

  /* Get the Image and return the ID or -1 on error*/
  image_ID = ReadImage (fd,
			Bitmap_Head_ND.biWidth,
			Bitmap_Head_ND.biHeight,
			ColorMap,
	//		Bitmap_Head_ND.biClrUsed,
			Bitmap_Head_ND.biBitCnt,
			Bitmap_Head_ND.biCompr,
			rowbytes,
			Grey);
  cout << "back from ReadImage"<<endl;
  if (Bitmap_Head_ND.biXPels > 0 && Bitmap_Head_ND.biYPels > 0)
    {
      /* Fixed up from scott@asofyet's changes last year, njl195 */
      double xresolution;
      double yresolution;

      /* I don't agree with scott's feeling that Gimp should be
       * trying to "fix" metric resolution translations, in the
       * long term Gimp should be SI (metric) anyway, but we
       * haven't told the Americans that yet  */

      xresolution = Bitmap_Head_ND.biXPels * 0.0254;
      yresolution = Bitmap_Head_ND.biYPels * 0.0254;

     // gimp_image_set_resolution (image_ID, xresolution, yresolution);
    }

	m_image = (IMAGE*) malloc(sizeof(IMAGE));
	if (m_image >0)
	{
		m_image->data = image_ID;
		m_image->hsize[0] = Bitmap_Head_ND.biHeight;
		m_image->hsize[1] = Bitmap_Head_ND.biWidth;
		
	}
	cout << "end of imread"<<endl;
  return m_image;
}
