/***********************************************************
** @file ImageUtility.h
** Utility functions to process an image.
** 
** @date 12/2010
** @author Yooyoung Lee 
**
** Note: Please send BUG reports to yooyoung@<NOSPAM>nist.gov.
** For more information, refer to: http://www.nist.gov/itl/iad/ig/vasir.cfm
**
** @par Disclaimer
** This software was developed at the National Institute of Standards 
** and Technology (NIST) by employees of the Federal Government in the
** course of their official duties. Pursuant to Title 17 Section 105 
** of the United States Code, this software is not subject to copyright 
** protection and is in the public domain. NIST assumes no responsibility 
** whatsoever for use by other parties of its source code or open source 
** server, and makes no guarantees, expressed or implied, about its quality, 
** reliability, or any other characteristic.
***********************************************************
** @file ImageUtility.h
** Utility functions to process an image.
** 
** @date 12/2010
** @author Yooyoung Lee 
**
** Note: Please send BUG reports to yooyoung@<NOSPAM>nist.gov.
** For more information, refer to: http://www.nist.gov/itl/iad/ig/vasir.cfm
**
** @par Disclaimer
** This software was developed at the National Institute of Standards 
** and Technology (NIST) by employees of the Federal Government in the
** course of their official duties. Pursuant to Title 17 Section 105 
** of the United States Code, this software is not subject to copyright 
** protection and is in the public domain. NIST assumes no responsibility 
** whatsoever for use by other parties of its source code or open source 
** server, and makes no guarantees, expressed or implied, about its quality, 
** reliability, or any other characteristic.
**************************************************************/

#ifndef IMAGE_UTILITY_H
#define IMAGE_UTILITY_H

//#undef max

#include "Masek.h"
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/core/core_c.h>
#include <opencv2/imgproc/imgproc_c.h>

#include <time.h>
#include <iostream>
using namespace std;

//NoID: cvCopyImage renamed to cvCopy
#define cvCopyImage cvCopy
//NoID: cvFitEllipse renamed to cvFitEllipse2
#define cvFitEllipse cvFitEllipse2

/** Valid data types. */
#define NIR_IRIS_STILL 88 /// Classical still: Near-infrared iris still image.
#define NIR_FACE_VIDEO 99 /// Distant video: Near-infrared face video image.

#define ICE2005_IRIS_LG2200		01// (CODE: ICE)
#define MBGC_IRIS_LG2200		02// (CODE: MIL)
#define MBGC_FACE_IOM			03// (CODE: MFI)
#define ND_IRIS20_LG4000		04// (CODE: N20)
#define ND_IRIS48_LGICAM		05// (CODE: N48)
#define ND_IRIS49_IRISGUARD		06// (CODE: N49)
#define ND_IRIS59_CFAIRS		07// (CODE: N59)


/** Number of gray levels. */
#define GRAY_LEVEL 256

/**
 * Image processing utility class.
 */
class ImageUtility
{
public: 
	/**
	 * Region of Interest and center points information
	 */
	typedef struct 
	{
		CvRect	rect;	///< ROI.
		CvPoint p; 		///< Center point.
	} SETVALUE;

	ImageUtility();
	virtual ~ImageUtility();
	/** 
	 * Set an ROI of an image as IplImage.
	 *
	 * @param img	Input image.
	 * @param x		X starting point
	 * @param y		X end point
	 * @param wd	Width
	 * @param ht	Height
	 * @return Returns	IplImage type image of ROI
	 */
	static IplImage* setROIImage(IplImage* img, int x, int y, int wd, int ht);

	/** 
	 * Set an ROI of an image as IMAGE.
	 *
	 * @param image	Input image.
	 * @param x		X starting point
	 * @param y		X end point
	 * @param wd	Width
	 * @param ht	Height
	 * @return Returns	IMAGE type image of ROI
	 */
	static Masek::IMAGE * setROIImage_C(Masek::IMAGE *image, int x, int y, int wd, int ht);


	/** 
	 * Calculate the mean of an image.
	 * 
	 * @param img	Input image
	 * @param n		Total number of pixels
	 * @return		Mean value
	 */
	static float myMean(IplImage* img, double n);

