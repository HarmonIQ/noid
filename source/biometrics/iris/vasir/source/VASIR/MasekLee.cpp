#include "MasekLee.h"
#include <string.h>
#include <math.h>

#define miINT8 1
#define miUINT8 2
#define miINT16 3
#define miUINT16 4
#define miINT32 5
#define miUINT32 6
#define miSINGLE 7
#define miDOUBLE 9
#define	miINT64	12
#define miUINT64 13
#define	miMATRIX 14
#define miCOMPRESSED 15
#define miUTF8 16
#define miUTF16 17
#define miUTF32 18

#define mxDOUBLE_CLASS 6
#define mxINT32_CLASS 12

MasekLee::MasekLee()
{
    //masek = NULL;
}

MasekLee::~MasekLee()
{
   // if(masek != NULL)
   //     delete masek;
}



int MasekLee::getvalue(unsigned char *str, int len, int reverse)
{
    int i;
    int value;
    value = 0;
    if (reverse==0)
    {
        for (i = 0; i<len; i++)
        {
            value*=256;
            value+=str[i];

        }
    }
    else
    {
        for (i = len-1; i>=0; i--)
        {
            value*=256;
            value+=str[i];

        }
    }
    return value;

}

void MasekLee::setvalue(unsigned char *str, int value, int len)
{
    int i;


    for (i = 0; i<len; i++)
    {
        str[i] = value%256;
        value = value/256;
    }

}

void MasekLee::gettypebytes(unsigned char *str, int reverse, int *type, int *bytes, int *next)
{

    if ((!reverse && (str[0]!=0 || str[1]!=0)) || (reverse && (str[3]!=0 || str[2]!=0)))
    {
        //small
//		*bytes = getvalue(str, 2, reverse);
//		*type = getvalue(str+2, 2, reverse);
        *bytes = getvalue(str+2, 2, reverse);
        *type = getvalue(str, 2, reverse);
        *next=4;
    }
    else
    {
        *type = getvalue(str, 4, reverse);
        *bytes = getvalue(str+4, 4, reverse);
        *next=8;
    }
}

