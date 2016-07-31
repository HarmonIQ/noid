#include "FindEyelidCurve.h"
#include <iostream>
using namespace std;
#include <string.h>
#include <stdio.h>
#include <math.h>
#include "ImageQuality.h"

static int maxFunc(int orgVal, int newVal) 
{
  return (orgVal < newVal) ? newVal : orgVal;
}

static int minFunc(int orgVal, int newVal) 
{
  return (orgVal >newVal) ? newVal : orgVal;
}


FindEyelidCurve::FindEyelidCurve(void)
{
}

FindEyelidCurve::~FindEyelidCurve(void)
{
}

// Eyelid curves
void FindEyelidCurve::findCurves(IplImage* iplImg, 
									 int xPupil, int yPupil,int rPupil, 
									 int xIris, int yIris, int rIris, 
									 double* x, double* ty, double* by)
{
	// Convert IplImage to IMAGE type
	ImageUtility * imgUtil = NULL;
	Masek::IMAGE* eyeImage = imgUtil->convertIplToImage(iplImg);
	
	// Detect the top and bottom eyelid points
	int sX1=0, sX2=0, sX3=0, stY1=0, sbY2=0;
	//begining/ending curve points: can be adjusted depending on your purpose
	int extra = rIris/2;
	// Start X for the left point
	sX1 = imgUtil->getValue(xIris-rIris-extra, eyeImage->hsize[1]-1);
	// Start X for the right point
	sX2 = imgUtil->getValue(xIris+extra, eyeImage->hsize[1]-1);
	// Start X for the center point
	sX3 = imgUtil->getValue(xIris-extra, eyeImage->hsize[1]-1);
	// Start top Y (iris' edge to the pupil's center point)
	stY1 = imgUtil->getValue(yIris-rIris, eyeImage->hsize[0]-1);
	// Start bottom Y (pupil's edge to iris' edge)
	sbY2 = imgUtil->getValue(yPupil+rPupil, eyeImage->hsize[0]-1);

	// Height of the bottom area
	int hBottom = (yIris+rIris)-sbY2;
	if(hBottom < 0)
		hBottom = 0;
	int endY = imgUtil->getValue(yIris+rIris-1, eyeImage->hsize[0]-1);
	if(endY < 0)
		endY =0;

	// Extract ROI
	Masek::IMAGE* leftTopEyelid = imgUtil->setROIImage_C(eyeImage, sX1, stY1, rIris, rIris);
	Masek::IMAGE* rightTopEyelid = imgUtil->setROIImage_C(eyeImage, sX2, stY1, rIris, rIris);
	Masek::IMAGE* leftBottomEyelid = imgUtil->setROIImage_C(eyeImage, sX1, sbY2, rIris, hBottom);
	Masek::IMAGE* rightBottomEyelid = imgUtil->setROIImage_C(eyeImage, sX2, sbY2, rIris, hBottom);
	Masek::IMAGE* centerTopEyelid = imgUtil->setROIImage_C(eyeImage, sX3, stY1, extra*2, hBottom);
	Masek::IMAGE* centerBottomEyelid = imgUtil->setROIImage_C(eyeImage, sX3, sbY2, extra*2, hBottom);

	// Initialize...
	int min = -1;
	int max = eyeImage->hsize[0]+1;
	// Left/right top y maxium
	int maxltY = max, maxrtY = max;
	// Left/right top y minimum
	int minltY = min, minrtY = min;
	// Left/right bottom y maxium
	int maxlbY = max, maxrbY = max;	
	// Left/right bottom y minimum
	int minlbY = min, minrbY = min;

	// Left/right center y maxium
	int maxctY = max, minctY=min;
	// Left/right center y minimum
	int mincbY = min, maxcbY=max;

	getEyelidPoint(leftTopEyelid, eyeImage, minltY, maxltY, sX1,stY1);
	getEyelidPoint(rightTopEyelid, eyeImage, minrtY, maxrtY, sX2,stY1);	
	getEyelidPoint(leftBottomEyelid, eyeImage, minlbY, maxlbY, sX1, sbY2);	
	getEyelidPoint(rightBottomEyelid, eyeImage, minrbY, maxrbY,sX2, sbY2);

  	getEyelidPoint(centerTopEyelid, eyeImage, minctY, maxctY, sX3,stY1);
	getEyelidPoint(centerBottomEyelid, eyeImage, mincbY, maxcbY, sX3,sbY2);

	// Values from the other side line
	if(maxltY  < 1 ||maxltY > eyeImage->hsize[0]-1)
		maxltY = maxrtY;
	if(maxrtY < 1 ||maxrtY > eyeImage->hsize[0]-1)
 		maxrtY = maxltY;

	if(minltY < 1 || minltY > eyeImage->hsize[0]-1)
		minltY = imgUtil->getValue(stY1+1, yIris);
	if(minrtY < 1 || minrtY > eyeImage->hsize[0]-1)
 		minrtY = imgUtil->getValue(stY1+1, yIris);	
	
	if(maxltY  < 1 ||maxltY > eyeImage->hsize[0]-1)
		maxltY = imgUtil->getValue(stY1+1, yIris);
	if(maxrtY < 1 ||maxrtY > eyeImage->hsize[0]-1)
 		maxrtY = imgUtil->getValue(stY1+1, yIris);

	// Values from the other side line
	if(minlbY < 1 || minlbY  > eyeImage->hsize[0]-1)
		minlbY = minrbY;
	if(minrbY < 1|| minrbY  > eyeImage->hsize[0]-1)
 		minrbY = minlbY;

	if(minlbY < 1 || minlbY  > eyeImage->hsize[0]-1)
		minlbY = endY;
	if(minrbY < 1|| minrbY  > eyeImage->hsize[0]-1)
 		minrbY = endY;
	if(maxlbY < 1 ||  maxlbY > eyeImage->hsize[0]-1)
		maxlbY = endY;
	if(maxrbY < 1 || maxrbY > eyeImage->hsize[0]-1)
		maxrbY = endY;

	// Top: Select the minimum y point between the left and right maximum points of the line
	int center_t_Y = cv::max(minltY, minrtY);
	// Bottom: Select the minimum y point between the left and right minimum points of the line
	int center_b_Y = cv::min(maxlbY, maxrbY);
	
	if(center_t_Y < maxctY)
		center_t_Y = maxctY;
	if(center_b_Y > mincbY)
 		center_b_Y = mincbY;
	if(maxltY < center_t_Y)
		maxltY = center_t_Y;
	if(maxrtY < center_t_Y)
		maxrtY = center_t_Y;
	if(minlbY > center_b_Y)
		minlbY = center_b_Y;
	if(minrbY > center_b_Y)
		minrbY = center_b_Y;  

	int endX = imgUtil->getValue(xIris+rIris+extra, eyeImage->hsize[1]);
	
	int adj = 2;//use for adjustment: pixel unit
	x[0] = (double)sX1;
	x[1] = (double)xIris;
	x[2] = (double)endX;

	ty[0] = (double)maxltY+adj;
	ty[1] = (double)center_t_Y+adj;
	ty[2] = (double)maxrtY+adj;
	
  	by[0] = (double)minlbY-adj;
	by[1] = (double)center_b_Y-adj;
	by[2] = (double)minrbY-adj;

	delete imgUtil;

	free(eyeImage->data);
	free(eyeImage);
	
	free(leftTopEyelid->data);
	free(leftTopEyelid);
	free(rightTopEyelid->data);
	free(rightTopEyelid);

	free(leftBottomEyelid->data);
	free(leftBottomEyelid);
	free(rightBottomEyelid->data);
	free(rightBottomEyelid);

	free(centerTopEyelid->data);
	free(centerTopEyelid);
	free(centerBottomEyelid->data);
	free(centerBottomEyelid);
}

