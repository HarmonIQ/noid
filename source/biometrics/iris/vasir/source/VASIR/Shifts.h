/*********************************************************//**
** @file Shifts.h
** Shifts the bitwise for correcting rotational inconsistency.
**
** @date 12/2010 (updated 4/2012)
** @author: Reimplemented and modified by Yooyoung Lee
**
** @note Additions and Modifications to existing source code.
**       Also give a credit to:
**       - Libor Masek (masekl01@csse.uwa.edu.au) - Matlab code
**       - Xiaomei Liu (xliu5@cse.nd.edu) - Translated to C code
**
** Note: Please send suggestions/BUG reports to yooyoung@<NOSPAM>nist.gov. 
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

#ifndef SHIFTS_H
#define SHIFTS_H

#include "ImageUtility.h"

class Shifts
{
public:
	Shifts(void);
	~Shifts(void);

	/**
	* Shifts the iris patterns bitwise.
	* In order to provide the best match each shift is by two bits 
	* to the left and to the right.
	*
	* @param templates Input template
	* @param width Input width
	* @param height Input height
	* @param noshifts >0: Number of shifts to the right
	* 				<0: Number of shifts to the left
	* @param nscales  Number of filters used for encoding, needed to
    *                 determine how many bits should be moved when shifting
	* @param templatenew Used to return the shifted template
	*/
	static void X_ShiftBits(int *templates, int width, int height, 
					int noshifts, int nscales, int *templatenew); 
	
	/**
	* Shifts the iris patterns bitwise.
	* In order to provide the best match each shift is one bit up and down.
	*
	* @param templates Input template
	* @param width Input width
	* @param height Input height
	* @param noshifts >0: Number of shifts to the down (two-bit unit)
	* 				<0: Number of shifts to the up (one-bit unit)
	* @param nscales  Number of filters used for encoding, needed to
    *                 determine how many bits should be moved when shifting
	* @param templatenew Used to return the shifted template
	*/
	static void Y_ShiftBits(int *templates, int width, int height, 
					int noshifts,int nscales, int *templatenew);


};
#endif // !SHIFTS_H