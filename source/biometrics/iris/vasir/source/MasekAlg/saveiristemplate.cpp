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
#include <float.h> // Lee: added
#include "Masek.h"


//#include "global.h"
//#include "createiristemplate.h"
//#include "imread.h"
//IMAGE* imread(const char *name);
/*
extern void segmentiris(IMAGE *eyeimage, int *circleiris, int *circlepupil, double *imagewithnoise, int eyelidon, int highlighton, int highlightvalue);
extern int segmentiris_gt(IMAGE *eyeimage, char *gndfilename, int *circleiris, int *circlepupil, double *imagewithnoise, int eyelidon, int highlighton, int highlightvalue);
extern int segmentiris_iridian(IMAGE *eyeimage, char *gndfilename, int *circleiris, int *circlepupil, double *imagewithnoise, int eyelidon, int highlighton, int highlightvalue);
extern void encode(filter *polar_array, IMAGE* noise_array, int nscales, int minWaveLength, int mult, double sigmaOnf, int **_template, int **mask, int *width, int *height);
extern int circlecoords(double x0, double y0, double r, int *imgsize, double _nsides, int **x, int **y);
extern int loadsegmentation(const char *file, MAT_DATA ** m_data) ;
extern int savesegmentation(const char *file, MAT_DATA * m_data);
extern void normaliseiris(filter *image, int xiris, int yiris, int riris, int xpupil, int ypupil, int rpupil,char *eyeimage_filename, int radpixels, int angulardiv, filter *polar_array, IMAGE *polar_noise);
*/

