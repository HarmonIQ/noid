/*********************************************************//**
** @file FindPupilCircleNew.h
** Static methods for pupil boundary detection.
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

#ifndef FIND_PUPIL_CIRCLE_NEW_H
#define FIND_PUPIL_CIRCLE_NEW_H

#include <opencv2/core/core.hpp>

/**
 * Pupil boundary detection class.
 */
class FindPupilCircleNew
{
public:
	/**
	* Detects pupil boundaries using contour points.
	*
	* @param img			Input image
	* @param limitRadius	Maximum radius of potential pupil circle
	* @param ratio4Circle	Select the greater length bewteen width and height under the size condition
	* @param closeItr		Number of iteration for closing
	* @param openItr		Number of iteration for opening
	* @param m				Boost speed 
	* 						(Default=1: When increased this method is faster, 
	* 						but the results gets slightly worse)
	* @param alpha			Additional value for determining threshold
	* @param nScale			Valid \c scales values:
	*						- \c 1 = if \c img is smaller or equal than 600x440px
	*						- \c 2 = if \c img is greater than 600x440px
	* @param destVal			(OUT) Pupil circle info
	*/
	static void doDetect(IplImage* img, 
						int limitRadius, 
						double ratio4Circle, 
						int closeItr, 
						int openItr,
						int m, 
						int alpha,
						double norm,
						float nScale, 
						int* destVal);	


	/**
	* Detect the pupil's center for both left and right.
	*
	* @param img		Input image
	* @param rPupilMax	Pupil maximum radius
	* @param space		Width of the nose bridge	
	* @param ratio4Circle	Select the greater length bewteen width and height under the size condition
	* @param closeItr		Number of iteration for closing
	* @param openItr		Number of iteration for opening
	* @param speed_m		Boost speed (Default=1: larger number is less precise)
	* @param alpha		Additional value for determining threshold
	* @param nScale		Valid \c scales values:
	*					- \c 1 = if \c img is smaller than 600x440px
	*					- \c 2 = if \c img is greater than 600x440px
	* @param circle1 	(OUT) Right pupil info
	* @param circle2 	(OUT) Left pupil info
	*/
	static void doDetectTwoPupils(IplImage* img, 
									int rPupilMax, 
									const int space,
									double ratio4Circle,
									int closeItr,
									int openItr,
									int speed_m,
									int alpha,
									double norm,
									float nScale,
									int* circle1, 
									int* circle2);

private:

	/**
	* Determine the threshold using standard deviation.
	*
	* @param img	Input image
	* @param minVal Minimum intensity within an image
	* @param alpha	Additional value for threshold
	* @return 		Threshold
	*/
	static int getThreshold(IplImage* img, int minVal, int alpha, double norm);

	/**
	* Find the pupil's location by thresholding image
	*
	* @param grayImg		Input image
	* @param closeItr		Number of iteration for closing
	* @param openItr		Number of iteration for opening
	* @param threshold		Threshold value
	* @param limitRadius	Maximum radius of a potential pupil circle 	
	* @param ratio4Circle	Select the greater length bewteen width and height under the size condition
	* @param nScale			Valid \c scales values:
	*						- \c 1 = if \c img is smaller than 600x440px
	*						- \c 2 = if \c img is greater than 600x440px
	* @param circles 		(OUT) Pupil circle info
    * 		(Biggest circle-> 0:x, 1:y, 2:radius, Secondary circle->3:x, 4:y, 5:radius,)
	*/
	static void getCoordinates(IplImage* grayImg, int closeItr, int openItr,
		int threshold, int limitRadius, 
		double ratio4Circle, float nScale, int* circles);
	
	/**
	* Return the radius of a pupil circle.
	*
	* @param width			Image width
	* @param height			Image height
    * @param limitRadius	Maximum radius of a potential pupil circle
	* @param ratio4Circle	Select the greater length bewteen width and height under the size condition
	* @return				Circle radius
	*/
	static int getRadius(int width, int height, int limitRadius, double ratio4Circle);
	
	/**
	* Determine the pupil center-points and radius.
	*
	* @param contour		Sequences of points defining a curve within an image
	* @param minCount		Minimum contours
	* @param maxCount		Maximum contours
	* @param limitRadius	Maximum radius of pupil circle
	* @param ratio4Circle	Select the greater length bewteen width and height under the size condition
	* @param circles 		(OUT) Pupil circle info
    * 		(Biggest circle-> 0:x, 1:y, 2:radius, Secondary circle->3:x, 4:y, 5:radius,)
    */
	static void getPupilPosition(CvSeq* contour, int minCount, int maxCount, 
						int limitRadius, double ratio4Circle, int *circles);

	/**
	* Determines the maximum contour count.
	*
	* @param contour	Contour sequences of points defining a curve within an image
	* @param count		(OUT) Maximum and second maximum contour point
	*/
	static void getMaxCount(CvSeq* contour, int* count);

};
#endif // !FIND_PUPIL_CIRCLE_NEW_H
