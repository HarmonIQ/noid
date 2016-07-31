/*********************************************************//**
** @file ImageSource.h
** ImageSource interface.
** 
** @date 10/2010
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
***************************************************************/
#ifndef CV_IMAGE_SOURCE_H
#define CV_IMAGE_SOURCE_H

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/video.hpp>

/**
 * Abstract class that defines the methods an image source, i.e. provider,
 * needs to implement.
 */
class ImageSource
{
public:
  /**
   * This method should returns a single sentence describing the image source.
   */
  virtual const char* getDescription() const = 0;

  /**
   * This method should return the number of available images.
   *
   * @return Number of available images (>= 0)
   */
  virtual int getNumberOfImages() const = 0;

   /**
   * This method should return \c true in case there are more images that can
   * be fetched using getNextImage()
   *
   * @return \c true = one or more images available, \c false = no images
   */
  virtual bool hasNextImage() = 0;

  /**
   * This method should return the next image.
   *
   * @note The deallocation of the returned image is up to the program.
   *
   * @return Next image, or \c NULL in case of an error or if there is not any
   *    image left for processing
   */
  virtual IplImage* getNextImage() = 0;
};

/**
 * Abstract class to simplify some OpenCV functionality.
 */
class CVImageSource : public ImageSource
{
public:  
	CVImageSource();
	virtual ~CVImageSource();

	/** 
	* Extracts the next frame out of the stream and returns it as an image.
	*
	* return Image
	*/
	virtual IplImage* getNextImage();
	
protected:
	CvCapture* capture;
private: 
	IplImage*  copy;
};

/**
 * Webcam as ImageSource.
 */
class CVCameraSource : public CVImageSource
{
public:
	CVCameraSource();
	virtual ~CVCameraSource();

	const char* getDescription() const;

	 /**
	* Since a webcam delivers a continuous stream, this function will always
	* return \c -1.
	*
	* @return \c -1
	*/
	int getNumberOfImages() const { return -1; };
	
	 /**
	* Always returns \c true.
	*
	* @return \c true
	* @see getNumberOfImages
	*/
	bool hasNextImage() { return true; };
};


/**
 * Video file as ImageSource.
 */
class CVVideoSource : public CVImageSource
{
public:

	 /**
	* Loads a video file.
	*
	* @param fileName location of a video file
	*/
	CVVideoSource(const char* fileName);
	virtual ~CVVideoSource();

	const char* getDescription() const;

	/**
   * Returns the total number of frames.
   *
   * @return Total number of frames (>= 0)
   */
	int getNumberOfImages() const;
	bool hasNextImage();

private:
    int numFrames;
};

#endif // !CV_IMAGE_SOURCE_H
