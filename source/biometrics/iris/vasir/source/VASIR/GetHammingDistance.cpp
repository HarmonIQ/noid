#include "GetHammingDistance.h"
#include <iostream>
using namespace std;
#include <string.h>
#include "Shifts.h"

GetHammingDistance::GetHammingDistance(int x, int y)
{
	width = 0;
	height = 0;
	maxShiftX = x;
	maxShiftY = y;

	template1s = NULL;
	mask1s = NULL;
	mask = NULL;
	C = NULL;
}
GetHammingDistance::~GetHammingDistance()
{
	if(template1s != NULL)
		delete[] template1s;
	if(mask1s != NULL)
		delete[] mask1s;
	if(mask != NULL)
		delete[] mask;
	if(C != NULL)
		delete[] C;
}

void GetHammingDistance::initializeTemplates(const MatchingTemplate* classTemplate1,
						       const MatchingTemplate* classTemplate2)
{
	int t_width = classTemplate1->getNumAngularDivisions();
	int t_height = classTemplate1->getNumRadialDivisions();

	if ( (t_width != classTemplate2->getNumAngularDivisions() ) || 
		 (t_height != classTemplate2->getNumRadialDivisions() ) ) 
	{
		cout << "Error: Template dimensions not equal" << endl;
		return;
	}
	
	if (width*height == 0) 
	{
		template1s = new int[t_width*t_height];
		mask1s = new int[t_width*t_height];		
		mask = new int[t_width*t_height];
		C = new int[t_width*t_height];
	}
	else
	{
		if(template1s != NULL)
			delete[] template1s;
		
		template1s = new int[t_width*t_height];

		if(mask1s != NULL)
			delete[] mask1s;
		mask1s = new int[t_width*t_height];

		if(mask != NULL)
			delete[] mask;
		mask = new int[t_width*t_height];

		if(C != NULL)
			delete[] C;
		C = new int[t_width*t_height];
	}

	width = t_width;
	height = t_height;
}

double GetHammingDistance::calcHD(int* tTemplate, int* tMask, int* qTemplate, int* qMask, 
									   int* newTemplate, int* newMask)
{
	int nummaskbits = 0;
	int bitsdiff = 0; 
	int totalbits = 0;
	double hd1 = -1; 

    for (int i = 0; i < width * height; i++) 
	{
      mask[i] = newMask[i] | qMask[i];//Bitwise: OR
      if (mask[i] == 1)
		nummaskbits++;	

	  C[i] = newTemplate[i] ^ qTemplate[i];//Bitwise: XOR
      C[i] = C[i] & (1-mask[i]);//Bitwise: AND
      if (C[i] == 1)
		bitsdiff++;	  
    }

    totalbits = width * height - nummaskbits;   	
	
    if (totalbits != 0)  
	{       
      hd1 = (double)bitsdiff / totalbits;
	}
	
	return hd1;
}

double GetHammingDistance::computeHDX(const MatchingTemplate* classTemplate1,
						       const MatchingTemplate* classTemplate2,
						       int scales) 
{
	initializeTemplates(classTemplate1, classTemplate2);

	int *template1 = classTemplate1->getIrisTemplatePtr();
	int *template2 = classTemplate2->getIrisTemplatePtr();
	int *mask1 = classTemplate1->getIrisMaskPtr();
	int *mask2 = classTemplate2->getIrisMaskPtr();

	double hd = -1;
    //int index = 1;

	for (int shifts = -maxShiftX; shifts <= maxShiftX; shifts++)
	{    
		Shifts::X_ShiftBits(template1, width, height, shifts, scales, template1s);    
		Shifts::X_ShiftBits(mask1, width, height, shifts, scales, mask1s);

		double hd1 = calcHD(template1, mask1, template2, mask2, template1s, mask1s);

		if(hd1 < hd || hd == -1) 
			hd = hd1;
	}

	return hd;

}


double GetHammingDistance::computeHDY(const MatchingTemplate* classTemplate1,
						       const MatchingTemplate* classTemplate2,
						       int scales) 
{
	initializeTemplates(classTemplate1, classTemplate2);

	int *template1 = classTemplate1->getIrisTemplatePtr();
	int *template2 = classTemplate2->getIrisTemplatePtr();
	int *mask1 = classTemplate1->getIrisMaskPtr();
	int *mask2 = classTemplate2->getIrisMaskPtr();

	double hd = -1;
	int index = 1;

	for (int shifts = -maxShiftY; shifts <= maxShiftY; shifts++)
	{    
		Shifts::Y_ShiftBits(template1, width, height, shifts, scales, template1s);    
		Shifts::Y_ShiftBits(mask1, width, height, shifts, scales, mask1s);

		double hd1 = calcHD(template1, mask1, template2, mask2, template1s, mask1s);

		if(hd1 < hd || hd == -1) 
			hd = hd1;

	}
	
	return hd;
}

double GetHammingDistance::computeHDXorY(const MatchingTemplate* classTemplate1,
						       const MatchingTemplate* classTemplate2,
						       int scales) 
{
	double xHD = computeHDX(classTemplate1, classTemplate2, scales);
	double yHD = computeHDY(classTemplate1, classTemplate2, scales);
	
	double hd = (xHD < yHD) ? hd = xHD : hd = yHD;

	return hd;
}

double GetHammingDistance::computeHDXandY(const MatchingTemplate* classTemplate1,
						       const MatchingTemplate* classTemplate2,
						       int scales) 
{	
	initializeTemplates(classTemplate1, classTemplate2);
	template2s = new int[width*height];
	mask2s = new int[width*height];

	int *template1 = classTemplate1->getIrisTemplatePtr();
	int *template2 = classTemplate2->getIrisTemplatePtr();
	int *mask1 = classTemplate1->getIrisMaskPtr();
	int *mask2 = classTemplate2->getIrisMaskPtr();

	double hd = -1;
    //int index = 1;

	for (int shifts1 = -maxShiftY; shifts1 <= maxShiftY; shifts1++)
	{ 	 
		Shifts::Y_ShiftBits(template1, width, height, shifts1, scales, template1s);
		Shifts::Y_ShiftBits(mask1, width, height, shifts1, scales, mask1s);	
		
		for (int shifts2 = -maxShiftX; shifts2 <= maxShiftX; shifts2++)
		{
			Shifts::X_ShiftBits(template1s, width, height, shifts2, scales, template2s);
			Shifts::X_ShiftBits(mask1s, width, height, shifts2, scales, mask2s);
			
			double hd1 = calcHD(template1, mask1, template2, mask2, template2s, mask2s);

			if(hd1 < hd || hd == -1) 
				hd = hd1;
		}
	}

	delete[] template2s;
	delete[] mask2s;

	return hd;
}


