/*********************************************************//**
** @file GetHammingDistance.h
** Methods for calculating the Hamming Distance (HD).
**
** @date 09/2011 (updated 04/2012)
** @author: Reimplemented and heavily modified by Yooyoung Lee
** Note: Please send suggestions/BUG reports to yooyoung@<NOSPAM>nist.gov. 
** For more information, refer to: http://www.nist.gov/itl/iad/ig/vasir.cfm
**          
** @note Additions and Modifications to existing source code.
**
** Author: C translation from Masek's matlab code by Xiaomei Liu (xliu5@cse.nd.edu)
** Computer Vision Research Laboratory
** Department of Computer Science & Engineering, U. of Notre Dame
**
** Author: Original source code by Libor Masek (masekl01@csse.uwa.edu.au), 
** School of Computer Science & Software Engineering
** The University of Western Australia, November 2003
** See "Terms and Conditions" at 
** http://www.csse.uwa.edu.au/~pk/studentprojects/libor/sourcecode.html
**************************************************************/

#ifndef GET_HAMMING_DISTANCE_H
#define GET_HAMMING_DISTANCE_H

#include "MatchingTemplate.h"

/**
 * Class for calculating the Hamming Distance between two iris templates.
 *
 * The class involves the noise masks, i.e. noise bits are not used when 
 * calculating the HD.
 */
class GetHammingDistance 
{
 public:
	 /**
	 * Initialize
	 *@param x Horizontal shift
	 *@param y Vertical shift
	 */
	GetHammingDistance(int x, int y);
	virtual ~GetHammingDistance();

	/**
	* Calculate HD by shifting left and right and then returning the minimum 
	* HD value.
	*
	* @param classTemplate1	Input target template
	* @param classTemplate2	Input query template
	* @param scales			Number of filters used for encoding - needed to
    *						determine how many bits should be moved when shifting.
	* @return 				HD Minimum
	*/
	double computeHDX(const MatchingTemplate*,
					  const MatchingTemplate*,
					  int);
	/**
	* Calculate HD by shifting up and down and returning the minimum HD value.
	*
	* @param classTemplate1 Input target template
	* @param classTemplate2 Input query template
	* @param scales			Number of filters used for encoding - needed to
    *						determine how many bits should be moved when shifting.
	* @return				HD Minimum
	*/
	double computeHDY(const MatchingTemplate*,
					  const MatchingTemplate*,
					  int); 

	/**
	* Calculate HD by shifting towards left/right or up/down
	* and returning minimum HD value.
	*
	* @param classTemplate1 Input target template
	* @param classTemplate2 Input query template
	* @param scales			Number of filters used for encoding - needed to
    *						determine how many bits should be moved when shifting.
	* @return 				HD minimum
	*/
	double computeHDXorY(const MatchingTemplate*,
					   const MatchingTemplate*,
					   int); 

	/**
	* Calculate HD by shifting towards left/right and up/down
	* and returning minimum HD value.
	*
	* @param classTemplate1 Input target template
	* @param classTemplate2 Input query template
	* @param scales			Number of filters used for encoding - needed to
    *						determine how many bits should be moved when shifting.
	* @return 				HD minimum
	*/
	double computeHDXandY(const MatchingTemplate*,
						   const MatchingTemplate*,
						   int); 


 private:

	int width;
	int height;
	int maxShiftX;
	int maxShiftY;
  
	int *template1s;
	int *mask1s;
	int *template2s;
	int *mask2s;

	int *mask;
	int *C;

	/**
	* Initialize the given templates.
	*
	* @param classTemplate1 Input target template
	* @param classTemplate2 Input query template
	*/
	void initializeTemplates(const MatchingTemplate*,
						     const MatchingTemplate*);

	/**
	* Calculates HD.
	*
	* @param tTemplate Input target template
	* @param tMask Input target mask incorperating with noise
	* @param qTemplate Input query template
	* @param qMask Input query mask incorperating with noise
	* @param newTemplate Input new template
	* @param newMask Input new mask incorperating with noise
	* @return HD minimum
	*/

    double calcHD(int* tTemplate, int* tMask, int* qTemplate, int* qMask, 
				int* newTemplate, int* newMask);


	
}; 
#endif // !GET_HAMMING_DISTANCE_H
