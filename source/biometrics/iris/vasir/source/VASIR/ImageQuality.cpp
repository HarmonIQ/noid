#include "ImageQuality.h"
#include <math.h>

double ImageQuality::doProcess(IplImage* img, int factors)
{
	double ed = 0.0;
	int numPix = 0;

	if(img == NULL)
	{
		cout << "ERROR: Failed to load the image" << endl;
		return -1;
	}	

	IplImage* eyeImg = NULL;
	eyeImg = cvCloneImage(img);	
	
    //double n = eyeImg->width*eyeImg->height;
	double scores = 0.0;
	//debug
	if(factors < 0 || factors > 12)
	{
		cout << "ERROR: Failed to load the image quality measures" << endl;
		return -1;
	}
	switch(factors)
	{
		// Spacial domain
		case 0: scores = calcSobelEdge(eyeImg,numPix); break;
	}

	ed = (double)(scores/numPix);
	//cout << "quality score : " << ed <<endl;
	cvReleaseImage(&eyeImg);
	return ed;  
}

//Second-order statistics.
//*Note: j=X, i=Y 
float ImageQuality::_contrast(IplImage* img, double n, double** _glcm)
{	
	float contrast = 0.0;
	for(int i=0; i<GRAY_LEVEL; i++)
	{
		for(int j=0; j<GRAY_LEVEL; j++)
		{			
			contrast += (float)(pow((double)(j-i),2)*_glcm[i][j]);
		}
	}	
	return contrast;	
}

/*
 * Direction independent GLCM (Gray Leve Co-occurence Matrics)
 */
double** ImageQuality::mGLCM(IplImage*img)
{
	// Supported angles (0,45,90,135)
	double n = 4.0f; // number of directions
	double** _glcm1 = GLCM(img,0);
	double** _glcm2 = GLCM(img,45);
	double** _glcm3 = GLCM(img,90);
	double** _glcm4 = GLCM(img,135);

	// Angular independent texture average
	double** meanGLCM; 
	meanGLCM = new double*[GRAY_LEVEL];
	for(int i=0; i<GRAY_LEVEL; i++)
	{
		meanGLCM[i] = new double[GRAY_LEVEL];
		for(int j=0; j<GRAY_LEVEL; j++)
		{
			meanGLCM[i][j] = (_glcm1[i][j]+_glcm2[i][j]+_glcm3[i][j]+_glcm4[i][j])/n;
		}
	}
	_deleteGLCM(_glcm1);
	_deleteGLCM(_glcm2);
	_deleteGLCM(_glcm3);
	_deleteGLCM(_glcm4);

	return meanGLCM;
}


double** ImageQuality::GLCM(IplImage* img, int phi)
{
	static int d = 1;
	static bool symmetry = true;
  
	int value; // value at pixel of interest
	int dValue; // value of pixel at offset 
	double pixelCount = 0;	
	int offsetX = 1;
	int offsetY = 0;	

	double** glcm;
	glcm = new double*[GRAY_LEVEL];
	for(int y=0;y<GRAY_LEVEL;y++)
	{
		glcm[y]= new double[GRAY_LEVEL];
		for(int x=0;x<GRAY_LEVEL;x++)
		{
			glcm[y][x] = 0.0f; // Initialize
		}
	}

	// Set our initial offsets based on the selected angle (default:(1,0))
	if (phi== 0)//(1,0) 
	{
		offsetX = d;
		offsetY = 0;
	} 
	else if(phi == 45)//(1,-1) 
	{
		offsetX = d;
		offsetY = -d;
	} 
	else if(phi == 90)//(0,-1) 
	{
		offsetX = 0;
		offsetY = -d;
	} 
	else if(phi == 135)//(-1,-1) 
	{
		offsetX = -d;
		offsetY = -d;
	} 
	else // Invalid angle parameter
	{    
		cout <<"The requested angle" << phi << "is not one of the supported angles (0,45,90,135)" <<endl;
	}  

	for(int y=0;y<img->height;y++)
	{
		for(int x=0;x<img->width;x++)
		{
			value = 0xff & (unsigned char)img->imageData[x+y*img->widthStep];

			// Supported angles (0, 45, 90, 135)
			if((0 <= (x+offsetX) && (x+offsetX) < img->width)
				&&(0 <= (y+offsetY) && (y+offsetY) < img->height))
			{			
				dValue = 0xff & (unsigned char)img->imageData[(x+offsetX)+(y+offsetY)*img->widthStep];		  
				glcm[value][dValue]++;
				pixelCount++;
			}		  
			
			if(symmetry) // Supported angles (180, 225, 270, 315)
			{		
			    if((0 <= (x-offsetX) && (x-offsetX) < img->width)
				&&(0 <= (y-offsetY) && (y-offsetY) < img->height))
				{				  
					dValue = 0xff & (unsigned char)img->imageData[(x-offsetX)+(y-offsetY)*img->widthStep];
					glcm[dValue][value]++;
					pixelCount++;
				}
			}
		}
   }

  // Convert the GLCM to probabilities	
  for(int i=0;i<GRAY_LEVEL;i++)//y
  {
	  for(int j=0;j<GRAY_LEVEL;j++)//x
	  {
		glcm[i][j] = glcm[i][j]/pixelCount;
	  }
  }
  return glcm;

}

void ImageQuality::_deleteGLCM(double** _glcm)
{
	for(int i=0; i<GRAY_LEVEL; i++)
	{
		delete[] _glcm[i];
	}	
	delete[] _glcm;
}

