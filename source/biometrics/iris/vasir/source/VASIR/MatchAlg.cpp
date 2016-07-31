#include "MatchAlg.h"
#include "MatchingTemplate.h"
#include "GetHammingDistance.h"

double MatchAlg:: mainMatchAlg(char *galleryName,char *probeName, 
							   int gDataType, int pDataType)
{
	if(galleryName ==NULL || probeName == NULL)
	{
		cout << "Failed to load the file name" << endl;
		return -1;
	}	

	MatchingTemplate *gallery = new MatchingTemplate();
	MatchingTemplate *probe= new MatchingTemplate();

	gallery->loadVASIRTemplate(galleryName, gDataType);
	probe->loadVASIRTemplate(probeName, pDataType);	
	
	// DEBUG
	if (gallery->getIrisTemplatePtr() == NULL 
		|| probe->getIrisTemplatePtr() == NULL) 
	{
		printf("Error: Gallery = (%s) %p, probe = (%s) %p\n", 
			galleryName, gallery->getIrisTemplatePtr(), 
			probeName, probe->getIrisTemplatePtr());
		delete gallery;
		delete probe;
		return -1;
	}
	// Calculate Hamming distance using XorY shifts method
	GetHammingDistance hamming (5, 1);//10, 3
	double hd = -1;
	int hdScales = 1;	

	hd = hamming.computeHDXorY((const MatchingTemplate*)gallery,
							   (const MatchingTemplate*)probe, 
				   			    hdScales);//X or Y direction shifts*/
	
	printf("Hamming Distance: %.4f\n", hd);

	delete gallery;
	delete probe;
	return hd;   
}
