/*********************************************************//**
** @file Utility.h
** Utility functions
**
** @date 10/2011 (updated 04/2013)
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

#ifndef __Utility_H
#define __Utility_H

#include <iostream>
using namespace std;
#include <vector>
#include <stdio.h>
#include <algorithm>
#include "string.h"

class Utility
{
public:
	Utility(void);
	~Utility(void);

	/**
	 * Read single column from a file and save it into vector.
	 *
	 * @param fname		File name
	 * @param location	Column number where needs to be extracted
	 * @param skip		Number of rows to skip (3)
	 * @param val		(OUT) extracted data from the column
	 */
	static int readSingleColumn(const char* fname, 
								int location, 
								int skip,
								std::vector<float> &val);	

	/**
	 * Read four columns from a file and save it into vectors.
	 *
	 * @param fname		File name
	 * @param location	Starting column number where needs to be extracted
	 * @param skip		Number of rows to skip (3)
	 * @param val1		(OUT) extracted data from the column1
	 * @param val2		(OUT) extracted data from the column2
	 * @param val3		(OUT) extracted data from the column3
	 * @param val4		(OUT) extracted data from the column4
	 */
	static int readFourScores(const char* fname, 
							  int location, 
							  int skip,  
							  std::vector<float> &val1,
							  std::vector<float> &val2,
							  std::vector<float> &val3,
							  std::vector<float> &val4);
	
	/**
	 * Write data.
	 */
	static void appendToFile(const char* outputFileName, 
							 const char* fmt, ...);

	/**
	 * Get partial file name.
	 */
	static char* getFilenamePart(char* buffer);

	/**
	 * \n to \0.
	 */
	static void stripLineBreak(char* buffer);
	
protected:

    /**
	 * give a number of tabs as a option.
	 */
	static char* jumpToColumn(char* buf, 
							int colNo, 
							int tabNum); 
  
	/**
	 * Indicate how many tab or space you used--default: 1.
	 */
	static char* getColumn(char** buf, 
						 int tabNum);
  

};
#endif // !__Utility_H