	/** 
	 * Calculate the standard deviation of an image.
	 * 
	 * @param img	Input image
	 * @param n		Total number of pixels
	 * @param mean	Previously calculated mean value
	 * @return		Standard deviation value
	 * @see myMean
	 */
	static float mySD(IplImage* img, double n, float mean);
	
	/** 
	 * Displays an image and waits for a keypress.
	 *
	 * @note Uses OpenCV to display the image
	 *
	 * @param name	Window name (= caption)
	 * @param img	Input image
	 */
	static void showImage(const char* name, IplImage* img);

	/** 
	 * Displays an image overlaid with circles and ellipses.
	 *
	 * @note The image is modified in the process!
	 *
	 * @param name			Window name (= caption)
	 * @param img			Input image
	 * @param eyeFileName	If you would like to save the image, specify a filename
	 * @param xyPupil		Pupil's center point
	 * @param rPupil			Pupil's radius
	 * @param xyIris			Iris' center point
	 * @param rIris			Iris' radius
	 * @param ellipseVal	(OUT) Used to return the ellipse info
	 * @param angleVal	 	(OUT) Used to return the orientation
	 */
	static void showCircles(const char* name, IplImage* img, const char* eyeFileName, 
                           CvPoint xyPupil, int rPupil, CvPoint xyIris, int rIris, 
                           int* ellipseVal, double* angleVal);

	/** 
	 * Display an image overlaid with a cross.
	 *
	 * @param eyeImg			Input image
	 * @param centerx		Center X 
	 * @param centery		Center Y 
	 * @param xCrossLength	Cross' length of X 
	 * @param yCrossLength	Cross' length of Y 
	 * @param color			Cross' color
	 */
	static void drawCross(IplImage* eyeImg, int centerx, int centery, 
						int xCrossLength, int yCrossLength, CvScalar color);
  
	/** 
	 * Convert a color image to a gray-scale image.
	 *
	 * @note The returned image needs to be deallocated
	 * 
	 * @param img	Input image
	 * @return		Created gray-scale image
	 */
	static IplImage* convertToGray(IplImage* img);

	/** 
	 * Convert an IplImage to an IMAGE.
	 *
	 * @note The returned image needs to be deallocated
	 * 
	 * @param iplImg	Input IPL type image
	 * @return			IMAGE type image
	 * @see convertImageToIpl
	 */
	static Masek::IMAGE* convertIplToImage(IplImage* iplImg);
	//static IplImage* convertFilterToIpl(Masek::filter* image);

	/** 
	 * Convert an IMAGE to an IplImage
	 *
	 * @note The returned image needs to be deallocated
	 * 
	 * @param image Input IMAGE type image
	 * @return		IPL type image
	 * @see convertIplToImage
	 */
	static IplImage* convertImageToIpl(Masek::IMAGE* image);	

	/** 
	 * Save an IplImage as BMP file.
	 *
	 * Resulting filename: "<fileName>_<ch>.bmp"
	 *
	 * @param img		Input image
	 * @param fileName	Filename of the input image
	 * @param ch		Append the following prefix to the output filename
	 * @param format	Image file format
	 * @return			Name of the created file
	 * @see SaveImageOptions
	 */
	static char* SaveEyeImages(IplImage* img, char* fileName, 
		const char* ch, const char* format);
	
