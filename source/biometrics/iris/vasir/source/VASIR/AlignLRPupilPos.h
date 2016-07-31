/*********************************************************//**
** @file AlignLRPupilPos.h
** Functions for aligning the left and right eye position.
**
** @date 10/2011 (updated 04/2012)
** @author Yooyoung Lee 
**
** Note: Please send suggestions/BUG reports to yooyoung@<NOSPAM>nist.gov
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
#ifndef ALIGN_LR_PUPIL_POS_H
#define ALIGN_LR_PUPIL_POS_H

#include "ImageUtility.h"


class AlignLRPupilPos
{
public:
	AlignLRPupilPos(void);
	~AlignLRPupilPos(void);

	/**
	* Determine the degree difference between the left and the right eye based the pupil center info.
	* Extracts the left ("_L") and right ("_R") eye as files.
	*
	* @param img		Input image	
    * @param rIrisMax	Maximum radius of a potential pupil circle
	* @param circle1 	(OUT) Returns the right pupil info
    *     (Biggest circle-> 0:x, 1:y, 2:radius, Secondary circle->3:x, 4:y, 5:radius,)
	* @param circle2 	(OUT) Returns the left pupil info
    *     (Biggest circle-> 0:x, 1:y, 2:radius, Secondary circle->3:x, 4:y, 5:radius,)
	* @param lImg 		(OUT) Returns the left iris image
   	* @param rImg	 	(OUT) Returns the right iris image
    */
	static void alignDegreeDiff(IplImage* img, 										  
                                      int rIrisMax,
									  int* circle1, int* circle2,
									  IplImage*& lImg, IplImage*& rImg);

	/**
	* Rotate an image based on a specified angle.
	*	
	* @param eyeImg		Input image
	* @param angle		Degree difference
	* @return (OUT)		Rotated image
	*/
	static IplImage* rotate(IplImage* eyeImg, double angle);

	/**
	* Get the angle difference between the right and left pupil position.
	*	
	* @param px1		Right puil center X
	* @param py1		Right puil center Y
	* @param px2		Left puil center X
	* @param py2		Left puil center Y
	* @return (OUT)		Angle
	*/
	static double getAngle(int px1, int py1, int px2, int py2);
};
#endif // !ALIGN_LR_PUPIL_POS_H

