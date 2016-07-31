/**************************************************
*  This is a C translation from Masek's matlab code
*  Author:
*  Xiaomei Liu
*  xliu5@cse.nd.edu
*  Computer Vision Research Laboratory
*  Department of Computer Science & Engineering
*  U. of Notre Dame
***************************************************/


/*% gethammingdistance - returns the Hamming Distance between two iris templates
% incorporates noise masks, so noise bits are not used for
% calculating the HD
%
% Usage: 
% [template, mask] = createiristemplate(eyeimage_filename)
%
% Arguments:
%	template1       - first template
%   mask1           - corresponding noise mask
%   template2       - second template
%   mask2           - corresponding noise mask
%   scales          - the number of filters used to encode the templates,
%                     needed for shifting.
%
% Output:
%   hd              - the Hamming distance as a ratio
%
% Author: 
% Libor Masek
% masekl01@csse.uwa.edu.au
% School of Computer Science & Software Engineering
% The University of Western Australia
% November 2003*/
#include <stdlib.h>
#include <stdio.h>
#include "Masek.h"

//extern void shiftbits(int *templates, int width, int height, int noshifts,int nscales, int *templatenew);
double Masek::gethammingdistance(int *template1, int *mask1, int *template2, int *mask2, int scales, int width, int height)
{
	double hd, shifts;
	int bitsdiff, nummaskbits, totalbits;
	int *template1s, /**template2s, */*mask1s, *mask, *C;
	double hd1;
	int i;
	
//	FILE *fid;
	hd = -1;
	//LEE: need to work on here
	hd1 = -1;//LEE:added

/*% shift template left and right, use the lowest Hamming distance
%for shifts=-8:8*/
	
	template1s = (int*) malloc(sizeof(int)*width*height);
	//template2s = (int*) malloc(sizeof(int)*width*height);
	mask1s = (int*) malloc(sizeof(int)*width*height);
//	mask2s = (int*) malloc(sizeof(int)*width*height);
	mask = (int*) malloc(sizeof(int)*width*height);
	C =  (int*) malloc(sizeof(int)*width*height);

	//for (shifts=-10; shifts<=10; shifts++)
  for (shifts=-10; shifts<=10; shifts++)
	{
		shiftbits(template1, width, height, (int)shifts,scales, template1s);
		shiftbits(mask1, width, height, (int)shifts,scales, mask1s);

		/*if (shifts == -10)
		{
			fid = fopen("template1_o.txt", "w");
			for (i = 0; i<height; i++)
				for (j = 0; j<width; j++)
					fprintf(fid, "%d %d %d\n", i+1, j+1, template1[i*width+j]);
			fclose(fid);

			fid = fopen("template1.txt", "w");
			for (i = 0; i<height; i++)
				for (j = 0; j<width; j++)
					fprintf(fid, "%d %d %d\n", i+1, j+1, template1s[i*width+j]);
			fclose(fid);

			fid = fopen("template2.txt", "w");
			for (i = 0; i<height; i++)
				for (j = 0; j<width; j++)
					fprintf(fid, "%d %d %d\n", i+1, j+1, template2[i*width+j]);
			fclose(fid);

			fid = fopen("mask1_o.txt", "w");
			for (i = 0; i<height; i++)
				for (j = 0; j<width; j++)
					fprintf(fid, "%d %d %d\n", i+1, j+1, mask1[i*width+j]);
			fclose(fid);

			fid = fopen("mask1.txt", "w");
			for (i = 0; i<height; i++)
				for (j = 0; j<width; j++)
					fprintf(fid, "%d %d %d\n", i+1, j+1, mask1s[i*width+j]);
			fclose(fid);

			fid = fopen("mask2.txt", "w");
			for (i = 0; i<height; i++)
				for (j = 0; j<width; j++)
					fprintf(fid, "%d %d %d\n", i+1, j+1, mask2[i*width+j]);
			fclose(fid);
		}
		*/
		nummaskbits = 0;

		for (i = 0; i<width*height; i++)
		{
			mask[i] = mask1s[i] | mask2[i];
			if (mask[i] == 1)
				nummaskbits++;			
		}
    
		    
		//printf("width is %d height is %d nummaskbits is %d\n", width, height, nummaskbits);
		totalbits = width*height - nummaskbits;    
		
		bitsdiff = 0;
		for (i = 0; i<width*height; i++)
		{
      //LEE: one template is empty or too small			
			C[i] = template1s[i] ^ template2[i];
			C[i] = C[i] & (1-mask[i]);
			if (C[i] == 1)
				bitsdiff++;
			
		}  
		
		if (totalbits == 0)        
			hd = (double)-1; //LEE added: double       
		else		        
			hd1 = (double)bitsdiff / totalbits;
					        
        
        if  ((hd1 < hd) || (hd == -1))
		{
            hd = hd1;
		}
            
        
	}
	free (template1s);
	//free (template2s);
	free(mask1s);
	//free(mask2s);
	free(mask);
	free(C);

	return hd;
}