	/** 
	 * Save an IplImage along with the position and sequence info encoded in
	 * the output filename.
	 *
	 * Resulting filename: "<fileName>_F<frame>_<str><num>"
	 *
	 * @param img		Input image
	 * @param fileName	Filename of the input image
	 * @param frame		Frame sequence
	 * @param str		"L" (left) or "R" (right) position 
	 * @param num		Detected eye image sequence
	 * @param totalFrame Total number of frames
	 * @see SaveEyeImages
	 */
	static void SaveImageOptions(IplImage* img, char* fileName, int frame, const char* str, int num, int totalFrame);

	
	/** 
	 * Extract a rectangular part out of an image.
	 *
	 * @note The returned image needs to be deallocated
	 *
	 * @param img	Input image.
	 * @param rect	Rectangle info
	 * @param x		Starting x
	 * @param y		Starting y
	 * @param wd	Width
	 * @param ht	Height
	 * @return		Extracted image.
	 */
	static IplImage* extractImagePart(IplImage* img, CvRect& rect, int x, int y, int wd, int ht);
	
	
	/** 
	 * Return the ROI of an image as IplImage.
	 *
	 * @param eyeImg	Input image.
	 * @param startX	X starting point
	 * @param endX		X end point
	 * @param startY	Y starting point
	 * @param endY		X end point
	 * @return Returns	IPL image of ROI
	 *
	 * @see getROIImage_C
	 */
	static IplImage* getROIImage(IplImage* eyeImg, int startX, int endX, int startY, int endY);
	
	/** 
	 * Returns an ROI of an image as IMAGE.
	 *
	 * @param eyeImg	Input image.
	 * @param startX	X starting point
	 * @param endX		X end point
	 * @param startY	Y starting point
	 * @param endY		Y end point
	 * @return Returns	IMAGE type image of ROI
	 *
	 * @see getROIImage
	 */
	static Masek::IMAGE* getROIImage_C(Masek::IMAGE* eyeImg, int startX, int endX, int startY, int endY);
	
	/** 
	 * Calculates the rectangular info and center point.
	 *
	 * @param eyeImg	Input image.
	 * @param center	Input center point Rectangle info
	 * @param cr		Input radius
	 * @param xLimit	Max. distance of X from the center
	 * @param yLimit	Max. distance of Y from the center
	 * @return			Rect info and center point
	 *
	 * See also SETVALUE
	 */
	static SETVALUE setImage(IplImage* eyeImg, CvPoint center, int cr, int xLimit, int yLimit);
	
	/** 
	 * Calculate the square rectangular info.
	 *
	 * @param img		Input image.
	 * @param x			Input x
	 * @param y			Input y
	 * @param radius		Input radius
	 * @param destVal 	(OUT) Used to return the rect info (0:left, 1:right, 2:bottom, 3:top)
	 */
	static void myRect(IplImage* img, int x, int y, int radius, int* destVal);	
	
	/** 
	 * Calculates the rectangular info with different X and Y radius.
	 *
	 * @param img		Input IPL image.
	 * @param x			Input x
	 * @param y			Input y
	 * @param width		Input X radius
	 * @param height	Input Y radius
	 * @param destVal 	(OUT) Used to return the rect info (0:left, 1:right, 2:bottom, 3:top)
	 */
	static void myXYRect(IplImage* img, int x, int y, int width, int height, int* destVal);

	/** 
	 * Calculates the rectangular info with different X and Y radius.
	 *
	 * @param image		Input IMAGE image.
	 * @param x			Input x
	 * @param y			Input y
	 * @param width		Input X radius
	 * @param height	Input Y radius
	 * @param destVal 	(OUT) Used to return the rect info (0:left, 1:right, 2:bottom, 3:top)
	 */
	static void myXYRect_C(Masek::IMAGE *image, int x, int y, int width, int height, int* destVal);

	/** 
	 * Calculate the square rectangular info of an IMAGE type image.
	 *
	 * @param lidImg	Input image.
	 * @param x			Input x
	 * @param y			Input y
	 * @param radius	Input radius
	 * @param destVal	(OUT) Used to return the rect info (0:left, 1:right, 2:bottom, 3:top)
	 */
	static void myRect_C(Masek::IMAGE *lidImg, int x, int y, int radius, int* destVal);
	
	/** 
	 * Debug values.
	 *
	 * @param value		Actual value.
	 * @param maxSize	Max width or height
	 * @return			Proper value
	 */
	static int getValue(int value, int maxSize);	

	/** 
	 * Time elapsed.
	 *
	 * @param clock1 	Ending time
	 * @param clock2 	Begining time
	 * @return			Difference in ms
	 */
    static double diffclock(clock_t clock1, clock_t clock2);


};

#endif // !IMAGE_UTILITY_H
