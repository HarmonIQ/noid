#include "ImageUtility.h"
#if _MSC_VER
    #ifndef snprintf
    #define snprintf _snprintf
    #endif
#endif

ImageUtility::ImageUtility()
{
	
}

ImageUtility::~ImageUtility()
{
}

float ImageUtility::myMean(IplImage* img, double n)
{
	float mean = 0.0f;
	double sum =0.0f;

	for(int y=0; y< img->height; y++)
		for(int x=0; x< img->width; x++)
		{
			sum += (unsigned char) img->imageData[y*img->widthStep+x];
		}
	mean = (float) sum / (float)n;
	return mean;
}

float ImageUtility::mySD(IplImage* img, double n, float mean)
{
	float variance = 0.0f, sd=0.0f;

	// Use a look-up table for faster calculation
	double lookup[GRAY_LEVEL];
	for (int val = 0; val < GRAY_LEVEL; val++) 
	{
		lookup[val] = pow((val - mean), 2);
	}

	for(int y=0; y<img->height; y++)
	{
		for(int x=0; x<img->width; x++)
		{
			unsigned char val = (unsigned char) img->imageData[y*img->widthStep+x];
			variance += (float)lookup[val];
		}
	}
	sd = sqrt(variance/(float)(n));
	return sd;
}

void ImageUtility::showImage(const char* name, IplImage* img)
{
	cvNamedWindow(name, 1);
	cvShowImage(name, img);
	cvWaitKey(0);
 	cvDestroyWindow(name);
 }

void ImageUtility::showCircles(const char* name, IplImage* img, const char* eyeFileName, 
                           CvPoint xyPupil, int rPupil, CvPoint xyIris, int rIris, 
                           int* ellipseVal, double* angleVal)
{
	// Draw circles and ellipses
	cvCircle(img, xyPupil, rPupil, CV_RGB(255,255,255), 1, 8);
	cvCircle(img, xyIris, rIris, CV_RGB(255,255,255), 1, 8);
	cvEllipse(img, cvPoint(ellipseVal[0],ellipseVal[1]),cvSize(ellipseVal[2], ellipseVal[3]), angleVal[0], 0, 180,CV_RGB(255,255,255),1,CV_AA, 0);
	cvEllipse(img, cvPoint(ellipseVal[0],ellipseVal[1]),cvSize(ellipseVal[2], ellipseVal[4]), angleVal[0], 180, 360,CV_RGB(255,255,255),1,CV_AA, 0);

    // Show the image
	showImage(name, img);

	// Save the image
    //char* saveFName = SaveEyeImages(img, (char*)eyeFileName, name);
	//delete[] saveFName;
}

void ImageUtility::drawCross(IplImage* eyeImg, int centerx, int centery, int xCrossLength, int yCrossLength, CvScalar color)
{
  // Create a copy of the image
  IplImage* colorImg = NULL;
  colorImg = cvCreateImage(cvSize(eyeImg->width, eyeImg->height), 8, 3);
  
  cvCvtColor(eyeImg, colorImg, CV_GRAY2RGB);

  // Calculate the cross' parameters
  CvPoint pt1,pt2,pt3,pt4;
  pt1.x = centerx - xCrossLength;
  pt1.y = centery;
  pt2.x = centerx + xCrossLength;
  pt2.y = centery;

  pt3.x = centerx;
  pt3.y = centery - yCrossLength;
  pt4.x = centerx;
  pt4.y = centery + yCrossLength;

  // Draw the cross on top of the image
  cvLine(colorImg,pt1,pt2,color,1,8);
  cvLine(colorImg,pt3,pt4,color,1,8);


  showImage("Cross", colorImg);
  cvReleaseImage(&colorImg);
}

IplImage* ImageUtility::convertToGray(IplImage* img)
{
   IplImage* grayImg = NULL;
   grayImg = cvCreateImage(cvGetSize(img), img->depth, 1);
   cvCvtColor(img, grayImg, CV_RGB2GRAY);
   return grayImg; 
}


Masek::IMAGE* ImageUtility:: convertIplToImage(IplImage* iplImg)
{
	IplImage* grayImg = NULL;
	grayImg = cvCreateImage(cvSize(iplImg->width, iplImg->height), 8, 1);
	cvCopyImage(iplImg, grayImg);

	Masek::IMAGE * eyeImg;
	eyeImg = (Masek::IMAGE*) malloc (sizeof(Masek::IMAGE));
	eyeImg->hsize[0] = grayImg->height;
	eyeImg->hsize[1] = grayImg->width;
	eyeImg->data = (unsigned char*)malloc(sizeof(unsigned char)*eyeImg->hsize[0]*eyeImg->hsize[1]);

	for(int j=0; j<eyeImg->hsize[0]; j++)
		for(int i=0; i<eyeImg->hsize[1]; i++)
			eyeImg->data[i+j*eyeImg->hsize[1]] = grayImg->imageData[i+j*grayImg->widthStep];
	
	cvReleaseImage(&grayImg);
	return eyeImg;
}