void Masek::saveiristemplate(const char *filedir, const char *templatedir, const char *segdir, const char * _eyeimage_filename, int nscales, int **_template, int **mask, int *width, int *height, int mode, int eyelidon, int highlighton, int highlightvalue)
{
	int radial_res, angular_res, minWaveLength, mult;
    double sigmaOnf;
	IMAGE * eyeimage;
	char savefile[200];//LEE:, savefileold[200];
//	FILE *stat;//LEE:
	IMAGE imagewithnoise2, imagewithcircles, polararrayimg, noisearrayimg;
	filter imagewithnoise;
	int circlepupil[3], circleiris[3];
//	MAT_DATA *m_data; //LEE:

	int i; //LEE:, j;
//	int ndir; //LEE:
	filter polar_array;
	IMAGE noise_array;
	char eyeimage_filename[100], gnd_filename[100], id[25];
	char fname[50];
	int *x, *y;
	int len;
	//char day[4]; //LEE:, number[4];
	int namelen;


	/*% path for writing diagnostic images
	global DIAGPATH
	%DIAGPATH = 'diagnostics';

	%normalisation parameters*/
	radial_res = 20;
	angular_res = 240;
	/*% with these settings a 9600 bit iris template is
	% created

	%feature encoding parameters*/
	
	minWaveLength=18;
	mult=1; /*% not applicable if using nscales = 1*/
	sigmaOnf=0.5;

	sprintf(eyeimage_filename, "%s%s", filedir, _eyeimage_filename);
	printf("eyefile is %s\n", eyeimage_filename);
	eyeimage = imread(eyeimage_filename); 

	//savefile = (char*)malloc(sizeof(char)*(strlen(eyeimage_filename)+20));
	sprintf(savefile, "%s%s-houghpara.mat", templatedir, _eyeimage_filename);

	if (mode>1)
	{
		namelen =strlen(_eyeimage_filename);
		strncpy(id, _eyeimage_filename, namelen-4);
		id[namelen-4] = '\0';

		//strncpy(day, _eyeimage_filename+5, 3);
		//day[4] = '\0';

		if (mode==2)
		{
            sprintf(gnd_filename, "%s2004-iris/%s.gnd", segdir,id);//lee
			//sprintf(gnd_filename, "%s%12s.gnd", templatedir, id);
			//sprintf(gnd_filename, "%s2004-%s-iris/%s.gnd", segdir, day,  id);
			//sprintf(gnd_filename, "correctgnd/%s.gnd", id);
	
		}
		else
			sprintf(gnd_filename, "%s%s.ir", segdir, id);
		printf("gnd_filename is %s\n", gnd_filename);

	}
	
	//[stat,mess]=fileattrib(savefile);
	/*	stat = fopen(savefile, "r");

	if (stat != NULL)
	/*% if this file has been processed before
    % then load the circle parameters and
    % noise information for that file.* /
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

/*		ndir = loadsegmentation(savefile, &m_data);

		printf("ndir is %d\n", ndir);
		//get imagewithnoise
		imagewithnoise = (unsigned char*) malloc(sizeof(unsigned char)*m_data[0].dim[0]*m_data[0].dim[0]);
		for (i = 0; i<m_data[0].dim[0]*m_data[0].dim[1]; i++)
		{
			imagewithnoise[i] =(unsigned char)((double*)m_data[0].ptr)[i];
			if (i<20)
				printf("%d\n", imagewithnoise[i]);
		}


		//get circlepupil
		circlepupil = (int *) malloc(sizeof(int)*m_data[1].dim[0]*m_data[1].dim[1]);
		for (i = 0; i<m_data[1].dim[0]*m_data[1].dim[1]; i++)
			circlepupil[i] =(int)((double*)m_data[1].ptr)[i];
		
		//get circleiris
		circleiris = (int *) malloc(sizeof(int)*m_data[2].dim[0]*m_data[2].dim[1]);
		for (i = 0; i<m_data[2].dim[0]*m_data[2].dim[1]; i++)
			circleiris[i] =(int)((double*)m_data[2].ptr)[i];
		
		for (i = 0; i<ndir; i++)
			mxDestroyArray(m_data[i].ptr);

* /
	}
    else
	{*/
    /*% if this file has not been processed before
    % then perform automatic segmentation and
    % save the results to a file*/
    
	//	[circleiris circlepupil imagewithnoise] = segmentiris(eyeimage);
		
		imagewithnoise.hsize[0] = eyeimage->hsize[0];
		imagewithnoise.hsize[1] = eyeimage->hsize[1];
		imagewithnoise.data = (double*)malloc(sizeof(double)*eyeimage->hsize[0]*eyeimage->hsize[1]);
		if (mode == 1)
			segmentiris(eyeimage, circleiris, circlepupil, imagewithnoise.data, eyelidon, highlighton, highlightvalue);
		else if (mode==2)
		{
				if (segmentiris_gt(eyeimage, gnd_filename, circleiris, circlepupil, imagewithnoise.data, eyelidon, highlighton, highlightvalue)==-1)
					return;
		}
		else
		{
				if (segmentiris_iridian(eyeimage, gnd_filename, circleiris, circlepupil, imagewithnoise.data, eyelidon, highlighton, highlightvalue)==-1)
					return;
		}

		
		/*save(savefile,'circleiris','circlepupil','imagewithnoise');*/

	/*	m_data = (MAT_DATA*)malloc (sizeof(MAT_DATA)*3);
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

		
	}*/



/*	% WRITE NOISE IMAGE
	%*/

	imagewithnoise2.hsize[0] = imagewithnoise.hsize[0];
	imagewithnoise2.hsize[1] = imagewithnoise.hsize[1];
	imagewithnoise2.data = (unsigned char*)malloc(imagewithnoise.hsize[1]*imagewithnoise.hsize[0]);
	for (i = 0; i<imagewithnoise.hsize[0]*imagewithnoise.hsize[1]; i++)
	{
        if (isnan(imagewithnoise.data[i])) // Lee: renamed
			imagewithnoise2.data[i] = 0;
		else
			imagewithnoise2.data[i] = eyeimage->data[i];
	}

	imagewithcircles.hsize[0] = imagewithnoise.hsize[0];
	imagewithcircles.hsize[1] = imagewithnoise.hsize[1];
	imagewithcircles.data = (unsigned char*)malloc(imagewithnoise.hsize[1]*imagewithnoise.hsize[0]);
	memcpy(imagewithcircles.data, eyeimage->data, imagewithnoise.hsize[1]*imagewithnoise.hsize[0]);
	
	


	//%get pixel coords for circle around iris
	len = circlecoords(circleiris[1], circleiris[0], circleiris[2], (int*)&imagewithcircles.hsize, -1, &x, &y);
	for (i = 0; i<len; i++)
	{
		imagewithnoise2.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
		imagewithcircles.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
	}
	free (x);
	free (y);

	//%get pixel coords for circle around pupil
	len = circlecoords(circlepupil[1], circlepupil[0], circlepupil[2], (int*)&imagewithcircles.hsize, -1, &x, &y);
	
	
	for (i = 0; i<len; i++)
	{
		imagewithnoise2.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
		imagewithcircles.data[y[i]*imagewithnoise2.hsize[1]+x[i]] = 255;
	}
	free (x);
	free (y);

	
	
	sprintf(fname, "%sdiagnostics/%s-noise.bmp", templatedir, _eyeimage_filename);
	
//	imwrite(fname, &imagewithnoise2);
	
	sprintf(fname, "%sdiagnostics/%s-segmented.bmp", templatedir, _eyeimage_filename);
	//imwrite(fname, &imagewithcircles);
	
	
	free(imagewithnoise2.data);
	free(imagewithcircles.data);
	
	//% perform normalisation

	normaliseiris(&imagewithnoise, circleiris[1], circleiris[0], circleiris[2], circlepupil[1], circlepupil[0], circlepupil[2],eyeimage_filename, radial_res, angular_res, &polar_array, &noise_array);


	polararrayimg.hsize[0] = polar_array.hsize[0];
	polararrayimg.hsize[1] = polar_array.hsize[1];

	polararrayimg.data = (unsigned char*)malloc(polar_array.hsize[0]*polar_array.hsize[1]);
	for (i=0; i<polar_array.hsize[0]*polar_array.hsize[1]; i++)
		polararrayimg.data[i] = (unsigned char) (255*polar_array.data[i]);

	sprintf(fname, "%sdiagnostics/%s-polar.bmp", templatedir, _eyeimage_filename);
	//sprintf(fname, "%s-polar.bmp", eyeimage_filename);
	//imwrite(fname, &polararrayimg);
	free(polararrayimg.data);

	noisearrayimg.hsize[0] = noise_array.hsize[0];
	noisearrayimg.hsize[1] = noise_array.hsize[1];

	noisearrayimg.data = (unsigned char*)malloc(noise_array.hsize[0]*noise_array.hsize[1]);
	for (i=0; i<noise_array.hsize[0]*noise_array.hsize[1]; i++)
		noisearrayimg.data[i] = 255*noise_array.data[i];

	sprintf(fname, "%sdiagnostics/%s-polarnoise.bmp", templatedir, _eyeimage_filename);
	//sprintf(fname, "%s-polarnoise.bmp", eyeimage_filename);
	//imwrite(fname, &noisearrayimg);
	free(noisearrayimg.data);

	/*% WRITE NORMALISED PATTERN, AND NOISE PATTERN
	%w = cd;
	%cd(DIAGPATH);
	//imwrite(polar_array,[eyeimage_filename,'-polar.jpg'],'jpg');
	//imwrite(noise_array,[eyeimage_filename,'-polarnoise.jpg'],'jpg');
	%cd(w);

	% perform feature encoding*/
	

	encode(&polar_array, &noise_array, nscales, minWaveLength, mult, sigmaOnf, _template, mask, width, height); 
	
	free(polar_array.data);
	free(noise_array.data);
	free(imagewithnoise.data);
	free(eyeimage->data);
	free(eyeimage);
	//free(savefile);
}

