#include "EyeRegionExtraction.h"
#include "AlignLRPupilPos.h"
#include "FindPupilCircleNew.h"

void EyeRegionExtraction::doExtract(IplImage* currentImg, int rPupilMax, int rIrisMax,
									 double ratio4Circle, int closeItr, int openItr, 
									 int speed_m, int alpha, double norm, 
		                             float nScale, const char* format, IplImage*& lImg, IplImage*& rImg)
{
	
	// Convert image to the gray scale
	IplImage* grayImg = NULL;
	grayImg = ImageUtility::convertToGray(currentImg);
	
	const int space = cvRound((grayImg->width/5) / 2);
	int circle1[6]; // Right pupil circle info
	int circle2[6]; // Left pupil circle info

	FindPupilCircleNew::doDetectTwoPupils(grayImg, (int)(rPupilMax*nScale), space, ratio4Circle, closeItr, openItr, speed_m, alpha, norm,
		                                  nScale, circle1, circle2);	
    AlignLRPupilPos::alignDegreeDiff(grayImg, (int) (rIrisMax*nScale), circle1, circle2, lImg, rImg);
	cvReleaseImage(&grayImg);	
}
