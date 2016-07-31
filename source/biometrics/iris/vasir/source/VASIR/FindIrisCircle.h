/*********************************************************//**
** @file FindIrisCircle.h
** Find the iris circle using Hough Transform.
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

#ifndef FIND_IRIS_CIRCLE_H
#define FIND_IRIS_CIRCLE_H

#include <opencv2/core/core.hpp>

/**
 * Iris boundaries detection class.
 */
class FindIrisCircle
{
public:

  /**
	* Detect the iris boundary using Hough Transform.
	*
	* @param img				Input image
	* @param rPupil			Radius of the pupil circle
	* @param uIrisRadius		Maximum radius
	* @param scaling			Scaling factor to speed up the Hough transform
	* @param lowThres		Threshold for connected edges
	* @param highThres		Threshold for creating edge map
	* @param destVal 		(OUT) Iris center points and radius (0:x, 1:y, 2:radius)
	*/
	static void doDetect(IplImage *img, int rPupil, int uIrisRadius, 
		double scaling, double lowThres, double highThres,
		int* destVal);
	
	/**
	* Find the iris center points. 
    * The distance between pupil and iris center points is constained.
	*
	* @param xyPupil		Pupil's center
	* @param xyIris		Iris' center
	* @param setPt		Points out of the ROI (Region Of Interest)
    * @param val 		Maximum distance between pupil and iris center position
	* @param dataType	Valid \c dataType values:
	*					- \c 1 = classic still image
	*					- \c 2 = video captured at a distance (distant-videos)
	* @return 			Original coordinates for pupil circle
	*/
	static CvPoint getOriginPoints(CvPoint xyPupil, CvPoint xyIris, CvPoint setPt, int val); 
};
#endif // !FIND_IRIS_CIRCLE_H