IplImage* ImageUtility:: convertImageToIpl(Masek::IMAGE* image)
{
	IplImage* grayImg = NULL;
	grayImg = cvCreateImage(cvSize(image->hsize[1], image->hsize[0]), 8, 1);

	for(int j=0; j<grayImg->height; j++)
		for(int i=0; i<grayImg->width; i++)
			grayImg->imageData[i+j*grayImg->widthStep] = image->data[i+j*image->hsize[1]];
	
	return grayImg;
}


char* ImageUtility::SaveEyeImages(IplImage* img, char* fileName, const char* ch, const char* format)
{
	// Delete the extension (e.g.,"avi")
	char* dotPtr = strrchr(fileName, '.');
	size_t partLen = (dotPtr == NULL) ? strlen(fileName) : dotPtr - fileName;

	// Create a new filename
	char* saveFileName= new char[FILENAME_MAX];
	memcpy(saveFileName, fileName, partLen);
	saveFileName[partLen]='\0';	
    //sprintf_s(saveFileName, FILENAME_MAX, "%s%s.%s", saveFileName, ch, format);
    snprintf(saveFileName, FILENAME_MAX, "%s%s.%s", saveFileName, ch, format);

	// Copy the image and save it
	IplImage* grayImg = NULL;
	grayImg = cvCloneImage(img);
	cvSaveImage(saveFileName, grayImg);
	cvReleaseImage(&grayImg);	

	return saveFileName;  
}

// For analysis purposes:
// Save the extracted image and write the file info to a TXT file
void ImageUtility::SaveImageOptions(IplImage* img, char* fileName, int frame, const char* str, int num, int totalFrame)
{
	char buffer[1024];
	sprintf(buffer,"%d_%s%d", frame, str, num);
	const char* temp = buffer;
	char* saveFName = SaveEyeImages(img, fileName, temp, "bmp");
	delete[] saveFName;
}

IplImage* ImageUtility::extractImagePart(IplImage* img, CvRect& rect, int x, int y, int wd, int ht)
{
  rect = cvRect(x, y, wd, ht);
  cvSetImageROI(img, rect);
  IplImage* part = NULL;
  part = cvCreateImage(cvSize(wd, ht), img->depth, img->nChannels);
  cvCopyImage(img, part); // Copy the image instead of saving it
  cvResetImageROI(img);
  return part;
}

IplImage* ImageUtility:: getROIImage(IplImage* eyeImg, int startX, int width, int startY, int height)
{
	IplImage* setImg = NULL;
	setImg = cvCreateImage(cvSize(width, height), eyeImg->depth, eyeImg->nChannels);
	
	int i, j, x, y;	
	for(j = startY, y = 0; y < height; j++, y++)	
		for(i = startX, x = 0; x < width; i++, x++)		
			setImg->imageData[x+y*setImg->widthStep] = 
				eyeImg->imageData[i+j*eyeImg->widthStep]; // Use the widthStep for the alignment

	return setImg;
}

// Get the ROI set image
Masek::IMAGE* ImageUtility:: getROIImage_C(Masek::IMAGE* eyeImg, int startX, int width, int startY, int height)
{
	Masek::IMAGE* setImg;
	setImg = (Masek::IMAGE*) malloc (sizeof(Masek::IMAGE));
	setImg->hsize[0] = height ;
	setImg->hsize[1] = width ;
	setImg->data = (unsigned char*)malloc(sizeof(unsigned char)*setImg->hsize[0]*setImg->hsize[1]);
	
	int i, j, x, y;	
	for(j = startY-1, y = 0; y < height; j++, y++)	
		for(i = startX-1, x = 0; x < width; i++, x++)	
			setImg->data[x+y*setImg->hsize[1]] = 
			eyeImg->data[i+j*eyeImg->hsize[1]]; // For alignment

	return setImg;
}


ImageUtility::SETVALUE ImageUtility::setImage(IplImage* eyeImg, CvPoint center, int cr, int xLimit, int yLimit)
{
	SETVALUE setVal;

	// Initialize
	setVal.rect.x = 0;
	setVal.rect.y = 0;
    setVal.rect.width = 0;
    setVal.rect.height = 0;
	setVal.p.x = 0;
	setVal.p.y = 0;	
			
	if(center.x != 0 && center.y != 0 && cr != 0)
	{
		CvPoint newCircle;
        //int newRadius= cr;
		
		// Set ROI in the assumed iris area
        int startX=0, endX=0, startY=0, endY=0;
		
		if(center.x-cr < 0)
		{
			startX = MAX(0, 0);
			newCircle.x = center.x;
		}
		else
		{
			startX = MAX(center.x-xLimit, 0);
			newCircle.x = xLimit;
		}
		
		endX = MIN(center.x+xLimit, eyeImg->width);

		if(center.y-cr < 0)
		{
			startY = MAX(0,0);
			newCircle.y = center.y;
		}
		else
		{
			startY = MAX(center.y-yLimit, 0);
			newCircle.y = yLimit;
		}

		endY = MIN(center.y+yLimit, eyeImg->height);
		
		// Return ROI and center points
		setVal.rect.x = startX;
		setVal.rect.y = startY;
		setVal.rect.width = endX-startX;
		setVal.rect.height = endY-startY;
		setVal.p.x = newCircle.x;
		setVal.p.y = newCircle.y;

		if(setVal.p.x < 1 || setVal.p.y < 1)
		{
			cout << "Failed to load ROI for new circle" << endl;
		}
	}
	return setVal;
}