int MasekLee::findline(Masek::IMAGE *image, double **lines)
{

    Masek::filter *I2, *orND, *I3, *I4, *edgeimage;
    double theta[180];
    double *R, *xp;
    double maxval;
    int size, linecount;
    double cx, cy;
    int i, j, index, x, y;
    double r, t;
    int tmpcount;
    FILE *fid;
    Masek *masek = NULL;

    fid = fopen("image.txt", "w");
    for (i = 0; i<image->hsize[0]; i++)
    {
        for (j = 0; j<image->hsize[1]; j++)
            fprintf(fid, "%d %d %d\n", i+1, j+1, image->data[i*image->hsize[1]+j]);

    }
    fclose(fid);

    I2 = (Masek::filter*)malloc(sizeof(Masek::filter));
    orND = (Masek::filter*)malloc(sizeof(Masek::filter));


    image = masek->canny(image, 2, 1, 0.00, 1.00, I2, orND);
    //imwrite("C:\\testImage090716.bmp", image);

    /*fid = fopen("I2.txt", "w");
    for (i = 0; i<I2->hsize[0]; i++)
    {
        for (j = 0; j<I2->hsize[1]; j++)
            fprintf(fid, "%d %d %f\n", i+1, j+1, I2->data[i*I2->hsize[1]+j]);
    }
    fclose(fid);

    fid = fopen("or.txt", "w");
    for (i = 0; i<or->hsize[0]; i++)
    {
        for (j = 0; j<or->hsize[1]; j++)
            fprintf(fid, "%d %d %f\n", i+1, j+1, or->data[i*or->hsize[1]+j]);

    }
    fclose(fid);*/


    I3 = masek->adjgamma(I2, 1.9);
    //I3 = Masek::adjgamma(I2, 1.9);
    /*fid = fopen("I3.txt", "w");
    for (i = 0; i<I3->hsize[0]; i++)
    {
        for (j = 0; j<I3->hsize[1]; j++)
            fprintf(fid, "%d %d %f\n", i+1, j+1, I3->data[i*I3->hsize[1]+j]);

    }
    fclose(fid);*/

    I4 = masek->nonmaxsup(I3, orND, 1.5);
    
    /*fid = fopen("I4.txt", "w");
    for (i = 0; i<I4->hsize[0]; i++)
    {
        for (j = 0; j<I4->hsize[1]; j++)
            fprintf(fid, "%d %d %f\n", i+1, j+1, I4->data[i*I4->hsize[1]+j]);

    }
    fclose(fid);*/

    edgeimage = masek->hysthresh(I4, 0.20, 0.15);
    

    for (i = 0; i<180; i++)
        theta[i] = i;

    /*fid = fopen("edge.txt", "w");
    for (i = 0; i<edgeimage->hsize[0]; i++)
    {
        for (j = 0; j<edgeimage->hsize[1]; j++)
            fprintf(fid, "%d %d %f\n", i+1, j+1, edgeimage->data[i*edgeimage->hsize[1]+j]);

    }
    fclose(fid);*/

    size = masek->radonc(edgeimage->data, theta, edgeimage->hsize[0], edgeimage->hsize[1], 180, &R, &xp);



    /*fid = fopen("r.txt", "w");
    for (i = 0; i<size; i++)
    {
        for (j = 0; j<180; j++)
            fprintf(fid, "%d %d %f\n", i+1, j+1, R[i*180+j]);

    }
    fclose(fid);
    fid = fopen("xp.txt", "w");
    for (i = 0; i<size; i++)
    {
        fprintf(fid, "%f\n", xp[i]);

    }
    fclose(fid);
    */

    maxval = -1;
    index = -1;


    linecount=0;
    for (i = 0; i<size*180; i++)
    {
        if (R[i]>maxval)
        {
            maxval = R[i];
            index = i;
            linecount=1;
        }
        else if (R[i]==maxval)
        {
            linecount++;
        }
    }

    if (maxval<=25)
        return 0;


    *lines = (double*) malloc(sizeof(double)*linecount*3);


    cx = image->hsize[1]/2.0-1;
    cy = image->hsize[0]/2.0-1;

    tmpcount=0;

    for (i = index; i<size*180; i++)
    {
        if (R[i]==maxval)
        {
            y = i/180;
            x = i%180;
            t = -theta[x]*PI/180;
            r = xp[y];

            (*lines)[tmpcount*3] = cos(t);
            (*lines)[tmpcount*3+1] = sin(t);
            (*lines)[tmpcount*3+2] = -r;
            (*lines)[tmpcount*3+2] = (*lines)[tmpcount*3+2] - (*lines)[tmpcount*3]*cx - (*lines)[tmpcount*3+1]*cy;
            tmpcount++;
        }
    }

    free(R);
    free(xp);
    free(I2->data);
    free(I2);
    free(orND->data);
    free(orND);
    //delete masek;
    return linecount;
}
void MasekLee::findcircle(Masek::IMAGE *image, int lradius, int uradius, double scaling, double sigma, double hithres, double lowthres, double vert, double horz,
                       int *_row, int *_col, int *_r)
{
    int lradsc, uradsc, rd;
    Masek::filter *gradient, *orND;
    int i, j, k, cur;
    Masek::filter *I3, *I4, *edgeimage;
    int row, col, tmpi, count;
    double maxtotal;//, tt;
    double *h;
    int r;
//	FILE *fin;
//	char tchar[50];
    Masek *masek = NULL;

    lradsc = masek->roundND(lradius*scaling);
    uradsc = masek->roundND(uradius*scaling);
    rd = masek->roundND(uradius*scaling - lradius*scaling);
   
    //printf("%d %d %d\n", lradsc, uradsc, rd);

//% generate the edge image

    gradient = (Masek::filter *)malloc(sizeof(Masek::filter));
    orND = (Masek::filter *)malloc(sizeof(Masek::filter));

    image = masek->canny(image, sigma, scaling, vert, horz, gradient, orND);
    

    /*fin = fopen("or.txt", "w");
for (i = 0; i<or->hsize[0]; i++)
for (j = 0; j<or->hsize[1]; j++)
        fprintf(fin, "%d %d %f\n", i+1, j+1, or->data[i*or->hsize[1]+j]);
fclose(fin);

fin = fopen("gradient.txt", "w");
for (i = 0; i<gradient->hsize[0]; i++)
for (j = 0; j<gradient->hsize[1]; j++)
        fprintf(fin, "%d %d %f\n", i+1, j+1, gradient->data[i*gradient->hsize[1]+j]);

fclose(fin);
*/
    I3 = masek->adjgamma(gradient, 1.9);
    

/*	fin = fopen("I3.txt", "w");
for (i = 0; i<I3->hsize[0]; i++)
for (j = 0; j<I3->hsize[1]; j++)
        fprintf(fin, "%d %d %f\n", i+1, j+1, I3->data[i*I3->hsize[1]+j]);

fclose(fin);
*/

    I4 = masek->nonmaxsup(I3, orND, 1.5);
    

/*fin = fopen("I4.txt", "w");
for (i = 0; i<I4->hsize[0]; i++)
for (j = 0; j<I4->hsize[1]; j++)
        fprintf(fin, "%d %d %f\n", i+1, j+1, I4->data[i*I4->hsize[1]+j]);

fclose(fin);
*/
    edgeimage = masek->hysthresh(I4, hithres, lowthres);
    
    count = 0;
/*	fin = fopen("edge.txt", "w");
for (i = 0; i<edgeimage->hsize[0]; i++)
    for (j = 0; j<edgeimage->hsize[1]; j++)
    if (edgeimage->data[i*edgeimage->hsize[1]+j]==1)
    {
        fprintf(fin, "%d %d\n", i+1, j+1);
        count++;
    }
    fclose(fin);
*/

//% perform the circular Hough transform

//%h = houghcircle(edgeimage, lradsc, uradsc);


    h = masek->houghcircle(edgeimage, lradsc, uradsc);
    

    maxtotal = 0;
    count = 0;

    //% find the maximum in the Hough space, and hence
    //% the parameters of the circle
    for (k = 0; k<rd; k++)
    {
        for (i=0; i<edgeimage->hsize[1]; i++)
            for (j=0; j<edgeimage->hsize[0]; j++)
            {
                cur = k*edgeimage->hsize[1]*edgeimage->hsize[0]+j*edgeimage->hsize[1]+i;
                if (h[cur] > maxtotal)
                {
                    maxtotal = h[cur];
                    count = cur;
                }
            }
    }

//printf("maxtotal is %f\n", maxtotal);
    tmpi = count/(edgeimage->hsize[0]*edgeimage->hsize[1]);
/*printf("tmpi is %d\n", tmpi);
printf("lradsc is %d\n", lradsc);
printf("scaling is %.20f\n", scaling);
*/
    r = (((lradsc+tmpi+1) / scaling))+AdjPrecision;
    tmpi = count-tmpi*(edgeimage->hsize[0]*edgeimage->hsize[1]);

    row = tmpi/edgeimage->hsize[1];
    col = tmpi-row*edgeimage->hsize[1];
    row++;
    col++;

    //printf("row is %d, scaling is %f, newone is %f", row, scaling, row/scaling);
    row = (int)(row / scaling + AdjPrecision);// % returns only first max value
    col = (int)(col / scaling + AdjPrecision);

    *_row = row;
    *_col = col;
    *_r = r;

    free(gradient->data);
    free(gradient);
    free(orND->data);
    free(orND);

    free(h);
   // delete masek;
}


