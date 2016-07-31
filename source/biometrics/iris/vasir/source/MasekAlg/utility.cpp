#include "Masek.h"

int Masek::fix(double x)
{
	int ret;	
	ret = (int)x;	
	return ret;
}


int Masek::roundND(double x)
{
	int ret;
	if (x >= 0.0)
		ret = (int)(x+0.5);
	else
		ret = (int)(x-0.5);
	return ret;
}
