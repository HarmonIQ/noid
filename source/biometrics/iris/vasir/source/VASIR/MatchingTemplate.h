/*********************************************************//**
** @file MatchingTemplate.h
** Methods for creating templates.
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

#ifndef MATCHING_TEMPLATE_H
#define MATCHING_TEMPLATE_H


#include "ImageUtility.h"

/**
 * Class to create or instantiate templates.
 */
class MatchingTemplate
{

public:

   MatchingTemplate();
   ~MatchingTemplate();
  
   /**
	* Load an existing template
	*
	* @param templateFileName	Input template file name
	* @return					\c true if the template was loaded successfully
	*/
   bool loadTemplate(const char* templateFileName);
  

   /**
	* Load an existing VASIR's template or creates a new template.
	*
	* @param fileName Input image file name
	* @param dataType Valid \c dataType values:
	*					- \c 1 = classic still image
	*					- \c 2 = video captured at a distance (distant-videos)
	*/
   void loadVASIRTemplate(char* fileName, int dataType);


	/** Returns the angular value of the template. */
  	inline int getNumAngularDivisions() const { return numAngularDivisions; }

  	/** Returns the radial value of the template. */
	inline int getNumRadialDivisions() const { return numRadialDivisions; }

	/** Returns the iris template. */
  	inline int *getIrisTemplatePtr() const { return irisTemplate; }

  	/** Returns the noise mask. */
	inline int *getIrisMaskPtr() const { return irisMask; }

  	int *irisTemplate;
  	int *irisMask;


private:
	int numAngularDivisions;
	int numRadialDivisions;

	/**
	* Create a new VASIR template.
	*
	* @param fileName			Input image file name
	* @param templateFileName	Input template file name
	* @param dataType			Valid \c dataType values:
	*							- \c 1 = classic still image
	*							- \c 2 = video captured at a distance (distant-videos)
	*/
   void createVASIRTemplate(char* fileName, const char* templateFileName, int dataType);
 

    /**
	* Create a new Masek template.
	*
	* @param fileName			Input image file name
	* @param templateFileName	Input template file name
	* @param nScale				Number of scales
	* @param dataType			Valid \c dataType values:
	*							- \c 1 = classic still image
	*							- \c 2 = video captured at a distance (distant-videos)
	*/
   void createMASEKTemplate(char* fileName, const char* templateFileName, 
						int nScale, int dataMode);
};

#endif // !_MATCHING_TEMPLATE_H





 
