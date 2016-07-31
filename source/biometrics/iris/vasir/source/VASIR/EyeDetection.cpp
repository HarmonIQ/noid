#include "EyeDetection.h"
#include "ImageUtility.h"

EyeDetection::EyeDetection(const char* cascadeFileName)
{ 
	storage = cvCreateMemStorage(0);

	// Load the HaarClassifier Cascade
	cascade = (CvHaarClassifierCascade*)cvLoad(cascadeFileName, 0, 0, 0 );
    //if(!cascade)
    //	printf("Fail to load the Cascade XML file\n");
	 
}

EyeDetection::~EyeDetection()
{
	if(cascade != NULL)  
		cvFree(&cascade);  

	if(storage != NULL)
		cvReleaseMemStorage(&storage); 
}

/**
 * @param scales 4: 2048x2048(MBGC Data), 2: 1024x1024, 1: 512x512
 *
 * @note/w = 200, h = 45: Set the minimum size of the two eye regions to avoid 
 * 		false detection
 */
EyeDetection::RESULT* EyeDetection::detect(IplImage* img, int scales, int val, int w, int h)
{
	if(storage != NULL)
		cvClearMemStorage(storage);  
	if(!cascade)	
		return NULL;

	int space; // for the nose bridge
	CvPoint pt1, pt2;// for retangles
	
	// Downsample input image to boost a performance (about 7 times faster)
	IplImage* imgSmall = NULL;
	imgSmall = cvCreateImage(cvSize(img->width/scales,img->height/scales), img->depth, img->nChannels );
	cvResize(img, imgSmall);
	
  
	// There can be more than one eye in an image. Therefore, create a 
    // growable sequence of faces.
	// Detect the objects and store them in the sequence    
	CvSeq* eyes = 0;

	// Use downscaled image
	eyes = cvHaarDetectObjects(imgSmall, cascade, storage,
                              1.1, 2, CV_HAAR_DO_CANNY_PRUNING, cvSize(40, 40));
	// Something was detected
	for(int i = 0; i < (eyes ? eyes->total : 0); i++ )
	{
		CvRect r = *(CvRect*)cvGetSeqElem(eyes, i);
		
		// Use the original size again
		r.x = r.x*scales;
		r.y = r.y*scales;
		r.width = r.width*scales;
		r.height = r.height*scales;

		// An eye; ignore noise; values are depended on your image resolution	
		// 800 (=200x4) and 180 (=45x4) are for MBGC FaceNIRVideo dataset
 		if(r.width > w*scales && r.height > h*scales)
		{
			// Create a new rectangle for extracting the eye(s)
			pt1.x = r.x;
			pt2.x = (r.x + r.width);//devide 2.5 1.5
			pt1.y = r.y;
			pt2.y = (r.y + r.height);
			
			// Remove the nose bridge region
	        space = cvRound((r.width/5) / 2);
			
			// Extract the left and right eye images out of the frame
			result.rightImg = ImageUtility::extractImagePart(img, result.rightRect, pt1.x, pt1.y+val, r.width/2-space, r.height-(val*2));
			result.leftImg = ImageUtility::extractImagePart(img, result.leftRect, pt1.x+(r.width/2+space), pt1.y+val, r.width/2-space, r.height-(val*2));
			result.bothImg = ImageUtility::extractImagePart(img, result.bothRect, pt1.x, pt1.y, r.width, r.height);
	
			return &result; // FIXME
		}
	}	
	if( imgSmall != NULL )
        cvReleaseImage( &imgSmall);

	return NULL;	
}
