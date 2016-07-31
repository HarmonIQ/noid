/**************************************************
*  Function: Load/Save Templates
*  Author:
*  Xiaomei Liu
*  xliu5@cse.nd.edu
*  Computer Vision Research Laboratory
*  Department of Computer Science & Engineering
*  U. of Notre Dame
***************************************************/

#include <string.h>
#include "Masek.h"


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

int getvalue(unsigned char *str, int len, int reverse)
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

void setvalue(unsigned char *str, int value, int len)
{
	int i;

	
	for (i = 0; i<len; i++)
	{
		str[i] = value%256;
		value = value/256;
	}

}

void gettypebytes(unsigned char *str, int reverse, int *type, int *bytes, int *next)
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

int Masek::loadsegmentation(const char *file, Masek::MAT_DATA** m_data) 
{
	FILE *f;
	int offset;
	char description[116];
	char tmp[100];
	unsigned char tmp_uc[100];
	void *tmpbuffer;

	int reverse;

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
	fread(tmp, 1, 8, f);
	offset = atoi(tmp);

	//version
	fread(tmp, 1, 2, f);

	//endian indicator
	fread(tmp, 1, 2, f);
	if (tmp[0] =='I')
		reverse = 1;
	else 
		reverse = 0;


	ndir = 0;
	*m_data = (MAT_DATA*)malloc (sizeof(MAT_DATA)*3);
	while (ndir<3)
	{
		//data element
		//data type, bytes
		fread(tmp_uc, 1, 8, f);
		gettypebytes(tmp_uc, reverse, &type, &len, &next);

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
		fread(tmp_uc, 1, 8,f);
		gettypebytes(tmp_uc, reverse, &type, &bytes, &next);
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
		fread(tmp_uc, 1, 8, f);
		gettypebytes(tmp_uc, reverse, &type, &bytes, &next);
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


		fread(tmp_uc, 1, 8, f);
		row = (*m_data)[ndir].dim[0] = getvalue(tmp_uc, 4, reverse);
		col = (*m_data)[ndir].dim[1] = getvalue(tmp_uc+4, 4, reverse);
		cur+=8;

		fread(tmp_uc, 1, 8, f);
		gettypebytes(tmp_uc, reverse, &type, &namelen, &next);
		if (type!=miINT8)
		{
			printf("error: should be miINT8\n");
			return ndir;
		}
	
		cur+=next;
		if (next<8)
			fseek(f, -4, SEEK_CUR);

		fread((*m_data)[ndir].name, 1, namelen, f);
		

		switch(ndir)
		{
		case 0:
			if (strncmp("imagewithnoise", (*m_data)[ndir].name, namelen)!=0)
			{
				printf("parameter name error\n");
				return ndir;
			}
			break;
		case 1:
			if (strncmp("circlepupil", (*m_data)[ndir].name, namelen)!=0)
			{
				printf("parameter name error\n");
				return ndir;
			}
			break;
		case 2:
			if (strncmp("circleiris", (*m_data)[ndir].name, namelen)!=0)
			{
				printf("parameter name error\n");
				return ndir;
			}
			break;
		default:
			break;

		}

		cur+=namelen;
		if (namelen%8)
		{
			cur+=8-namelen%8;
			fread(tmp, 1, 8-namelen%8, f);
		}

		fread(tmp_uc, 1, 8, f);
		gettypebytes(tmp_uc, reverse, &type, &bytes, &next);
		
		cur+=next;
		if (ndir == 0)
		{
			if (row*col*8!=bytes)
			{
				printf("bytes type error\n");
				return (ndir);
			}

			tmpbuffer = (void *)malloc(bytes);
			
			(*m_data)[ndir].ptr = (void*) malloc(sizeof(double)*row*col );
			fread(tmpbuffer, 1, bytes, f);
			for (i = 0; i<row*col; i++)
				((double*)((*m_data)[0]).ptr)[i] =((double*)tmpbuffer)[i];
			free(tmpbuffer);
		}
		else 
		{

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
			else
			{
				printf("bytes type error\n");
				return (ndir);
			}
			
			free (tmpbuffer);

		}
				
		cur+=bytes;
				
		fread(tmp, 1, len-cur, f);
				
		ndir++;
	}

	fread(tmp, 1, 100, f);
  	fclose(f);	  
	
	return ndir;
	
}

int Masek::savesegmentation(const char *file, Masek::MAT_DATA *m_data) 
{
	FILE *f;

	char description[116];
	char tmp[100];
	unsigned char tmp_uc[100]; 


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
	
	while (ndir<3)
	{
		row = m_data[ndir].dim[0];
		col = m_data[ndir].dim[1];
		
		switch(ndir)
		{
		case 0:
			namelen = 14;
			bytes = row*col*8;
			
			break;
         case 1:
			namelen = 11;
			bytes = row*col*2;
			break;
		case 2:
			namelen = 10;
			bytes = row*col*2;
			break;
		}

		if (namelen%8)
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
		setvalue(tmp_uc, miINT8, 4);
		
		
		
		setvalue(tmp_uc+4, namelen, 4);
		fwrite(tmp_uc, 1, 8, f);
		cur+=8;

		fwrite(m_data[ndir].name, 1, namelen, f);
		cur+=namelen;
			
		if (extra1>0)
		{
			cur+=extra1;
			for (i = 0; i<extra1; i++)
				tmp[i] = 0;
			fwrite(tmp, 1, extra1, f);

		}
		
		if (ndir == 0)
		{
		
			setvalue(tmp_uc, miDOUBLE, 4);
			setvalue(tmp_uc+4, bytes, 4);
			fwrite(tmp_uc, 1, 8, f);
			cur+=8;
			fwrite(m_data[ndir].ptr, 1, bytes, f);
			cur+=bytes;
		}
		else 
		{
			setvalue(tmp_uc, miUINT16, 4);
			setvalue(tmp_uc+4, bytes, 4);
			fwrite(tmp_uc, 1, 8, f);
			cur+=8;

			cur += bytes;
			fwrite(m_data[ndir].ptr, 2, row*col, f);
			if (extra2)
			{
				for (i = 0; i<extra2; i++)
					tmp[i] = 0;
				fwrite(tmp, 1, extra2, f);
				cur+=extra2;
			}

			/*(*m_data)[ndir].ptr = (void*) malloc(sizeof(int)*(*m_data)[ndir].dim[0]*(*m_data)[ndir].dim[1]);
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
			else
			{
				printf("bytes type error\n");
				return (ndir);
			}
			
			free (tmpbuffer);
*/
		}
		
	
		
		ndir++;
	}

  	fclose(f);	  
	
	return ndir;
	
}


int Masek::loadtemplate(const char *file, Masek::MAT_DATA ** m_data) 
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
	*m_data = (MAT_DATA*)malloc (sizeof(MAT_DATA)*2);
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

int Masek::savetemplate(const char *file, Masek::MAT_DATA *m_data) 
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
