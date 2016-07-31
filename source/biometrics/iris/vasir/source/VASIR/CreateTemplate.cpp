#include "CreateTemplate.h"
#include "Masek.h"
#include "EncodeLee.h"
#include "ImageUtility.h"
#include "FindPupilCircleNew.h"
#include "FindIrisCircle.h"
#include "FindHighLights.h"
#include "FindEyelidCurve.h"
#include "Normalization.h"

// Run left and right eye together for video
void CreateTemplate::newCreateIrisTemplate(const char *fileName, 										
										int **template1, int **mask1, 
										int *width, int *height, 
										int dataType)
{
	ImageUtility *imgUtil = NULL;
	// Load the image as gray scale
	IplImage* eyeImg = NULL;
	eyeImg = cvLoadImage(fileName,0);
	if(eyeImg == NULL)
	{
		cout << "Failed to load the file" << endl;
		return;
	}

	// Create an image
    //IplImage* grayImg = NULL;
    //grayImg = cvCloneImage(eyeImg);
    IplImage* grayImg = NULL;
    grayImg = cvCreateImage(cvSize(eyeImg->width, eyeImg->height), 8, 1);
    cvCopyImage(eyeImg, grayImg);

	if(grayImg == NULL)
	{
		cout << "Failed to load the file" << endl;
		return;
	}		
	
	/********************************************************
	 * Iris Segmentation
	 *********************************************************/	
	//PUPIL INPUTS	
	float nScale = 1.0;		
	const int speed_m = 1;// Default 1
	int alpha = 20; // Alpha value for contrast threshold
	// Setup the parameters to avoid that noise caused by reflections and 
    // eyelashes covers the pupil
	double ratio4Circle = 1.0;
    // Initialize for Closing and Opening process
	int closeItr = 2;//dilate->erode
	int openItr = 3;//erode->dilate
	double norm = 256.0;

	//IRIS INPUTS
	double scaling = 0.4;// Default
	double lowThres = 0.11;// Default
	double highThres = 0.19;// Default 

	if(dataType == NIR_IRIS_STILL) //classical iris image
	{
		nScale = 2.0;
		alpha = 25;		
		ratio4Circle = 0.90;
		closeItr = 2;
		openItr = 3;
		scaling = 0.4;
	}	
	else if(dataType == NIR_FACE_VIDEO) // Distant video frame
	{	
		nScale = 1.0;
		alpha = 20;		
		ratio4Circle = 0.65;
		closeItr = 0;
		openItr = 3;
		scaling = 0.45;
	}

	/*defined
	ICE2005_IRIS_LG2200		01 (CODE: ICE)
	MBGC_IRIS_LG2200		02 (CODE: MIL)
	MBGC_FACE_IOM			03 (CODE: MFI)	
	ND_IRIS20_LG4000		04 (CODE: N20)
	ND_IRIS48_LGICAM		05 (CODE: N48)
	ND_IRIS49_IRISGUARD		06 (CODE: N49)
	ND_IRIS59_CFAIRS		07 (CODE: N59)*/

	if(dataType == ICE2005_IRIS_LG2200) // Classic still images
	{
		nScale = 2.0;
		alpha = 20;		
		ratio4Circle = 0.92;
		closeItr = 2;
		openItr = 3;
		scaling = 0.25;
	}
	else if(dataType == MBGC_IRIS_LG2200) //02 (CODE: MIL)
	{
		nScale = 2.0;
		alpha = 30;		
		ratio4Circle = 0.92;
		closeItr = 2;
		openItr = 1;
		scaling = 0.2;
	}
	else if(dataType == MBGC_FACE_IOM) // Distant video imagery
	{
		nScale = 1.0;		
		alpha = 20;		
		ratio4Circle = 0.65;
		closeItr = 0;
		openItr = 3;
		scaling = 0.45;		
	}	
	else if(dataType == ND_IRIS20_LG4000) //04 (CODE: N20)
	{
		nScale = 2.0;		
		alpha = 38;		
		ratio4Circle = 0.92;
		closeItr = 0;
		openItr = 4;
		scaling = 0.3;
	}
	else if(dataType == ND_IRIS48_LGICAM) //05 (CODE: N48)
	{
		nScale = 2.0;
		alpha = 40;		
		ratio4Circle = 0.92;
		closeItr = 0;
		openItr = 4;
		scaling = 0.2;
	}
	else if(dataType == ND_IRIS49_IRISGUARD) //06 (CODE: N49)
	{
		nScale = 2.0;
		alpha = 18;		
		ratio4Circle = 0.92;
		closeItr = 3;//best for noScaled Still
		openItr = 2;//best for noScaled Still
		scaling = 0.2;
	}
	else if(dataType == ND_IRIS59_CFAIRS) //07 (CODE: N59)
	{
		nScale = 2.0;
		alpha = 4;		
		ratio4Circle = 0.92;
		closeItr = 0;//best for noScaled Still
		openItr = 3;//best for noScaled Still
		scaling = 0.4;
	}
		
	const int rPupilMax = (int) (42*nScale);// Maximum radius of pupil's circle
	const int rIrisMax = (int) (82*nScale);// Maximum radius of iris' circle
		
	//fine the pupil circle using contours
    int pupilCircle[6]={0};

	FindPupilCircleNew::doDetect(grayImg, rPupilMax, ratio4Circle, closeItr, openItr, speed_m, alpha, norm, nScale, pupilCircle);

	CvPoint xyPupil;
	xyPupil.x = pupilCircle[0];
	xyPupil.y = pupilCircle[1];
	int rPupil = pupilCircle[2];
	
	//ROI for detecting the iris circle
	ImageUtility::SETVALUE setVal = imgUtil->setImage(grayImg, xyPupil, rPupil, rIrisMax, rIrisMax);	//82 is the best for video images, previous 80
	IplImage* setImg = NULL;
	setImg = imgUtil->getROIImage(grayImg, setVal.rect.x, setVal.rect.width, setVal.rect.y, setVal.rect.height);
	if(setImg == NULL)
	{
		cout << "Failed to load the file" << endl;
		return;
	}

	int centerAdjust=(int)(rIrisMax/4);//(rIrisMax/5); //for video dataset

	//find the iris circle using Hough Transform
    int irisCircle[3]={0};

	FindIrisCircle::doDetect(setImg, rPupil, rIrisMax, scaling, lowThres, highThres, irisCircle);	
	CvPoint xyIris;
	xyIris.x = irisCircle[0];
	xyIris.y = irisCircle[1];	
	int rIris = irisCircle[2];
	xyIris = FindIrisCircle::getOriginPoints(xyPupil, xyIris, setVal.p, centerAdjust);
	
	cvReleaseImage(&setImg);
	
	//find the upper and lower eyelids
    double x[3]={0}, ty[3]={0}, by[3]={0};
	FindEyelidCurve *eyelid = NULL;
	Masek::IMAGE* noiseImage = imgUtil->convertIplToImage(eyeImg);	
	
	eyelid->findCurves(grayImg, xyPupil.x, xyPupil.y, rPupil, 
                                      xyIris.x, xyIris.y, rIris,
									  x, ty, by);
	
	int size =(int)(x[2]-x[0]+1);
	int *destTY = new int[size];
	int *destBY = new int[size];

	eyelid->calcCurvePoints(grayImg, x, ty, destTY);
	eyelid->calcCurvePoints(grayImg, x, by, destBY);

	//error at this function
	eyelid->maskOutNoise(noiseImage, x, destTY, destBY);
	delete[] destTY;
	delete[] destBY;	

	delete eyelid;

	/// \todo Possible to optimize?
	//Find the eyelashes and reflections on the iris region
	const int min=3, max=250;
	//FindHighLights::removeHighLights2(noiseImage, min, max);//5, 230
	FindHighLights::removeHighLights2(noiseImage,min, max);


    cvReleaseImage(&grayImg);
	cvReleaseImage(&eyeImg); 

    /********************************************************
	 * Iris Normalization
	 *********************************************************/	
	/// \todo Possible to optimize?
	//size of the normalized image

	//Evaluation: April 11 2012 (better)
	int radialRes = 34;//orginal value: 20
	int angularRes = 260;//orignal value: 240

	Masek::filter polarArray;
	Masek::IMAGE noiseArray;
	Masek::filter* imgWithNoise;
	imgWithNoise = (Masek::filter*) malloc(sizeof(Masek::filter));
	imgWithNoise->hsize[0] = noiseImage->hsize[0];
	imgWithNoise->hsize[1] = noiseImage->hsize[1];
	imgWithNoise->data = (double*) malloc(sizeof(double)*noiseImage->hsize[0]*noiseImage->hsize[1]);
	
	for (int y = 0; y < noiseImage->hsize[0]; y++) 
	{
		for (int x = 0; x < noiseImage->hsize[1]; x++) 
		{
			if(noiseImage->data[x + y*noiseImage->hsize[1]] < 1) 								
				imgWithNoise->data[x + y*imgWithNoise->hsize[1]] = sqrt((double) -1);
			else
				imgWithNoise->data[x + y*imgWithNoise->hsize[1]] = (double)noiseImage->data[x + y*noiseImage->hsize[1]];
		}
	}
	free(noiseImage->data);
	free(noiseImage);

    Normalization::normalizeiris(imgWithNoise, xyIris.x, xyIris.y, rIris,
    xyPupil.x, xyPupil.y, rPupil, (const int)radialRes, (const int)angularRes,
    &polarArray, &noiseArray);

	free(imgWithNoise->data);
	free(imgWithNoise);

	/********************************************************
	/* Iris Encoding
	*********************************************************/
	const int encodeScales = 1;
	const int mult = 1; //not applicable if using encodeScales = 1
	int minWaveLength = 18;		
	double sigmaOnf = 0.55;//0.5
    //float coefThresRate = 0.0f;// for FX: 0.25 and 0.10

	float magLowerThresRate = 0.02f;
	float magUpperThresRate = 0.85f;

	EncodeLee::newEncodeLee(&polarArray, &noiseArray, encodeScales, (const int)minWaveLength, 
		mult, (const double)sigmaOnf, template1, mask1, width, height, 
        magLowerThresRate, magUpperThresRate);

	free(polarArray.data);	
	free(noiseArray.data);	
	delete imgUtil;	
}

