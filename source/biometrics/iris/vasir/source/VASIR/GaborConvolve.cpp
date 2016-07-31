/*********************************************************//**
** @date 12/2012 
** @author: Memory leaks were fixed by Yooyoung Lee
** Note: Please send suggestions/BUG reports to yooyoung@<NOSPAM>nist.gov. 
** For more information, refer to: http://www.nist.gov/itl/iad/ig/vasir.cfm
**          
**************************************************************/
#include "EncodeLee.h"
#include <math.h>
#include <string.h>

int EncodeLee::fix(double x)
{
	int ret;	
	ret = (int)x;	
	return ret;
}

void EncodeLee::fftshift(double *x, int numDims, int size[])
{
	int k;
	int count;
	int i, m, p;
	double *y;
	int *idx;    
	
	count = 0;

	y = (double*)malloc(sizeof(double)*size[0]*size[1]);
	memcpy(y, x, sizeof(double)*size[0]*size[1]);
	idx = (int*)malloc(sizeof(int)*size[0]*size[1]);

    for (k = numDims-1; k<numDims; k++)
	{
        m = size[k];
        p = (int) ceil(((double)m)/2);
		for (i = p+1; i<=m; i++)
			idx[count++] = i-1;
		
		for (i = 1; i<=p; i++)
			idx[count++] = i-1;				 
	}

    // Use comma-separated list syntax for N-D indexing.
	for (i = 0; i<count; i++)
		x[i] = y[idx[i]];

	free(y);
	free(idx);
}

void EncodeLee::dft(Masek::Complex *x, Masek::Complex *y, int N)
{
    Masek::Complex wk, mul;
	double kth;
	int k, j;
	
	// base case
	if (N ==1)
	{
		y[0].real = x[0].real;
		y[0].img = x[0].img;
	}
     
    for (k = 0; k < N; k++) 
	{
		for (j = 0; j<N; j++)
		{
           kth = -2 * k *j*PI / N;
			wk.real = cos(kth);
			wk.img = sin(kth);
            
			mul.real = wk.real*x[j].real-wk.img*x[j].img;
			mul.img = wk.real*x[j].img+wk.img*x[j].real;

			y[k].real += mul.real;
			y[k].img += mul.img;            
		}    
	}		
}

Masek::Complex* EncodeLee::fft(Masek::Complex* x, int N)
{
    Masek::Complex *y;
    Masek::Complex *even, *odd, *q, *r, wk;
	double kth;
    Masek::Complex mul;
	int k;
		
    y = (Masek::Complex*)malloc(sizeof(Masek::Complex)*N);
    memset(y, 0, sizeof(Masek::Complex)*N);
    
	// base case
	if (N ==1)
	{
		y[0].real = x[0].real;
		y[0].img = x[0].img;
		return y;
	}
     
    // radix 2 Cooley-Tukey FFT
    if (N % 2 != 0)
	{
	//	printf("N is not a power of 2");
		dft(x, y, N);
		return y;
	}

    even = (Masek::Complex*)malloc(sizeof(Masek::Complex)*N/2);
    odd = (Masek::Complex*)malloc(sizeof(Masek::Complex)*N/2);
	
	for (k = 0; k < N/2; k++) 
	{
		even[k].real = x[2*k].real;
		even[k].img = x[2*k].img;
	}

    for (k = 0; k < N/2; k++) 
	{
		odd[k].real  = x[2*k + 1].real;
		odd[k].img  = x[2*k + 1].img;
	}

    q = fft(even, N/2);
    r = fft(odd, N/2);

    for (k = 0; k < N/2; k++) 
	{
            kth = -2 * k * PI / N;
			wk.real = cos(kth);
			wk.img = sin(kth);
            
			mul.real = wk.real*r[k].real-wk.img*r[k].img;
			mul.img = wk.real*r[k].img+wk.img*r[k].real;

			y[k].real = q[k].real+mul.real;
			y[k].img = q[k].img+mul.img;

			y[k+N/2].real = q[k].real-mul.real;
			y[k+N/2].img = q[k].img-mul.img;
            
	}    

	free(q);
	free(r);
	free(even);
	free(odd);
        
    return y;
}