// Eyelid lines
void FindEyelidCurve::findLines(IplImage* iplImg, 
									int xPupil, int yPupil, int rPupil, 
									int xIris, int yIris, int rIris,
									double* x, double* ty, double* by)
{
	// Convert IplImage to IMAGE type
	ImageUtility * imgUtil = NULL;
	Masek::IMAGE* eyeImage = imgUtil->convertIplToImage(iplImg);
	
	// Detect the top and bottom eyelid points
	int sX1 =0, stY1=0, sbY2=0;

	//sX1 = imgUtil->getValue(xIris-rIris-rPupil, eyeImage->hsize[1]-1);
	sX1 = imgUtil->getValue(xIris-rIris, eyeImage->hsize[1]-1);
	stY1 = imgUtil->getValue(yIris-rIris, eyeImage->hsize[0]-1);
	//start bottom Y (pupil's edge to iris' edge)
	sbY2 = imgUtil->getValue(yPupil+rPupil, eyeImage->hsize[0]-1);

	// Height of bottom area
	int hBottom = (yIris+rIris)-sbY2;
	int endY = imgUtil->getValue(yIris+rIris, eyeImage->hsize[0]-1);	

	Masek::IMAGE* topEyelid = imgUtil->setROIImage_C(eyeImage, sX1, stY1, rIris*2, rIris);	
	Masek::IMAGE* bottomEyelid = imgUtil->setROIImage_C(eyeImage, sX1, sbY2, rIris*2, hBottom);
	
	int min = -1;
	int max = eyeImage->hsize[0]+1;
	int maxtY = max;
	int maxbY = max;
	int mintY = min;
	int minbY = min;

	getEyelidPoint(topEyelid, eyeImage, mintY, maxtY, sX1,stY1);
	getEyelidPoint(bottomEyelid, eyeImage, minbY, maxbY, sX1,sbY2);

	if(mintY < 1 || mintY > eyeImage->hsize[0]-1)
		mintY = imgUtil->getValue(stY1+1, yIris);
	if(maxtY  < 1 ||maxtY > eyeImage->hsize[0]-1)
		maxtY = imgUtil->getValue(stY1+1, yIris);

	if(minbY < 1 || minbY  > eyeImage->hsize[0]-1)
		minbY = endY;	
	if(maxbY < 1 ||  maxbY > eyeImage->hsize[0]-1)
		maxbY = endY;

	int endX = imgUtil->getValue(xIris+rIris, eyeImage->hsize[1]);

	x[0] = (double)sX1;
	x[1] = (double)xIris;
	x[2] = (double)endX;
	int adj = 2;
	ty[0] = (double)maxtY+adj;
	ty[1] = (double)maxtY+adj;
	ty[2] = (double)maxtY+adj;
	
  	by[0] = (double)minbY-adj;
	by[1] = (double)minbY-adj;
	by[2] = (double)minbY-adj;	
	
	delete imgUtil;
	free(eyeImage->data);
	free(eyeImage);
	free(topEyelid->data);
	free(topEyelid);
	free(bottomEyelid->data);
	free(bottomEyelid);
}


