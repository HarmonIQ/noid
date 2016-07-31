#include "Analysis.h"
#include "Utility.h"
#include "MatchingTemplate.h"
#include "GetHammingDistance.h"

#if _MSC_VER
    #ifndef snprintf
    #define snprintf _snprintf
    #endif
#endif



char* Analysis::newFileName(const char* outputFName, int index, const char* ch, const char* format)
{
	char* newOutputFName = new char[FILENAME_MAX];
    //sprintf_s(newOutputFName,FILENAME_MAX,"%s_%d%s%s", outputFName,index,ch,format);
    snprintf(newOutputFName,FILENAME_MAX,"%s_%d%s%s", outputFName,index,ch,format);
	return newOutputFName;
}

void Analysis::loadNewTitle(const char* outputFName)
{
	Utility* util = new Utility();;

	const char* title = util->getFilenamePart((char*)outputFName);
	util->appendToFile(outputFName, "Title: %s\n\n", title);
	util->appendToFile(outputFName, "  HDXorY  "
		"R/L M/NM gSubID pSubID     	  gFName                       pFName\n");
	delete util;
}

void Analysis::loadNewVectors(const char* imgFileList, 
							string path,
							vector<string> &VecFullPathName,
							vector<string> &VecImgFileName,
							vector<string> &vecSubjectID)
{
	
	FILE*     imgList = NULL;
	char      buffer[FN_BUFFER_SIZE];
	  
	imgList = fopen(imgFileList, "r");

	if (imgList == NULL)
	{   
		printf("ERROR: can not open file %s \n", imgFileList);
		return;   
	}

	while (fgets(buffer, FN_BUFFER_SIZE - 1, imgList) != NULL) 
	{
		string fullPathFname = "";
		string imgFName = "";
		char* fName;

		char*   ptr;
		Utility::stripLineBreak(buffer);
		// Find the filename part
		ptr = strrchr(buffer, '\\');
		if (ptr == NULL) 
		{
			ptr = strrchr(buffer, '/');
		}

		if(ptr == NULL)
		{
			imgFName =string(buffer);
			fullPathFname = path+string(buffer);
		}
		else
		{
			fullPathFname = string(buffer);
			fName = Utility::getFilenamePart(buffer);
			imgFName = string(fName);
		}

		VecFullPathName.push_back(fullPathFname);	
		VecImgFileName.push_back(imgFName);
		

		char subjectID[6];
		memcpy(subjectID, imgFName.c_str(), SUBJECT_ID_LEN);
		subjectID[5] = 0;//Terminate
		vecSubjectID.push_back(string(subjectID));
	}
	fclose(imgList); 
}


void Analysis::loadHammingDistance(char* galleryName, char* probeName, 
								 int gDataType, int pDataType, 
								  double & hd)
{
	//get the templates
	MatchingTemplate *gallery = new MatchingTemplate();
	MatchingTemplate *probe= new MatchingTemplate();
	gallery->loadVASIRTemplate(galleryName, gDataType);
	probe->loadVASIRTemplate(probeName, pDataType);
		
	//DEBUG
	if (gallery->getIrisTemplatePtr() == NULL 
		|| probe->getIrisTemplatePtr() == NULL)
	{
		printf("Error: Gallery = (%s)=> %p, probe = (%s) => %p\n", galleryName, gallery->getIrisTemplatePtr(),
			probeName, probe->getIrisTemplatePtr());
		delete gallery;
		delete probe;
		return;
	}

	//Get Hamming Distance
	GetHammingDistance hamming(5,2);//origin 10,3
	int hdScales = 1;
	

	hd = hamming.computeHDXorY((const MatchingTemplate*)gallery,
										   (const MatchingTemplate*)probe, 
							   			    hdScales);//XorY direction shifts*/
	delete gallery;
	delete probe;
}

//Match: file Name (SubjectID) comparison
void Analysis::doNewMatchListFile(const char* gallery_Img_List, 
								  const char* probe_Img_List, 								  
							   const char* outputFName, 
							  string path,
							   int pos,int gDataType, int pDataType)
{ 
	vector<string>	gVecFullPathName;
	vector<string>	pVecFullPathName;
	vector<string>	gVecImgFileName;
	vector<string>	pVecImgFileName;										
	vector<string>	gVecSubID;
	vector<string>	pVecSubID;
	
	//check whether or not the gallery and probe are the same.
	size_t len = 0;
	len = strlen(gallery_Img_List);
	bool isGPSame = false;
	if( memcmp(gallery_Img_List, probe_Img_List, len)== 0)
	{
		isGPSame = true;
	}	

	cout <<"Loading data from gallery..." << endl;
	loadNewVectors(gallery_Img_List, path, gVecFullPathName, gVecImgFileName, gVecSubID);

	cout <<"Loading data from probe..." << endl;
	if(isGPSame == true)
	{
		pVecFullPathName = gVecFullPathName;
		pVecImgFileName = gVecImgFileName;
		pVecSubID = gVecSubID;
	
	} else
	{
		loadNewVectors(probe_Img_List, path, pVecFullPathName, pVecImgFileName, pVecSubID);
	}
	cout <<"Loading data is done..." << endl;

	loadNewTitle(outputFName);

	len = 0;
	for(unsigned int j=0; j<gVecFullPathName.size();j++)
	{
		len = strlen(gVecImgFileName[j].c_str());

		for(unsigned int i=0; i<pVecFullPathName.size();i++)
		{			
			// if the file name is not the same
			if (memcmp(gVecImgFileName[j].c_str(), pVecImgFileName[i].c_str(), SUBJECT_ID_LEN) != 0 
				|| memcmp(gVecImgFileName[j].c_str(), pVecImgFileName[i].c_str(), len)== 0) //excludes if the file name is the same 
			{
				continue;
			}			
			
					
			char* gFName = (char*)gVecFullPathName[j].c_str();
			char* pFName = (char*)pVecFullPathName[i].c_str();
					
			doWriteHD(outputFName, gFName, pFName,
				gVecImgFileName[j].c_str(),pVecImgFileName[i].c_str(),
				gVecSubID[j].c_str(),pVecSubID[i].c_str(),
				pos,gDataType,pDataType, 1);
		}

		if(isGPSame == true)
		{
			pVecFullPathName.erase(pVecFullPathName.begin());
			pVecImgFileName.erase(pVecImgFileName.begin());
			pVecSubID.erase(pVecSubID.begin());			
		}
	}	

	gVecFullPathName.clear();
	pVecFullPathName.clear();
	gVecImgFileName.clear();
	pVecImgFileName.clear();	
	gVecSubID.clear();
	pVecSubID.clear();

	cout << "It is done..." << endl;
}

