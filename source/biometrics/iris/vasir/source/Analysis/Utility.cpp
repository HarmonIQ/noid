#include "Utility.h"
#include "Analysis.h"


Utility::Utility(void)
{
}


Utility::~Utility(void)
{
}

/*
* Jumps to a specific column
* ColNo = creater or equal to 0
* May return NULL in case there are not enough columns
*/
char* Utility::jumpToColumn(char* buf, int colNo, int tabNum)
{
	char* pos;
	while (colNo >= 0)
	{
		pos = strchr(buf, '\t');
		//pos = strchr(buf, ' ');
		//if no tab found
		if(pos == NULL)
			return NULL;

		//position 'buf' after the two tabs
		buf = pos+tabNum;
		colNo--;
	}
	return buf;
}

/*
* Extracts the text of the current column
* Note: Modifies the fiven buffer
* 'buf' will point to the next column after the call
*/
char* Utility::getColumn(char** buf, int tabNum)
{
	char* org;
	char* pos;

	org = *buf;
	pos = strchr(org, '\t');
	//pos = strchr(org, ' ');
	if(pos == NULL)
	{
		org[strlen(org) -1] = '\0';
		*buf = NULL;
	}
	else
	{
		*pos = '\0';
		*buf = pos+tabNum;//one tab, two tabs

	}
	return org;
}



//Read single column from a file
int Utility::readSingleColumn(const char* fname, int location, int skip, 
									std::vector<float> &val)
{	
	char buf[FN_BUFFER_SIZE];
	FILE* fin;
	char* pos;
	int tabNum = 1;//2
	float hd;

	fin = fopen(fname, "rt");
	if(fin == NULL)
	{
		printf("Failed to open %s\n", fname);
		return -1;
	}

	//Skip first # lines(read but do not process)
	for(int i=0; i<skip;i++)
		fgets(buf, FN_BUFFER_SIZE, fin);

	//Read until the end of the file
	while(fgets(buf, FN_BUFFER_SIZE, fin))
	{
		pos = jumpToColumn(buf, location, tabNum);
		if(pos == NULL)
			continue;
		//convert from char* to float
		hd = (float)atof(getColumn(&pos, tabNum));
		val.push_back(hd);
	}
	fclose(fin);
	return 0;
}

//Read the file from multiple columns
int Utility::readFourScores(const char* fname, 
							  int location, int skip,  
							  std::vector<float> &val1,
							  std::vector<float> &val2,
							  std::vector<float> &val3,
							  std::vector<float> &val4)
{	
	char buf[FN_BUFFER_SIZE];
	FILE* fin;
	char* pos;
	int tabNum = 1;//2;
	float v1, v2, v3, v4;

	fin = fopen(fname, "rt");
	if(fin == NULL)
	{
		printf("Failed to open %s\n", fname);
		return -1;
	}

	//Skip first three lines(read but do not process)
	for(int i=0; i<skip;i++)
		fgets(buf, FN_BUFFER_SIZE, fin);

	//Read until the end of the file
	while(fgets(buf, FN_BUFFER_SIZE, fin))
	{
		pos = jumpToColumn(buf, location, tabNum);//tab number:2
		if(pos == NULL)
			continue;

		//convert from char* to float
		v1 = (float)atof(getColumn(&pos, tabNum));
		v2 = (float)atof(getColumn(&pos, tabNum));
		v3 = (float)atof(getColumn(&pos, tabNum));
		v4 = (float)atof(getColumn(&pos, tabNum));
		//printf("hdX: %.3f, hdY: %.3f, hdXY: %.3f, hdCrossXY: %.3f\n", hdX, hdY, hdXY, hdCrossXY);
		
		//return value

		val1.push_back(v1);
		val2.push_back(v2);
		val3.push_back(v3);
		val4.push_back(v4);
	}
	fclose(fin);
	//free(pos);why error?
	return 0;
}

void Utility::stripLineBreak(char* buffer)
{
	size_t  len = 0;
	len = strlen(buffer);
	if (buffer[len - 1] == '\n') 
	{
		buffer[len - 1] = '\0';
	}
}

char* Utility::getFilenamePart(char* buffer)
{

	char*   ptr;
	stripLineBreak(buffer);	
  
	// Find the filename part
	ptr = strrchr(buffer, '\\');
	if (ptr == NULL) 
	{
		ptr = strrchr(buffer, '/');
	}

	if(ptr == NULL)
		return buffer;
	else
	{
		ptr++;
		return ptr;
	}
}
/* append the info to the file
@param fmt: format to be written
@param ...: variable arguments*/
void Utility::appendToFile(const char* outputFileName, const char* fmt, ...) 
{
	FILE*     fout = NULL;
	va_list   vargs;

	// Try to create or open an existing file
	fout = fopen(outputFileName, "at");
	if (fout == NULL)
	{
		fprintf(stderr, "Failed to create the output file '%s'\n", outputFileName); 
		return;
	}

	// Append the line
	va_start(vargs, fmt);
	vfprintf(fout, fmt, vargs);
	va_end(vargs);

	fclose(fout);
}
