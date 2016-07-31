/*********************************************************//**
** @file CreateTemplate.h
** Create iris template.
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
#ifndef CREATE_TEMPLATE_H
#define CREATE_TEMPLATE_H

/**
 * Utility methods to create iris templates.
 */
class CreateTemplate
{	
public:
	/**
	* Create an iris template for matching and non-matching.
	*
	* @param fileName	Input image file name
	* @param template1	(OUT) Returns the iris template
	* @param mask1		(OUT) Returns the iris mask
	* @param width		(OUT) Returns the template width
	* @param height		(OUT) Returns the template height
	* @param dataType	Valid \c dataType values:
	*					- \c 1 = classic still image
	*					- \c 2 = video captured at a distance (distant-videos)
	*/
	static void newCreateIrisTemplate(const char *fileName,										
										int **template1, int **mask1, 
										int *width, int *height, 
										int dataType);

};

#endif // !CREATE_TEMPLATE_H