//NonMatch: file Name (SubjectID) comparison
void Analysis::doNewRandomNonMatch(const char* gallery_Img_List, const char* probe_Img_List, const char* outputFName, 
								string path, int pos, int gDataType, int pDataType)
{	

	vector<string>	gVecFullPathName;
	vector<string>	pVecFullPathName;
	vector<string>	gVecImgFileName;
	vector<string>	pVecImgFileName;	
	vector<string>	gVecSubID;
	vector<string>	pVecSubID;

	//check whether or not the gallery and probe are the same.
	bool isGPSame = false;
	size_t len = 0;
	len = strlen(gallery_Img_List);

	if( memcmp(gallery_Img_List, probe_Img_List, len)== 0)
	{
		isGPSame = true;
	}	

	cout <<"Loading data from gallery..." << endl;
	loadNewVectors(gallery_Img_List, path, gVecFullPathName, gVecImgFileName, gVecSubID);

	cout <<"Loading data from probe..." << endl;
	if(isGPSame == true)
	{
		pVecFullPathName = gVecFullPathName;
		pVecImgFileName = gVecImgFileName;
		pVecSubID = gVecSubID;
	} else
	{
		loadNewVectors(probe_Img_List, path, pVecFullPathName, pVecImgFileName, pVecSubID);
	}
	cout <<"Loading data is done..." << endl;

	bool*			chosenMap;
	unsigned		index;
	unsigned		chosenIndexes[NUM_RANDOM_PROBES];
	
	if(pVecImgFileName.size() <= NUM_RANDOM_PROBES)
	{
		cout << "The number of images are less than " << NUM_RANDOM_PROBES << "..."<< endl;
		return;
	}
	// Create selection array
	chosenMap = new bool[pVecImgFileName.size()];
	srand((unsigned)time(NULL));

	loadNewTitle(outputFName);

	for(unsigned int j=0; j<gVecImgFileName.size(); j++)
	{		
		// Create new "random" selection
		unsigned left = NUM_RANDOM_PROBES;
		memset(chosenMap, false, pVecImgFileName.size());
		while (left > 0) 
		{
			unsigned ridx = (unsigned)rand() % pVecImgFileName.size();

			// Has not been selected yet
			if (!chosenMap[ridx]
			&& (memcmp(gVecImgFileName[j].c_str(), pVecImgFileName[ridx].c_str(), SUBJECT_ID_LEN) != 0)) 
			{
				chosenMap[ridx] = true;
				chosenIndexes[NUM_RANDOM_PROBES - left] = ridx;
				left--;
			}
		}

		// Comparison w/ random selection
		for (index = 0; index < NUM_RANDOM_PROBES; index++) 
		{
			unsigned ridx = chosenIndexes[index];
	
			char* gFName = (char*)gVecFullPathName[j].c_str();
			char* pFName = (char*)pVecFullPathName[ridx].c_str();
					
			doWriteHD(outputFName, gFName, pFName,
				gVecImgFileName[j].c_str(),pVecImgFileName[ridx].c_str(),
				gVecSubID[j].c_str(),pVecSubID[ridx].c_str(),
				pos,gDataType,pDataType, 2);//2=NonMatching
		}	
	} 

	gVecFullPathName.clear();
	pVecFullPathName.clear();
	gVecImgFileName.clear();
	pVecImgFileName.clear();	
	gVecSubID.clear();
	pVecSubID.clear();

	delete[] chosenMap;	
	
	cout << "It is done..." << endl;

}

void Analysis::doWriteHD(const char* outputFName, 
								char *gFullName, char *pFullName,
								const char *gPartName, const char *pPartName,
								const char *gSubjectID, const char *pSubjectID,
								int pos,int gDataType, int pDataType, int matchType)
{
	//Debug
	if(gFullName == NULL || pFullName == NULL
	|| gPartName == NULL || pPartName == NULL
	|| gSubjectID == NULL || pSubjectID == NULL)
	{
		cout << "Failed to load the file name" << endl;
		return;
	}
	
	double hdXorY = 0.0;
	loadHammingDistance(gFullName, pFullName, gDataType, pDataType, hdXorY);

	Utility::appendToFile(outputFName,
	"%10.5f\t"
	"%2d\t%2d\t%5s\t%5s\t%25s\t%25s\n",
	hdXorY,
	pos, matchType, gSubjectID, pSubjectID, gPartName, pPartName);

}

