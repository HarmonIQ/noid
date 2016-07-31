#include "FindHighLights.h"
#include "ImageUtility.h"

void FindHighLights::removeHighLights2(Masek::IMAGE* noiseImage, 
									   int eyelashThres, int reflectThres)
{
	// Remove reflections and eyelashes
	for (int i = 0; i<noiseImage->hsize[0]*noiseImage->hsize[1]; i++)
	{
		if (noiseImage->data[i] < eyelashThres 
			|| noiseImage->data[i] > reflectThres)
		{
			// Noise pixels will have NaN values
			noiseImage->data[i] = (unsigned char)sqrt((double)-1); 
		}
	}
}


