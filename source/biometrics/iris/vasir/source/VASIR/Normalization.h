/*********************************************************//**
** @file Normalization.h
** Performs normalization of the iris region by unwraping 
** the circular region into a rectangular block of constant dimensions.
**
** This is a C translation from Masek's matlab code
** Author: Xiaomei Liu (xliu5@cse.nd.edu)
** Computer Vision Research Laboratory
** Department of Computer Science & Engineering, U. of Notre Dame
**
** Author: Original source code by Libor Masek (masekl01@csse.uwa.edu.au), 
** School of Computer Science & Software Engineering
** The University of Western Australia, November 2003
** See "Terms and Conditions" at 
** http://www.csse.uwa.edu.au/~pk/studentprojects/libor/sourcecode.html
**************************************************************/
#ifndef NORMALIZATION_H
#define NORMALIZATION_H

#include "Masek.h"

/**
 * Class for normalizing the iris region.
 */
class Normalization
{
public:

	/**
	* Normalization of the iris region by unwraping 
	* the circular region into a rectangular block of constant dimensions.
	*
	* @param image		Input eye image to extract iris data
	* @param xiris		X coordinate of the circle defining the iris boundary
	* @param yiris		Y coordinate of the circle defining the iris boundary
	* @param riris		Radius of the circle defining the iris boundary
	* @param xpupil		X coordinate of the circle defining the pupil boundary
	* @param ypupil		Y coordinate of the circle defining the pupil boundary
	* @param rpupil		Radius of the circle defining the pupil boundary
    * @param radpixels	Radial resolution, defines vertical dimension of
    *					normalized representation
	* @param angulardiv	Angular resolution, defines horizontal dimension
    *					of normalized representation
	* @param polar_array Output of the polar iris array
	* @param polar_noise Output of the polar noise mask
	*/
	static void normalizeiris(Masek::filter *image, int xiris, int yiris, int riris, 
                          int xpupil, int ypupil, int rpupil,
						  int radpixels, int angulardiv, 
						  Masek::filter *polar_array, Masek::IMAGE *polar_noise);
private:
	/**
	* Round Double and return Int
	*
	* @param x Input double type
	* @return Int type value
	*/
	static int roundND(double x);

	/**
	* INTERP2 2-D interpolation (table lookup).
	* Linear interpolation.
	*
	* ZI = INTERP2(Z,XI,YI) interpolates to find ZI, the values of the
	* underlying 2-D function Z at the points in matrices XI and YI.
	*
	* @author MathWorks, Inc.
	* 
	* @param z  Underlying z-function
	* @param xi Row vector (matrix w/ constant columns)
	* @param yi Column vector (matrix w/ constant rows)
	* @param zi (OUT) Created matrix
	*/
	static void interp2(Masek::filter *z, Masek::filter *xi, Masek::filter *yi, Masek::filter *zi);

	/**
	* Returns the pixel coordinates of a circle defined by the radius and 
	* x, y coordinates of its centre.
	*
	* @param x0      Centre coordinates of the circle (x)
	* @param y0      Centre coordinates of the circle (y) 
	* @param r       The radius of the circle
	* @param imgsize Size of the image array to plot coordinates onto
	* @param _nsides The circle is actually approximated by a polygon, this
	*                argument gives the number of sides used in this 
	*                approximation. Default is 600.
	* @param x       (OUT) An array containing x coordinates of circle 
	*                boundary points
	* @param y       (OUT) An array containing y coordinates of circle 
	*                boundary points
	*/
	static int circlecoords(double x0, double y0, double r, int *imgsize, double _nsides, int **x, int **y);
};
#endif // !NORMALIZATION_H

