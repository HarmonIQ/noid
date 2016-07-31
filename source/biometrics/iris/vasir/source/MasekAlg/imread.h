struct Bitmap_File_Head_Struct_ND
{
  char            zzMagic[2];  /* 00 "BM" */
  unsigned long   bfSize;      /* 02 */
  unsigned short  zzHotX;      /* 06 */
  unsigned short  zzHotY;      /* 08 */
  unsigned long   bfOffs;      /* 0A */
  unsigned long   biSize;      /* 0E */
};

//typedef 
struct Bitmap_Head_Struct_ND
{
  unsigned long   biWidth;     /* 12 */
  unsigned long   biHeight;    /* 16 */
  unsigned short  biPlanes;    /* 1A */
  unsigned short  biBitCnt;    /* 1C */
  unsigned long   biCompr;     /* 1E */
  unsigned long   biSizeIm;    /* 22 */
  unsigned long   biXPels;     /* 26 */
  unsigned long   biYPels;     /* 2A */
  unsigned long   biClrUsed;   /* 2E */
  unsigned long   biClrImp;    /* 32 */
                        /* 36 */
};


