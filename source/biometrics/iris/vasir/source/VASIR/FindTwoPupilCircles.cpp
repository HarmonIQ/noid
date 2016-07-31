#include "FindPupilCircleNew.h"
#include "ImageUtility.h"
#include <vector>
#include <algorithm>

void FindPupilCircleNew::doDetectTwoPupils(IplImage* img, 
										int rPupilMax, 
										const int space,
										double ratio4Circle,
										int closeItr,
										int openItr,
										int speed_m,
										int alpha,
										double norm,
										float nScale,
										int* circle1, 
										int* circle2)
{
	// Make a copy of the given image
	IplImage* grayImg = NULL;
	grayImg = cvCloneImage(img);

	//for the right eye
	int wd1 = grayImg->width/2-space;
	IplImage* rImg = NULL;
	rImg = ImageUtility::setROIImage(grayImg,0,0, wd1, grayImg->height); 
	doDetect(rImg, rPupilMax, ratio4Circle, closeItr, openItr, speed_m, alpha, norm, nScale, circle1);
	
	cvReleaseImage(&rImg);

	//for the left eye
	int startX = grayImg->width/2+space;
	int wd2 = grayImg->width-(grayImg->width/2+space);
	IplImage* lImg = NULL;
	lImg =ImageUtility::setROIImage(grayImg,startX, 0, wd2, grayImg->height); 
	doDetect(lImg, rPupilMax,  ratio4Circle, closeItr, openItr, speed_m, alpha, norm, nScale, circle2);
	circle2[0] = ImageUtility::getValue(circle2[0]+startX, grayImg->width-1);
	if(circle2[0] == 1)
		circle2[0] = startX + 1;
	
	cvReleaseImage(&lImg);

	cvReleaseImage(&grayImg);	
}