int MasekLee::loadtemplate(const char *file, Masek::MAT_DATA ** m_data)
{
    FILE *f;
    int offset;
    char description[116];
    unsigned char tmp[100];
    char tmp_char[100];
    void *tmpbuffer;

    int reverse;
    int small = 0;
    int len;
    int complex, logical, global;
    int row, col, cur;

    int namelen;
    int type, bytes, next;
    int i;

    int ndir;

    f = fopen(file, "r");
    if (f == NULL)
    {
        printf("Error opening file %s\n", file);
        return(0);
    }

    //header
    fread(description, 1, 116, f);
    fread(tmp_char, 1, 8, f);
    offset = atoi(tmp_char);

    //version
    fread(tmp, 1, 2, f);

    //endian indicator
    fread(tmp, 1, 2, f);
    if (tmp[0] =='I')
        reverse = 1;
    else
        reverse = 0;

    ndir = 0;
    *m_data = (Masek::MAT_DATA*)malloc (sizeof(Masek::MAT_DATA)*2);
    while (ndir<2)
    {
        //data element
        //data type, bytes
        fread(tmp, 1, 8, f);
        gettypebytes(tmp, reverse, &type, &len, &next);

        if (type!=miMATRIX)
        {
            printf("error: only support array type\n");
            return ndir;
        }

        if (next<8)
            fseek(f, -4, SEEK_CUR);

        //array type
        cur = 0;

        //array flags
        fread(tmp, 1, 8,f);
        gettypebytes(tmp, reverse, &type, &bytes, &next);
        if (type!=miUINT32)
        {
            printf("error: should be miUINT32\n");
            return ndir;
        }

        if (bytes!=8)
        {
            printf("error: should be 8\n");
            return ndir;
        }
        cur+=next;

        //to be modified
        fread(tmp, 1, 8,f);

        //complex?, global?, logical?
        if ((tmp[1]&8)>0)
            complex=1;
        else
            complex=0;

        if ((tmp[1]&4)>0)
            global=1;
        else
            global=0;

        if ((tmp[1]&2)>0)
            logical=1;
        else
            logical=0;

        cur+=8;

        //dimensions array
        fread(tmp, 1, 8, f);
        gettypebytes(tmp, reverse, &type, &bytes, &next);
        if (type!=miINT32)
        {
            printf("error: should be miINT32\n");
            return ndir;
        }

        if (bytes!=8)
        {
            printf("error: should be 8\n");
            return ndir;
        }
        cur+=next;
        if (next<8)
            fseek(f, -4, SEEK_CUR);


        fread(tmp, 1, 8, f);
        row = (*m_data)[ndir].dim[0] = getvalue(tmp, 4, reverse);
        col = (*m_data)[ndir].dim[1] = getvalue(tmp+4, 4, reverse);
        cur+=8;

        fread(tmp, 1, 8, f);
        gettypebytes(tmp, reverse, &type, &namelen, &next);
        if (type!=miINT8)
        {
            printf("error: should be miINT8\n");
            return ndir;
        }

        cur+=next;
        if (next<8)
        {
            fseek(f, -4, SEEK_CUR);
            small = 1;
        }
        else
            small=0;

        fread((*m_data)[ndir].name, 1, namelen, f);
        /*printf("name is %s\n", (*m_data)[ndir].name);*/

        switch(ndir)
        {
        case 0:
            if (strncmp("mask_hd", (*m_data)[ndir].name, namelen)!=0)
            {
                printf("parameter name error\n");
                return ndir;
            }
            break;
        case 1:
            if (strncmp("template_hd", (*m_data)[ndir].name, namelen)!=0)
            {
                printf("parameter name error\n");
                return ndir;
            }
            break;
        default:
            break;

        }

        cur+=namelen;
        if (namelen%8 && small==0)
        {
            cur+=8-namelen%8;
            fread(tmp, 1, 8-namelen%8, f);
        }

        fread(tmp, 1, 8, f);
        gettypebytes(tmp, reverse, &type, &bytes, &next);

        cur+=next;


        (*m_data)[ndir].ptr = (void*) malloc(sizeof(int)*(*m_data)[ndir].dim[0]*(*m_data)[ndir].dim[1]);
        if (next<8)
        {
            if (row*col!=bytes)
            {
                printf("bytes type error\n");
                return (ndir);
            }
            for (i = 0; i<row*col; i++)
                ((int*)(*m_data)[ndir].ptr)[i] = (int)((unsigned char*)tmp)[next+i];
            cur+=4;
            fread(tmp, 1, len-cur, f);
            ndir++;
            continue;
        }

        tmpbuffer = (unsigned char*)malloc(bytes);
        fread(tmpbuffer, 1, bytes, f);
        if (row*col*4==bytes)
        {
            for (i = 0; i<col*row; i++)
            {
                ((int*)(*m_data)[ndir].ptr)[i] = (int)((int*)tmpbuffer)[i];
            }
        }
        else if (row*col*2==bytes)
        {
            for (i = 0; i<col*row; i++)
            {
                ((int*)(*m_data)[ndir].ptr)[i] = (int)((short*)tmpbuffer)[i];
            }
        }
        else if (row*col == bytes)
        {
            for (i = 0; i<col*row; i++)
            {
                ((int*)(*m_data)[ndir].ptr)[i] = (int)((unsigned char*)tmpbuffer)[i];
            }
        }
        else
        {
            printf("bytes type error\n");
            return (ndir);
        }

        free (tmpbuffer);


        cur+=bytes;


        fread(tmp, 1, len-cur, f);

        ndir++;
    }
    fclose(f);

    return ndir;

}

