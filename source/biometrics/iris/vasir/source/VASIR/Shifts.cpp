#include "Shifts.h"
#include <math.h>

Shifts::Shifts(void)
{
}

Shifts::~Shifts(void)
{
}

/// shift bit-wise
void Shifts::X_ShiftBits(int *templates, int width, int height, int shifts,int nscales, int *templatenew)
{
	int i, s, p, x, y;
	for (i = 0; i<width*height; i++)
		templatenew[i] = 0;
	
	s = 2*nscales*(int)fabs((float)shifts);	
	p = width-s;
	
	if (shifts == 0)
	{
		for (i = 0; i<width*height; i++)
			templatenew[i] = templates[i];
	}       
	else if (shifts<0) //shift left
	{
	  for (y = 0; y<height; y++)
	    for (x = 0; x<p; x++)
	      templatenew[y*width+x] = templates[y*width+s+x];
		   
	  for (y = 0; y<height; y++)
	    for (x=p; x<width; x++)
	      templatenew[y*width+ x] = templates[y*width+ x-p];
		
	}    
	else // shift right
	{
	  for (y = 0; y<height; y++)
	    for (x = s; x<width; x++)
	      templatenew[y*width+ x] = templates[y*width+x-s];		
    
	  for (y = 0; y<height; y++)
	    for (x=0; x<s; x++)
	      templatenew[y*width+x] = templates[y*width+p+x];
	}
}

void Shifts::Y_ShiftBits(int *templates, int width, int height, int shifts,int nscales, int *templatenew)
{
	int i, s, p, x, y;
	for (i = 0; i<width*height; i++)
		templatenew[i] = 0;

	// For up and down we should shift one bit (Radius)
	s = nscales*(int)fabs((float)shifts);
	p = height-s;

	if (shifts == 0)
	{
		for (i = 0; i<width*height; i++)
			templatenew[i] = templates[i];
	}
 	else if (shifts<0) // Shift upwards
	{
	  for (y = 0; y<p; y++)
	    for (x = 0; x<width; x++)
			templatenew[y*width+x] = templates[(y+s)*width+x];
    
	  for (y = p; y < height; y++)
	    for (x = 0; x<width; x++)
			templatenew[y*width+ x] = 1; // radius:This is not a circle
	}    
	else
	{
	  for (y = s; y<height; y++) // Shift downwards
	    for (x = 0; x<width; x++)
			templatenew[y*width+ x] = templates[(y-s)*width+x];

	  for (y = 0; y<s; y++)
	    for (x=0; x<width; x++)
			templatenew[y*width+ x] = 1; // Again, not a circle
	}
}

