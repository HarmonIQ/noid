#include "EncodeLee.h"
#include <iostream>
#include <vector>
#include <algorithm>
#include <string.h>
#include <math.h>
#include "ImageUtility.h"

#define REAL   1
#define IMG    2

/**
 * Fagile bits approach.
 */
double EncodeLee::getCoefficientThres(Masek::Complex **E, int w, int h,
									 float rate, int type)
{
	std::vector<double> vecArray1;
	std::vector<double> vecArray2;
	for (int i=0; i<h; i++)
	{
		for (int j=0; j<w; j++)
		{
			double coef_real = E[i][j].real;
			double coef_img = E[i][j].img;
			vecArray1.push_back(abs(coef_real));
			vecArray2.push_back(abs(coef_img));
		}
	}
	sort(vecArray1.begin(), vecArray1.end());
	sort(vecArray2.begin(), vecArray2.end());
	int index = (int)(vecArray1.size() * rate);//the real and img are the same size
	if(type == REAL)
		return vecArray1[index];
	else if(type == IMG)
		return vecArray2[index];
	else
		return 0;
}

/**
 * VASIR approach.
 */
double EncodeLee::getMagnitudeThres(Masek::Complex **E, int w, int h, float rate)
{
	std::vector<double> vecArray;
	for (int i=0; i<h; i++)
	{
		for (int j=0; j<w; j++)
		{
			double val = sqrt(E[i][j].img*E[i][j].img+E[i][j].real*E[i][j].real);
			vecArray.push_back(val);
		}
	}
	sort(vecArray.begin(), vecArray.end());
	int index = (int)(vecArray.size() * rate);
	if(index != 0)
		return vecArray[index-1];
	else
		return vecArray[0];
}

void EncodeLee::newEncodeLee(Masek::filter *polar_array, 
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
				   float magUpperThresRate)
{
    Masek::Complex *** EO, **E1;
	double *filtersum;
	int i, j;
	int length, length2;
	int k;
	int *H1, *H2, *H3;
	int lenw, lenh;
	double ja;

	// Convolve normalized region with Gabor filters
    EO = (Masek::Complex***) malloc(sizeof(Masek::Complex**)*nscales);

	gaborconvolve(polar_array, nscales, minWaveLength, mult, sigmaOnf, EO, &filtersum, &lenh, &lenw);

	length = polar_array->hsize[1]*2*nscales;

	*_template = (int*)malloc(sizeof(int)*length*polar_array->hsize[0]);
	memset(*_template, 0, sizeof(int)*length*polar_array->hsize[0]);

	*mask = (int*)malloc(sizeof(int)*length*polar_array->hsize[0]);
	memset(*mask, 0, sizeof(int)*length*polar_array->hsize[0]);

	length2 = polar_array->hsize[1];

	H1 = (int*)malloc(sizeof(int)*lenw*lenh);
	H2 = (int*)malloc(sizeof(int)*lenw*lenh);
	H3 = (int*)malloc(sizeof(int)*lenw*lenh);

	for (k=0; k<nscales; k++)
	{
	    
		E1 = EO[k];
		// Mask insignificant bits based on wavelet magnitude values
		double magLowerThreshold = getMagnitudeThres(E1, lenw, lenh, magLowerThresRate);//0% or 20%
		double magUpperThreshold = getMagnitudeThres(E1, lenw, lenh, magUpperThresRate);

		// Phase quantization
		for (i = 0; i<lenh; i++)
		{
			for (j = 0; j<lenw; j++)
			{
				if (E1[i][j].real > 0)
					H1[i*lenw+j] = 1;
				else
					H1[i*lenw+j] = 0;

				if (E1[i][j].img > 0)
					H2[i*lenw+j] = 1;
				else
					H2[i*lenw+j] = 0;   
	    
				// If amplitude is less than the lowthreshold or larger than highthreshold,
				// then phase data is not useful, so flag in the noise mask			
				if (sqrt(E1[i][j].img*E1[i][j].img+E1[i][j].real*E1[i][j].real)< magLowerThreshold
					|| sqrt(E1[i][j].img*E1[i][j].img+E1[i][j].real*E1[i][j].real)> magUpperThreshold)//origin thres: 0.0001
				{
					H3[i*lenw+j] = 1;					
				}
				else
				{
					H3[i*lenw+j] = 0;
				}

			}	
			free(E1[i]);			
		}
        free(E1);



    for (i=0; i<length2;i++)
	{                
        ja = (double)(2*nscales*(i));
		for (j = 0; j<polar_array->hsize[0]; j++)
		{
            // Construct iris template
			(*_template)[j*length+(int)ja+(2*k)] = H1[j*polar_array->hsize[1]+i];
			(*_template)[j*length+(int)ja+(2*k)+1] = H2[j*polar_array->hsize[1]+i];

			// Create noise mask		
			(*mask)[j*length+(int)ja+(2*k)] = noise_array->data[j*noise_array->hsize[1]+ i] || H3[j*polar_array->hsize[1]+i];
			(*mask)[j*length+(int)ja+(2*k)+1] =   noise_array->data[j*noise_array->hsize[1]+ i] || H3[j*polar_array->hsize[1]+i];
		}        
	} 
}

free(EO);

*height = polar_array->hsize[0];
*width = length;

free(filtersum);
free(H1);
free(H2);
free(H3);

}

