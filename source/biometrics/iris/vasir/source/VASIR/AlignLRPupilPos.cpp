#include "AlignLRPupilPos.h"
#include "FindPupilCircleNew.h"

AlignLRPupilPos::AlignLRPupilPos(void)
{
}


AlignLRPupilPos::~AlignLRPupilPos(void)
{
}


void AlignLRPupilPos::alignDegreeDiff(IplImage* img, 										  
                                      int rIrisMax,
									  int* circle1, int* circle2,
									  IplImage*& lImg, IplImage*& rImg)
{
	// Initialize...
	int px1 = 0, py1 = 0, r1 = 0;
	int px2 = 0, py2 = 0, r2 = 0;
	// Right pupil circle info
	px1 = circle1[0];
	py1 = circle1[1];
	r1 = circle1[2];
	// Left pupil circle info
	px2 = circle2[0];
	py2 = circle2[1];
	r2 = circle2[2];

	//debug
	if(r1 < 1)
		r1 = 1;
	if(r2 < 1)
		r2 = 1;

	double angle = getAngle(px1, py1, px2, py2);

	IplImage* rotatedImg = NULL;
	rotatedImg = rotate(img, angle);

	// Find the original point
	int pyy1 = 0;
	pyy1 = py1-cvRound((py1 - py2)*0.5);// not precise
	int pyy2 = 0;
	pyy2 = py2+cvRound((py1 - py2)*0.5);//not precise


	int xIrisMax = (int)(rIrisMax*1.4); // the image width
	int yIrisMax =(int)(rIrisMax*1.1); // the image height
		
	ImageUtility* imgUtil = NULL;
	// Save the right eye image
	rImg = imgUtil->setROIImage(rotatedImg, px1-xIrisMax, pyy1-yIrisMax, xIrisMax*2, yIrisMax*2);

	// Save the left eye image
	lImg = imgUtil->setROIImage(rotatedImg, px2-xIrisMax, pyy2-yIrisMax, xIrisMax*2, yIrisMax*2);	

	cvReleaseImage(&rotatedImg);
	delete imgUtil;
}

double AlignLRPupilPos::getAngle(int px1, int py1, int px2, int py2)
{
	int a = 0, b = 0;
	a = abs(px2 - px1);
	b = abs(py2 - py1);		
	double c = sqrt((double)(a*a + b*b)) ;
	double angle = acos(a/c)*(180/PI);
	if(0 > (py1 - py2))
	{
		angle = -(angle);
	}
	return angle;
}


IplImage* AlignLRPupilPos::rotate(IplImage* eyeImg, double angle)
{
	IplImage* destImg = cvCreateImage(cvSize(eyeImg->width, eyeImg->height), 8, 1);
	double factor = 1;
    //int delta = 1;

	float m[6];
    CvMat M = cvMat(2, 3, CV_32F, m);
    int w = eyeImg->width;
    int h = eyeImg->height;

	m[0] = (float)(factor*cos(angle*2*CV_PI/360.));
    m[1] = (float)(factor*sin(angle*2*CV_PI/360.));
    m[3] = -m[1];
    m[4] = m[0];
    m[2] = w*0.5f;  
    m[5] = h*0.5f;  

    cvGetQuadrangleSubPix(eyeImg, destImg, &M);

	return destImg;
}