int MasekLee::savetemplate(const char *file, Masek::MAT_DATA *m_data)
{
    FILE *f;
    char description[116];
    char tmp[100];
    unsigned char tmp_uc[100];

    int small = 0;
    int len;
    int row, col, cur;
    int namelen;
    int bytes;
    int i;
    int ndir;
    int extra1, extra2;


    f = fopen(file, "w");
    if (f == NULL)
    {
        printf("Error opening file %s\n", file);
        return(0);
    }

    //header
    sprintf(description, "MATLAB 5.0 MAT-file");
    fwrite(description, 1, 116, f);

    strcpy(tmp, "        ");
    fwrite(tmp, 1, 8, f);

    //version
    tmp[0] = 0;
    tmp[1] = 1;
    fwrite(tmp, 1, 2, f);

    //endian indicator, reverse order
    tmp[0] = 'I';
    tmp[1] = 'M';
    fwrite(tmp, 1, 2, f);

    ndir = 0;

    while (ndir<2)
    {
        row = m_data[ndir].dim[0];
        col = m_data[ndir].dim[1];
        bytes = row*col;
        switch(ndir)
        {
         case 0:
            namelen = 4;
            small = 1;
            break;
         case 1:
            namelen = 8;
            break;

        }

        if (namelen%8 && small==0)
        {
            extra1 = 8-namelen%8;
        }
        else
            extra1 = 0;

        if (bytes%8)
            extra2 = 8-bytes%8;
        else
            extra2 = 0;

        //data element
        //data type, bytes
        tmp[0] = miMATRIX;
        tmp[1] = 0;
        tmp[2] = 0;
        tmp[3] = 0;
        fwrite(tmp, 1, 4, f);

        len = 6*8+namelen+extra1+bytes+extra2;

        if (small==1)
            len -= 4;

        setvalue(tmp_uc,len, 4);
        fwrite(tmp_uc, 1, 4, f);

        //array type
        cur = 0;


        //array flags
        setvalue(tmp_uc, miUINT32, 4);
        setvalue(tmp_uc+4, 8, 4);
        fwrite(tmp_uc, 1, 8,f);

        cur+=8;

        //to be modified
        //if (ndir == 0)
            setvalue(tmp_uc, mxDOUBLE_CLASS, 8);
        //else
        //	setvalue(tmp, mxINT32_CLASS, 8);
        fwrite(tmp_uc, 1, 8,f);
        cur+=8;

        //dimensions array
        setvalue(tmp_uc, miINT32, 4);
        setvalue(tmp_uc+4, 8, 4);
        fwrite(tmp_uc, 1, 8, f);
        cur+=8;


        setvalue(tmp_uc,row, 4);
        setvalue(tmp_uc+4, col, 4);
        fwrite(tmp_uc, 1, 8, f);
        cur+=8;

        //name
        if (ndir == 1)
        {
            setvalue(tmp_uc, miINT8, 4);
            setvalue(tmp_uc+4, namelen, 4);
            fwrite(tmp_uc, 1, 8, f);
            cur+=8;

            fwrite(m_data[ndir].name, 1, namelen, f);
            cur+=namelen;
        }
        else
        {
            setvalue(tmp_uc, miINT8, 2);
            setvalue(tmp_uc+2, namelen, 2);
            fwrite(tmp_uc, 1, 4, f);
            cur+=4;

            fwrite(m_data[ndir].name, 1, namelen, f);
            cur+=namelen;
        }


        if (extra1>0)
        {
            cur+=extra1;
            for (i = 0; i<extra1; i++)
                tmp[i] = 0;
            fwrite(tmp, 1, extra1, f);

        }

        setvalue(tmp_uc, miUINT8, 4);
        setvalue(tmp_uc+4, bytes, 4);
        fwrite(tmp_uc, 1, 8, f);
        cur+=8;

        cur += bytes;
        fwrite(m_data[ndir].ptr, 1, row*col, f);
        if (extra2)
        {
            for (i = 0; i<extra2; i++)
                tmp[i] = 0;
            fwrite(tmp, 1, extra2, f);
            cur+=extra2;
        }
        ndir++;
    }

    fclose(f);

    return ndir;

}

void MasekLee::linescoords(double *lines, int row, int col, int *x, int *y)
{
    int i;

    double xd, yd;

    for (i = 0; i<col; i++)
    {
        xd = i+1;
        yd = (-lines[2]-lines[0]*xd)/lines[1];
        if (yd>row)
            yd = row;
        else if (yd<1)
            yd = 1;

        x[i] = (int)(xd+AdjPrecision);
        y[i] = (int)(yd+AdjPrecision);

    }


}