void FindEyelidCurve::calcCurvePoints(IplImage* img, double* x, double* y, int* destY)
{
	int j=0;	
	int startX = (int)x[0];
	int endX = (int)x[2];
	
	for(int i=startX; i <= endX;i++)
	{
		destY[j] = (int)LagrangeInterpolation(x, y, 3, i);
		//LEE: DEBUG (20120516)
		if(destY[j] < 1)
			destY[j] = (int)*y;

		++j;
	}
}

void FindEyelidCurve::calcLinePoints(IplImage* img, double* x, double* y, int* destY)
{
	int j=0;	
	int startX = (int)x[0];
	int endX = (int)x[2];
	
	for(int i=startX; i <= endX;i++)
	{
		destY[j] = (int)y[0];
		++j;
	}
}

void FindEyelidCurve::maskOutNoise(Masek::IMAGE* image, 
								   double *x, int* ty, int *by)
{	
    int startX = (int)x[0];
    int endX = (int)x[2];
    int j = 0;
    for(int i = startX; i < endX;i++)
    {
        for(int a = 0; a <= ty[j]; a++)
        {
            int index = i + a * image->hsize[1];
            if ((index >= 0) && (index < image->hsize[0] * image->hsize[1]))
            {
                image->data[index] = (unsigned char)sqrt((double)-1);
            }
        }

        for(int b = image->hsize[0]-1; b >= by[j]; b--)//image->hsize[0]-1: important
        {
            int index  = i + b * image->hsize[1];
            if ((index >= 0) && (index < image->hsize[0] * image->hsize[1]))
            {
                image->data[index] = (unsigned char)sqrt((double)-1);
            }
        }

        j++;
    }
}


// Find the eyelid points
void FindEyelidCurve::getEyelidPoint(Masek::IMAGE* image, 
									 Masek::IMAGE* eyeImage,
									 int& min, int& max, 
								     int originX, int originY)
{
	//Masek* masek = new Masek();
    Masek* masek = NULL;
    int lineCount;
    double *lines;
    int *xl, *yl;    
	int tMax = min;
	int tMin = max;


    // Find the top eyelid
    if (image->hsize[0]>1 && image->hsize[1]>1)
	{
       lineCount = masek->findline(image, &lines);
       
	}
    else
	{
	    lineCount = 0;
	}

    if (lineCount > 0)
    {
	    xl = (int*)malloc(sizeof(int)*image->hsize[1]);
	    yl = (int*)malloc(sizeof(int)*image->hsize[1]);

        masek->linescoords(lines, image->hsize[0], image->hsize[1], xl, yl);
        

	    for (int i = 0; i<image->hsize[1]; i++)
	    {
			xl[i] = xl[i]+originX;
		    yl[i] = yl[i]+originY;		
		   
			
			tMin = minFunc(tMin, yl[i]);
			tMax = maxFunc(tMax, yl[i]);
			
	    }
	    free (xl);
	    free (yl);
	    free(lines);
    }
	min = tMin;
	max = tMax;
    delete masek;
}

// Lagrange Interpolation
double FindEyelidCurve::LagrangeInterpolation(double *x, double *f, int n, double xbar)
{
	int i,j;
	double fx = 0.0;
	double l = 1.0;
	for (i=0; i<=n; i++)
	{
		l=1.0;
		for (j=0; j<=n; j++)
			 if (j != i) l *= (xbar-x[j])/(x[i]-x[j]);
		
		fx += l*f[i];
	}
	return fx;
}