void ImageUtility:: myRect(IplImage* img, int x, int y, int radius, int* destVal)
{  
	int startX, endX, startY, endY;
	startX = x-radius;// X starting point
	endX = x+radius;// X end point	
    startY = y-radius;// Y starting point
	endY = y+radius;// Y end point

	//debug
	destVal[0] = getValue(startX, img->width);
	destVal[1] = getValue(endX, img->width);	
	destVal[2] = getValue(startY, img->height);
	destVal[3] = getValue(endY, img->height);
	
}

void ImageUtility:: myXYRect(IplImage* img, int x, int y, int width, int height, int* destVal)
{  
	int startX, endX, startY, endY;
	startX = x-width;// X starting point
	endX = x+width;// X end point	
    startY = y-height;// Y starting point
	endY = y+height;// Y end point

	// Debugging
	destVal[0] = getValue(startX, img->width);
	destVal[1] = getValue(endX, img->width);	
	destVal[2] = getValue(startY, img->height);
	destVal[3] = getValue(endY, img->height);
}

void ImageUtility:: myRect_C(Masek::IMAGE *image, int x, int y, int radius, int* destVal)
{  
	int startX, endX, startY, endY;
	startX = x-radius;// X starting point
	endX = x+radius;// X end point	
    startY = y-radius;// Y starting point
	endY = y+radius;// Y end point
	
	destVal[0] = getValue(startX, image->hsize[1]);
	destVal[1] = getValue(endX, image->hsize[1]);	
	destVal[2] = getValue(startY, image->hsize[0]);
	destVal[3] = getValue(endY, image->hsize[0]);

}

void ImageUtility:: myXYRect_C(Masek::IMAGE *image, int x, int y, int width, int height, int* destVal)
{  
	int startX, endX, startY, endY;
	startX = x-width;// X starting point
	endX = x+width;// X end point	
    startY = y-height;// Y starting point
	endY = y+height;// Y end point
	
	destVal[0] = getValue(startX, image->hsize[1]);
	destVal[1] = getValue(endX, image->hsize[1]);	
	destVal[2] = getValue(startY, image->hsize[0]);
	destVal[3] = getValue(endY, image->hsize[0]);

}

// Debug values
int ImageUtility::getValue(int value, int maxSize)
{
	if(value < 1)
		value = 1;
	if(value > maxSize)
		value = maxSize;	
	return value;
}

IplImage* ImageUtility::setROIImage(IplImage* img, int x, int y, int wd, int ht)
{
	// Make a copy of the given image

	int icl, icu, irl, iru;
    int val[4];
    myXYRect(img, x, y, wd, ht, val);

	icl = val[0];// X starting point
	icu = val[1];// X ending point
    irl = val[2];// Y starting point
	iru = val[3];// Y ending point
	
	int startX = MAX(icl, x);
	int wd2 = MIN(wd, img->width-startX);
	int startY = MAX(irl, y);// 2 pixels: fixing the error for the pupil 
	int ht2 = MIN(ht, img->height-startY);
	
	IplImage* roiImg = NULL;
	roiImg = getROIImage(img, startX, wd2, startY, ht2);
	return roiImg;
}

Masek::IMAGE * ImageUtility::setROIImage_C(Masek::IMAGE *image, int x, int y, int wd, int ht)
{
	// Make a copy of the given image

	int icl, icu, irl, iru;
    int val[4];
    myXYRect_C(image, x, y, wd, ht, val);

	icl = val[0];// X starting point
	icu = val[1];// X ending point
    irl = val[2];// Y starting point
	iru = val[3];// Y ending point
	
	int startX = MAX(icl, x);
	int wd2 = MIN(wd, image->hsize[1]-startX);
	int startY = MAX(irl, y);// 2 pixels: fixing the error for the pupil 
	int ht2 = MIN(ht, image->hsize[0]-startY);
	
	Masek::IMAGE * roiImg = NULL;
	roiImg = getROIImage_C(image, startX, wd2, startY, ht2);
	return roiImg;
}