// compute the inverse FFT of x[], assuming its length is a power of 2
Masek::Complex * EncodeLee::ifft(Masek::Complex* x, int N) 
{
    Masek::Complex *y;
	int i;

	// Take conjugate
    for ( i = 0; i < N; i++)
	{
        x[i].real = x[i].real;
		x[i].img = -x[i].img;
	}

    // Compute forward FFT
    y = fft(x, N);

    // Take conjugate again
    for ( i = 0; i < N; i++)
	{
        y[i].real = y[i].real;
		y[i].img = -y[i].img;
	}	

    // Divide by N
    for ( i = 0; i < N; i++)
	{
        y[i].real = y[i].real/ N;
		y[i].img = y[i].img/ N;
	}

    return y;
}

void EncodeLee::gaborconvolve(Masek::filter* im, int nscale, int minWaveLength, 
							 int mult, double sigmaOnf, 
							 Masek::Complex*** EO, double** filtersum, int *EOh, int *EOw)
{
	int rows, cols;
	int ndata;
	double *logGabor, *radius, *m_filter;
	int wavelength;
	int i, j, s, r;
	double fo, rfo;
    Masek::Complex *ft, *imagefft, *signal;
	int size[2];

	rows = im->hsize[0];
	cols = im->hsize[1];

	ndata = cols;
	if (ndata/2 == 1)             // If there is an odd No of data points 
		ndata = ndata-1;          // throw away the last one.


	radius = (double*)malloc(sizeof(double)*(fix(ndata/2)+1));
	
	for (i = 0; i<fix(ndata/2)+1; i++)
	{
		radius[i] = ((double)i)/fix(ndata/2)/2;
	}
	
	radius[0] = 1;

	wavelength = minWaveLength;        // Initialize filter wavelength.

	logGabor = (double*)malloc(sizeof(double)*ndata);
	memset(logGabor, 0, sizeof(double)*ndata);

	*filtersum = (double*)malloc(sizeof(double)*ndata);
	memset(*filtersum, 0, sizeof(double)*ndata);

    imagefft = (Masek::Complex*)malloc(sizeof(Masek::Complex)*ndata);

	for (s = 0; s<nscale; s++)// For each scale.  
	{
        // Construct the filter - first calculate the radial filter component.
		fo = 1.0/wavelength;// Centre frequency of filter.
		rfo = fo/0.5;// Normalised radius from centre of frequency plane 
		// corresponding to fo.
		for (j = 0; j<ndata/2+1; j++)
		{
			logGabor[j] = exp(-log(radius[j]/fo)*log(radius[j]/fo)/(2*log(sigmaOnf)*log(sigmaOnf)));
			
		}
		
		logGabor[0] = 0;  
    
		m_filter = logGabor;    

		for (j = 0; j<ndata/2+1; j++)
		{
			(*filtersum)[j] += m_filter[j];			
		}
		    
		// For each row of the input image, do the convolution, back transform
        signal = (Masek::Complex*)malloc(sizeof(Masek::Complex)*ndata);
        EO[s] = (Masek::Complex**)malloc(sizeof(Masek::Complex*)*rows);
		for (r = 0; r<rows; r++)// For each row
		{
			for (j = 0; j<ndata; j++)
			{
				signal[j].real = im->data[r*im->hsize[1]+j];
				signal[j].img = 0;
			}
			// Fast Fourier Transform			
			ft = fft( signal, ndata);


			// Appling Gabor filter
			for (j = 0; j<ndata; j++)
			{
				imagefft[j].real = ft[j].real*m_filter[j];
				imagefft[j].img = ft[j].img*m_filter[j];
			}		
			// Invers Fast Fourier Transform
			// Save the ouput for each scale			                
			EO[s][r] = ifft(imagefft, ndata);   
			free(ft);//Lee added
		}
		wavelength = wavelength * mult;// Finally calculate Wavelength of next filter*/
		//Lee added
		free(signal);
		
	}//... and process the next scale

	size[0] = 1;
	size[1] = cols;
	fftshift(*filtersum, 2, size);

	*EOh = rows;
	*EOw = ndata;

	free(logGabor);
	free(radius);
	free(imagefft);
}
