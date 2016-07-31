/*********************************************************//**
** @file EyeDetection.h
** Eye region detection.
**
** @date 10/2010 (updated 03/2012)
** @author Yooyoung Lee 
**
** Note: Please send BUG reports to yooyoung@<NOSPAM>nist.gov
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

#ifndef EYE_DETECTION_H
#define EYE_DETECTION_H

#include <opencv2/core/core.hpp>
#include <opencv2/objdetect/objdetect.hpp>
#include <opencv2/highgui/highgui.hpp>

/**
 * Eye region detection class.
 */
class EyeDetection 
{
public: 

 /**
  * \struct Type to return the detection result  
  */
  typedef struct 
  {
	/// The rectangle of left and right eye, and eye pair
    CvRect leftRect, rightRect, bothRect;

    /// Left, right, and eye pair images
	IplImage* leftImg, *rightImg, *bothImg;
  } RESULT; //Returns multiple values

  typedef struct 
  {
	CvPoint prevCenter;
	int prevRadius;
  }CIRCLES; //Returns the center position and radius

  /**
  * Initializes the eye detection and loads a HaarClassifier Cascade.
  *
  * @note Writes an error message on \c cout in case of an error
  *
  * @param cascadeFileName Location of the cascade file
  */
  EyeDetection(const char* cascadeFileName);

   /**
  * Frees all previously allocated resources.
  */
  virtual ~EyeDetection();


  /** 
  * Detects both of left and right eye using Haar features.
  *
  * Valid \c scales values:
  * - \c 1 = 512x512px
  * - \c 2 = 1024x1024px
  * - \c 4 = 2048x2048px (MBGC dataset)
  *
  * @Note Depending on which cascade is used, both eyes as one region or only 
  *    one eye region can be detected
  *
  * @param img		Input image
  * @param scales	Consider a detected image below the given size noise
  * @param val		Adjust the rectangle size
  * @param w		Set the minimum width size of the two eye regions
  * @param h		Set the minimum height size of the two eye regions
  * @return			Returns the detected eye region (info, image) for left, right, 
  *					both left and right, or \c NULL in case nothing was found 
  */
  RESULT* detect(IplImage* img, int scales, int val, int w, int h); 
  

private:
  // Temporary space for calculations
  CvMemStorage* storage;
  // A new Haar classifier
  CvHaarClassifierCascade* cascade;     
  // To save the results
  RESULT result;  

   
};

#endif // !EYE_DETECTION_H

