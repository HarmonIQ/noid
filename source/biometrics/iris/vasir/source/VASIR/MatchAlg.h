/*********************************************************//**
** @file MatchAlg.h
** Functions to match one template to another template.
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
#ifndef MATCH_ALG_H
#define MATCH_ALG_H

#include "ImageUtility.h"

/*
* Matching of two templates.
*/
class MatchAlg 
{
public:
	/**
	* Match gallery template to probe template.
	*
	* @param galleryName	Input gallery image file name
	* @param probeName		Input probe image file name
	* @param gDataType		Valid \c dataType values:
	*						- \c 1 = classic still image
	*						- \c 2 = video captured at a distance (distant-videos)
	* @param pDataType		Valid \c dataType values:
	*						- \c 1 = classic still image
	*						- \c 2 = video captured at a distance (distant-videos)
	* @return Hamming distance value.
	*/
	static double mainMatchAlg(char *galleryName, char *probeName, int gDataType, int pDataType);
};
#endif // !MATCH_ALG_H
