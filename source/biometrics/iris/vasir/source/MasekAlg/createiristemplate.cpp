/**************************************************
*  This is a C translation from Masek's matlab code
*  Author:
*  Xiaomei Liu
*  xliu5@cse.nd.edu
*  Computer Vision Research Laboratory
*  Department of Computer Science & Engineering
*  U. of Notre Dame
***************************************************/


/*% createiristemplate - generates a biometric template from an iris in
% an eye image.
%
% Usage: 
% [template, mask] = createiristemplate(eyeimage_filename)
%
% Arguments:
%	eyeimage_filename   - the file name of the eye image
%
% Output:
%	template		    - the binary iris biometric template
%	mask			    - the binary iris noise mask
%
% Author: 
% Libor Masek
% masekl01@csse.uwa.edu.au
% School of Computer Science & Software Engineering
% The University of Western Australia
% November 2003

function [template, mask] = createiristemplate(eyeimage_filename)*/

#include <string.h>
#include <math.h>
#include <float.h> // Lee: Added
#include "Masek.h"


//LEE:
//#include "cv.h"
//#include "highgui.h"
//#include "cxcore.h"
//LEE add mode
void Masek::createiristemplate(char * eyeimage_filename, int nscales, int **_template, int **mask, int *width, int *height)
{
	int radial_res, angular_res, minWaveLength, mult;
  double sigmaOnf;
	IMAGE * eyeimage;
	char *savefile;
	IMAGE imagewithnoise2, imagewithcircles, polararrayimg, noisearrayimg;
	filter imagewithnoise;
	int circlepupil[3], circleiris[3];
	MAT_DATA *m_data;
	int i, j;
    //int ndir;
	filter polar_array;
	IMAGE noise_array;
	//char fname[50];
  char fname[200];//LEE
	int *x, *y;
	int len;


	/*% path for writing diagnostic images
	global DIAGPATH
	%DIAGPATH = 'diagnostics';

	%normalisation parameters*/
	radial_res = 20;
	angular_res = 240;

  //radial_res = 10;
	//angular_res = 20;
	/*% with these settings a 9600 bit iris template is
	% created

	%feature encoding parameters*/
	
	minWaveLength=18;
	mult=1; /*% not applicable if using nscales = 1*/
	sigmaOnf=0.5;

  //eyeimage_filename="C:/Iris2008/Develop/YooMasek/bin/eyeImage6.bmp";
	eyeimage = imread(eyeimage_filename); 

	savefile = (char*)malloc(sizeof(char)*(strlen(eyeimage_filename)+20));
	strcpy(savefile, eyeimage_filename);
    strcat(savefile, "-houghpara.mat");
	//[stat,mess]=fileattrib(savefile);
#if 0	
  stat = fopen(savefile, "r");

  //Need
	if (stat != NULL)
	//% if this file has been processed before
  // % then load the circle parameters and
  // % noise information for that file.
	{
		fclose(stat);
		ndir = loadsegmentation(savefile, &m_data);
		
		//get imagewithnoise
		imagewithnoise.hsize[0] = m_data[0].dim[0];
		imagewithnoise.hsize[1] = m_data[0].dim[1];
		
		imagewithnoise.data = (double*) malloc(sizeof(double)*m_data[0].dim[0]*m_data[0].dim[1]);

		for (i = 0; i<m_data[0].dim[0]; i++)
			for (j=0; j<m_data[0].dim[1]; j++)
			{
				imagewithnoise.data[i*m_data[0].dim[1]+j] =((double*)m_data[0].ptr)[j*m_data[0].dim[0]+i];
				//if (i*m_data[0].dim[1]+j<20)
				//	printf("%f\n", imagewithnoise.data[i*m_data[0].dim[1]+j]);
			}


		//get circlepupil
		for (i = 0; i<m_data[1].dim[0]*m_data[1].dim[1]; i++)
			circlepupil[i] =(int)((int*)m_data[1].ptr)[i];
		
		//get circleiris
		for (i = 0; i<m_data[2].dim[0]*m_data[2].dim[1]; i++)
			circleiris[i] =(int)((int*)m_data[2].ptr)[i];

		for (i = 0; i<ndir; i++)
			free(m_data[i].ptr);

		free(m_data);


	}
    else
	{
#endif
    /*% if this file has not been processed before
    % then perform automatic segmentation and
    % save the results to a file*/
    
	//	[circleiris circlepupil imagewithnoise] = segmentiris(eyeimage);
		imagewithnoise.hsize[0] = eyeimage->hsize[0];
		imagewithnoise.hsize[1] = eyeimage->hsize[1];
		imagewithnoise.data = (double*)malloc(sizeof(double)*eyeimage->hsize[0]*eyeimage->hsize[1]);
    
     segmentiris(eyeimage, circleiris, circlepupil, imagewithnoise.data, 1, 0, 0);
   

		//testing saving

		m_data = (MAT_DATA*)malloc (sizeof(MAT_DATA)*3);
		strcpy(m_data[0].name, "imagewithnoise");
		m_data[0].dim[0] = imagewithnoise.hsize[0];
		m_data[0].dim[1] = imagewithnoise.hsize[1];
		m_data[0].ptr = malloc(sizeof(double)*m_data[0].dim[0]*m_data[0].dim[1]);
		
		for (i = 0; i<m_data[0].dim[0]; i++)
			for (j=0; j<m_data[0].dim[1]; j++)
			{
				((double*)m_data[0].ptr)[j*m_data[0].dim[0]+i] = imagewithnoise.data[i*m_data[0].dim[1]+j];
			}

		strcpy(m_data[1].name, "circlepupil");
		m_data[1].dim[0] = 1;
		m_data[1].dim[1] = 3;
		m_data[1].ptr = malloc(sizeof(short)*m_data[1].dim[0]*m_data[1].dim[1]);
		for (i = 0; i<m_data[1].dim[0]*m_data[1].dim[1]; i++)
			((short*)m_data[1].ptr)[i] = circlepupil[i];
			
		strcpy(m_data[2].name, "circleiris");
		m_data[2].dim[0] = 1;
		m_data[2].dim[1] = 3;
		m_data[2].ptr = malloc(sizeof(short)*m_data[2].dim[0]*m_data[2].dim[1]);
		for (i = 0; i<m_data[2].dim[0]*m_data[2].dim[1]; i++)
			((short*)m_data[2].ptr)[i] = circleiris[i];

		savesegmentation(savefile, m_data);

		for (i = 0; i<3; i++)
			free(m_data[i].ptr);

		free(m_data);

		//save(savefile,'circleiris','circlepupil','imagewithnoise');*/
		
//	}

//	% WRITE NOISE IMAGE
//	%

	imagewithnoise2.hsize[0] = imagewithnoise.hsize[0];
	imagewithnoise2.hsize[1] = imagewithnoise.hsize[1];
	imagewithnoise2.data = (unsigned char*)malloc(imagewithnoise.hsize[1]*imagewithnoise.hsize[0]);
	for (i = 0; i<imagewithnoise.hsize[0]*imagewithnoise.hsize[1]; i++)
	{
        if (isnan(imagewithnoise.data[i])) // Lee: renamed from 'isnan'
			imagewithnoise2.data[i] = 0;
		else
			imagewithnoise2.data[i] = eyeimage->data[i];
	}

	imagewithcircles.hsize[0] = imagewithnoise.hsize[0];
	imagewithcircles.hsize[1] = imagewithnoise.hsize[1];
	imagewithcircles.data = (unsigned char*)malloc(imagewithnoise.hsize[1]*imagewithnoise.hsize[0]);
	memcpy(imagewithcircles.data, eyeimage->data, imagewithnoise.hsize[1]*imagewithnoise.hsize[0]);
	
	

	//get pixel coords for circle around iris
	len = circlecoords(circleiris[1], circleiris[0], circleiris[2], (int*)(&imagewithcircles.hsize), -1, &x, &y);
	for (i = 0; i<len; i++)
	{
		imagewithnoise2.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
		imagewithcircles.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
	}
	free (x);
	free (y);
	//%get pixel coords for circle around pupil
	len = circlecoords(circlepupil[1], circlepupil[0], circlepupil[2], (int*)(&imagewithcircles.hsize), -1, &x, &y);
	
	
	for (i = 0; i<len; i++)
	{
		imagewithnoise2.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
		imagewithcircles.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
	}
	free (x);
	free (y);

	sprintf(fname, "%s-noise.bmp", eyeimage_filename);
	imwrite(fname, &imagewithnoise2);
	sprintf(fname, "%s-segmented.bmp", eyeimage_filename);
	imwrite(fname, &imagewithcircles);

	free(imagewithnoise2.data);
	free(imagewithcircles.data);

	//% perform normalisation
#if 1
	normaliseiris(&imagewithnoise, circleiris[1], circleiris[0], circleiris[2], circlepupil[1], circlepupil[0], circlepupil[2],eyeimage_filename, radial_res, angular_res, &polar_array, &noise_array);

	polararrayimg.hsize[0] = polar_array.hsize[0];
	polararrayimg.hsize[1] = polar_array.hsize[1];

	polararrayimg.data = (unsigned char*)malloc(polar_array.hsize[0]*polar_array.hsize[1]);
	for (i=0; i<polar_array.hsize[0]*polar_array.hsize[1]; i++)
		polararrayimg.data[i] = (unsigned char)(255*polar_array.data[i]);

	sprintf(fname, "%s-polar.bmp", eyeimage_filename);
	imwrite(fname, &polararrayimg);
	free(polararrayimg.data);

	noisearrayimg.hsize[0] = noise_array.hsize[0];
	noisearrayimg.hsize[1] = noise_array.hsize[1];

	noisearrayimg.data = (unsigned char*)malloc(noise_array.hsize[0]*noise_array.hsize[1]);
	for (i=0; i<noise_array.hsize[0]*noise_array.hsize[1]; i++)
		noisearrayimg.data[i] = 255*noise_array.data[i];

	sprintf(fname, "%s-polarnoise.bmp", eyeimage_filename);
	imwrite(fname, &noisearrayimg);
	free(noisearrayimg.data);
	

	/*% WRITE NORMALISED PATTERN, AND NOISE PATTERN
	%w = cd;
	%cd(DIAGPATH);
	imwrite(polar_array,[eyeimage_filename,'-polar.jpg'],'jpg');
	imwrite(noise_array,[eyeimage_filename,'-polarnoise.jpg'],'jpg');
	%cd(w);

	% perform feature encoding*/
	encode(&polar_array, &noise_array, nscales, minWaveLength, mult, sigmaOnf, _template, mask, width, height); 

	free(polar_array.data);
	free(noise_array.data);
#endif
  free(eyeimage->data);
	free(eyeimage);
	free(imagewithnoise.data);
	free(savefile);
}
