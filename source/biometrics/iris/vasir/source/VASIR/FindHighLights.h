/*********************************************************//**
** @file FindHighLights.h
** Static methods for thresholding eyelash and reflection detection.
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

#ifndef FIND_HIGHLIGHTS_H
#define FIND_HIGHLIGHTS_H

#include <opencv2/core/core.hpp>
#include "Masek.h"

/**
 * Eyelash and reflection detection class.
 */
class FindHighLights
{
public:

	/**
	* Remove eyelashes and reflections within the iris region.
	*
	* @param noiseImage		Input Image
    * @param eyelashThres	Threshold value for eyelashes
	* @param reflectThres	Threshold value for reflections
	* @return Image 			Resulting image after removing both eyelashes and reflections
	*/
	static void removeHighLights2(Masek::IMAGE* noiseImage, int eyelashThres, int reflectThres);

};

#endif // !FIND_HIGHLIGHTS_H 

