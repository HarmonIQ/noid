#include "ImageQuality.h"

#define M_PI	3.14159265358979323846

/**
 * Creates an empty (all values = 0.0) image quality matrix.
 */
void ImageQuality::_create(double**& _tmp, int width, int height)
{
	_tmp = new double*[height];
	for(int y=0; y<height; y++)
	{
		_tmp[y] = new double[width];
		for(int x=0; x<width; x++)
		{
			_tmp[y][x] = 0.0;
		}
	}
}

/**
 * Deallocates a passed image quality matrix.
 */
void ImageQuality::_delete(double** _tmp, int size)
{
	for(int i=0; i<size; i++)
	{
		delete[] _tmp[i];
	}	
	delete[] _tmp;
}

/**
 * Calculates Sobel Edge.
 *
 * @param img Source image
 * @param (OUT) numPix Total number of processed pixel
 * @return Sum
 */
double ImageQuality::calcSobelEdge(IplImage *img, int& numPix)
{
	// 3x3 Sobel mask (X)
	const int maskX[3][3]={ -1, 0, 1,
						    -2, 0, 2,
				      		-1, 0, 1};

	const int maskY[3][3]={  1, 2, 1,
				       		 0, 0, 0,
				      		-1,-2,-1};	

	double sum = 0.0;
	double dx = 0.0, dy = 0.0;
	int countPix = 0;
	int i, j;

	IplImage* eyeImg = NULL;
	eyeImg = cvCloneImage(img);

	// Loop - ignoring the edges
	for(int y=1; y<img->height-1; y++)
	{
		for(int x=1; x<img->width-1; x++)
		{
			dx=0.0;
			dy=0.0;		 

			// Horizontal gradient (X)
			for(j=-1;j<=1;j++)
			{
				for(i=-1;i<=1;i++)
				{
				   dx = dx + (unsigned char)(img->imageData[(x+i)+(y+j)*img->widthStep])*maskX[j+1][i+1];
				}
			}
			// Vertical gradient (Y)
			for(j=-1;j<=1;j++)
			{
				for(i=-1;i<=1;i++)
				{
				   dy= dy + (unsigned char)(img->imageData[(x+i)+(y+j)*img->widthStep])*maskY[j+1][i+1];					  
				}
			}
			countPix++;

			// Gradient magnitude
			sum += sqrt((dx*dx)+(dy*dy));

		}
	}
   
	numPix = countPix;
	//ImageUtility::showImage("Sobel Mask", eyeImg);
	cvReleaseImage(&eyeImg);

	return sum;
}

