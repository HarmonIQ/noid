#include "CVImageSource.h"
#include <stdio.h>
#include <string.h>

CVImageSource::CVImageSource()
{
	copy = NULL;
}

CVImageSource::~CVImageSource()
{
	if (copy != NULL)
	{
		cvReleaseImage(&copy);
	}
	cvReleaseCapture(&capture);
}

IplImage* CVImageSource::getNextImage()
{
    if (!cvGrabFrame(capture))
    {
        return NULL;
    }

    IplImage* frame = cvRetrieveFrame(capture);
    if (!frame)
    {
      return NULL;
    }

    // Check the origin of image. If top left - ok
    if (frame->origin == IPL_ORIGIN_TL)
    {
      return frame;
    }

    if (copy != NULL) {
      cvReleaseImage(&copy);
    }
    
    // Allocate framecopy as the same size of the frame
    copy = cvCreateImage(cvSize(frame->width,frame->height),
                         IPL_DEPTH_8U, frame->nChannels );

    // Flip and copy the image
    cvFlip(frame, copy, 0);

    return copy;
}

CVCameraSource::CVCameraSource()
{
	capture = cvCaptureFromCAM(0);
	if(!capture)
	{
		printf("Fail to load the image from the camera\n");
	}
}
CVCameraSource::~CVCameraSource()
{
}

const char* CVCameraSource::getDescription() const
{
	return "Capture from camera";
}

CVVideoSource::CVVideoSource(const char *fileName)
{
	capture = cvCaptureFromAVI(fileName);
	if(!capture)
	{
		printf("Fail to load the AVI file\n");
	}

    numFrames = (int)cvGetCaptureProperty(capture, CV_CAP_PROP_FRAME_COUNT);
}
CVVideoSource::~CVVideoSource()
{
}

const char* CVVideoSource::getDescription() const
{
	return "Video file";
}

int CVVideoSource::getNumberOfImages() const 
{
    return numFrames;
}

//CV_CAP_PROP_POS_AVI_RATIO doesn't work here
bool CVVideoSource::hasNextImage()
{
    // Not at the end of the movie
    return ((int)cvGetCaptureProperty(capture, CV_CAP_PROP_POS_FRAMES) < numFrames);
};


