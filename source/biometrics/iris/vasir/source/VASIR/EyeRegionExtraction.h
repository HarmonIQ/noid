/*********************************************************//**
** @file EyeRegionExtraction.h
** Extract the iris images from the face-visible video frames.
**
** @date 03/2012
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

#ifndef EYE_REGION_EXTRACTION_H
#define EYE_REGION_EXTRACTION_H

#include "ImageUtility.h"

class EyeRegionExtraction
{
public:

	/**
	* Detect the pupil's position for both left and right, align the degree difference 
    * and then extract the eye region.
	*
	* @param currentImg	Input image
	* @param rPupilMax	Pupil maximum radius
	* @param rIrisMax	Iris maximum radius
	* @param ratio4Circle	Select the greater length bewteen width and height under the size condition
	* @param closeItr		Number of iteration for closing
	* @param openItr		Number of iteration for opening
	* @param speed_m	Boost speed (Default=1: larger number is less precise)
	* @param alpha		Additional value for determining threshold
    * @param format		Image format (default: BMP)
	* @param lImg 		(OUT) Aligned left eye image
	* @param rImg 		(OUT) Aligned left eye image
	*/
    static void doExtract(IplImage* currentImg, int rPupilMax, int rIrisMax, double ratio4Circle, int closeItr, int openItr,
						int speed_m, int alpha, double norm, float nScale, const char* format, IplImage*& lImg, IplImage*& rImg);
};
#endif // !EYE_REGION_EXTRACTION_H

