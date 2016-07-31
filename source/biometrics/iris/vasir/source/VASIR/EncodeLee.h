/*********************************************************//**
** @file EncodeLee.h
** Methods for generating a biometric template for a normalized iris region.
** This also creates the corresponding noise mask using the masking scheme.
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
#ifndef ENCODE_LEE_H
#define ENCODE_LEE_H

#include "Masek.h"


/**
 * Class for generating an iris template and a noise mask using the masking scheme.
 */
class EncodeLee
{
	public:
	/**
	* Encode the iris pattern using Gabor filter and generate the noise mask using the masking scheme 
	*
	* @param polar_array		Normalized iris region
    * @param noise_array		Corresponding normalized noise region map
	* @param nscales			Number of filters to use in encoding
    * @param minWaveLength	Base wavelength
    * @param mult			Multiplication factor between each filter
    * @param sigmaOnf		Bandwidth parameter
	* @param _template		(OUT) Iris template
	* @param mask			(OUT) Noise mask
	* @param width			(OUT) Width
	* @param height			(OUT) Height
	* @param magLowerThresRate Masking bits based on the low threshold Magnitude value
	* @param magUpperThresRate Masking bits based on the high threshold Magnitude value
	*/
	static void newEncodeLee(Masek::filter *polar_array, 
				   Masek::IMAGE* noise_array,
				   int nscales, 
				   int minWaveLength, 
				   int mult, 
				   double sigmaOnf, 
				   int **_template, 
				   int **mask, 
				   int *width, 
				   int *height,
				   float magLowerThresRate,
				   float magUpperThresRate);

	
private:
	/**
	* Sort complex values and select the threshold to mask noise. 
	*
	* @param E		Input real and imagery coefficients ([h][w] in size)
    * @param w		Width
	* @param h		Height
    * @param rate	Rate to select the threshold
	* @param type	Select either Real (= 1) or Imagery (= 2)
	* @return		Threshold value
	*/
    static double getCoefficientThres(Masek::Complex **E, int w, int h, float rate, int type);

	/**
	* Sort magnitude values and select the threshold - to mask noise.
	*
	* @param E		Input real and imagery coefficient ([h][w] in size)
    * @param w		Width
	* @param h		Height
    * @param rate	Rate to select the threshold
	* @return 		Threshold value
	*/
    static double getMagnitudeThres(Masek::Complex **E, int w, int h, float rate);

	/**
	* @note the following functions:
	*		  - \c fix(.)
	*         - \c fftshift(..)
	*         - \c dft(..)
	*         - \c fft(..)
	*         - \c ifft(..)
	*         - \c gaborconvolve(..)
	*
	* These are a C translation from Masek's matlab code
	* Author: Xiaomei Liu (xliu5@cse.nd.edu)
	* Computer Vision Research Laboratory
	* Department of Computer Science & Engineering, U. of Notre Dame
	*
	* Author: Original 'gaborconvolve' by Peter Kovesi, 2001
	* Heavily modified by Libor Masek (masekl01@csse.uwa.edu.au), November 2003
	* School of Computer Science & Software Engineering
    * The University of Western Australia
	* Modifiedy from http://www.cs.princeton.edu/introcs/97data/FFT.java.html
	*/

	/**
	* Change Double to Int.
	*
	* @param double Input double type
	* @return		Int type
	*/
	static int fix(double);

	/**
	* Shift zero-frequency component to center of spectrum.
    * For vectors, FFTSHIFT(X) swaps the left and right halves of X.  
	* For matrices, FFTSHIFT(X) swaps the first and third quadrants 
	* and the second and fourth quadrants.  
	* For N-D arrays, FFTSHIFT(X) swaps "half-spaces" of X along each dimension.
    * FFTSHIFT(X,DIM) applies the FFTSHIFT operation along the dimension DIM.
	* FFTSHIFT is useful for visualizing the Fourier transform with
    * the zero-frequency component in the middle of the spectrum.
	* See also IFFTSHIFT, FFT
	* Copyright 1984-2000 The MathWorks, Inc.
	* $Revision: 5.8$  $Date: 2000/06/14 21:10:10 
	*/
	static void fftshift(double *x, int numDims, int size[]);
	static void dft(Masek::Complex *x, Masek::Complex *y, int N);
	static Masek::Complex* fft(Masek::Complex* x, int N);
	static Masek::Complex * ifft(Masek::Complex* x, int N);

	/**
	* Function for convolving each row of an image with 1D log-Gabor filters
	*
	* @param im				Input image to convolve
	* @param nscale			Number of filters to use
	* @param minWaveLength	Wavelength of the basis filter
    * @param mult			Multiplicative factor between each filter
	* @param sigmaOnf		Ratio of the standard deviation of the Gaussian describing
	*						the log Gabor filter's transfer function in the frequency
	*						domain to the filter center frequency.
    * @param E0				Output of a 1D cell array of complex valued comvolution results
	* @param filtersum		(OUT) Sum of fiter
	* @param EOh			(OUT) Height
	* @param EOw			(OUT) Width
	*/
	static void gaborconvolve(Masek::filter* im, 
		int nscale, 
		int minWaveLength, 
		int mult, 
		double sigmaOnf, 
		Masek::Complex*** EO, 
		double** filtersum, 
		int *EOh, int *EOw);
};
#endif // !ENCODE_LEE_H
