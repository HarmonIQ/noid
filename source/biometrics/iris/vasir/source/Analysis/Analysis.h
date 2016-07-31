/*********************************************************//**
** @file Analysis.h
** Functions for calculating scores
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

#ifndef __ANALYSIS_H
#define __ANALYSIS_H

#include <opencv2/core/core.hpp>

#include <string>
#include <vector>
#include <stdio.h>
#include <stdlib.h>
#include <stdarg.h>
using namespace std;


#define FN_BUFFER_SIZE		2048
#define NUM_RANDOM_PROBES	50
#define SUBJECT_ID_LEN		5

class EyeDetection;

class Analysis
{
public:


	/**
	* Generate a new file name.
	*
	* @param outputFName	(OUT) image file name
	* @param index   		Experiment tag number
	* @param ch				Input text("_Matching" or "_Nonmatching")
	* @param format			File extension (e.g., bmp or tiff)	
	*/
	static char* newFileName(const char* outputFName, 
							int index, 
							const char* ch, 
							const char* format);

	/**
	* Get HD scores.
	*
	* @param galleryName	Input gallery image file name
	* @param probeName		Input probe image file name
	* @param gDataType		Valid \c dataType values:
	*						- \c 1 to 7
	* @param pDataType		Valid \c dataType values:
	*						- \c 1 to 7
	* @param hd				(OUT) return Hamming distance value.
	*/
	static void loadHammingDistance(char* galleryName, 
									char* probeName, 
									int gDataType, 
									int pDataType, 
									double & hd);

	/**
	* Get matching scores
	*
	* @param gallery_Img_List	Input image list file name
	* @param probe_Img_List		Input image list file name
	* @param outputFName		Output matching scores list file name
	* @param path				File path
	* @param pos			Valid \c pos values:
	*						- \c 1: Left
	*						- \c 2: Right	
	* @param gDataType		Valid \c dataType values:
	*						- \c 1 to 7
	* @param pDataType		Valid \c dataType values:
	*						- \c 1 to 7
	*/
	static void doNewMatchListFile(const char* gallery_Img_List, 
								   const char* probe_Img_List, 
								   const char* outputFName, 
							       string path,
							       int pos,
								   int gDataType, 
								   int pDataType);


	/**
	* Get nonmatching scores
	*
	* @param gallery_Img_List	Input image list file name
	* @param probe_Img_List		Input image list file name
	* @param outputFName		Output matching scores list file name
	* @param path				File path
	* @param gDataType		Valid \c pos values:
	*						- \c 1: Left
							- \c 2: Right	
	* @param gDataType		Valid \c dataType values:
	*						- \c 1 to 7
	* @param pDataType		Valid \c dataType values:
	*						- \c 1 to 7
	*/
	static void doNewRandomNonMatch(const char* gallery_Img_List, 
									const char* probe_Img_List, 
									const char* outputFName, 
									string path, 
									int pos, 
									int gDataType, 
									int pDataType);


	/**
	* Write the scores
	*/
	static void doWriteHD(const char* outputFName, 
						  char *gFullName, 
						  char *pFullName,
						  const char *gPartName, 
						  const char *pPartName,
						  const char *gSubjectID, 
						  const char *pSubjectID,
						  int pos,
						  int gDataType, 
						  int pDataType, 
						  int matchType);


	private:
			/**
	* Load new titls for the score list file
	*/
	static void loadNewTitle(const char* outputFName);
	/**
	* Generate new vectors for files
	*/
	static void loadNewVectors(const char* imgFileList,
							string path,
							vector<string> &VecFullPathName,
							vector<string> &VecImgFileName,
							vector<string> &vecSubjectID);


};
#endif  // !__Analysis_H__
