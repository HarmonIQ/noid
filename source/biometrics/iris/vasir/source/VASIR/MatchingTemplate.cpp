#include "MatchingTemplate.h"
#include "CreateTemplate.h"
#include <iostream>
using namespace std;
#include <string>


MatchingTemplate::MatchingTemplate()
{
	irisMask = NULL;
	irisTemplate = NULL;
};

MatchingTemplate::~MatchingTemplate()
{
	if(irisMask != NULL)
		delete[] irisMask;
	if(irisTemplate != NULL)
		delete[] irisTemplate;
};


/** Load an existing template */
bool MatchingTemplate::loadTemplate(const char* fileName)
{
    //Masek *masek = new Masek();
    Masek *masek = NULL;

    Masek::MAT_DATA *m_data = NULL;

    if (masek->loadtemplate(fileName, &m_data) == 0 )
    {
        cout << "Failed to load the template" << endl;
        return false;
    }

    int d00 = m_data[0].dim[0];
    int d01 = m_data[0].dim[1];

    irisMask = new int[d00*d01];
    for (int a = 0; a < d00; a++)
        for (int b = 0; b < d01; b++)
            irisMask[a*d01+b] =((int*)m_data[0].ptr)[b*d00+a];

    int d10 = m_data[1].dim[0];
    int d11 = m_data[1].dim[1];

    if ( (d00 != d10 ) || (d01 != d11) )
    {
        cout << "mask and template dimensions did not match" << endl;
        return false;
    }

    irisTemplate = new int[d10*d11];
    for (int a = 0; a < d10; a++)
        for (int b = 0; b < d11; b++)
            irisTemplate[a*d11+b] =((int*)m_data[1].ptr)[b*d10+a];

    numAngularDivisions = d01;
    numRadialDivisions = d00;

    free(m_data[0].ptr);
    free(m_data[1].ptr);
    free(m_data);

    delete masek;

    return true;
}

/** Load a VASIR template. */
void MatchingTemplate::loadVASIRTemplate(char* fileName, int dataType)
{
	FILE *statFile = NULL;
	std::string stringFileName = std::string(fileName);
	std::string templateFileName = stringFileName+"-template_hd.mat"; 
	statFile = fopen(templateFileName.c_str(), "r");
		
	if (statFile != NULL) // if "*-template.mat" exists
	{
		fclose(statFile);
		cout << "loading template " << templateFileName << endl;
		loadTemplate(templateFileName.c_str());	
		
	}
	else //if "*-template.mat" does not exists
	{
		cout << "creating template " << templateFileName << endl;
		createVASIRTemplate(fileName, templateFileName.c_str(), dataType);
	}
}

/** Create a new VASIR template. */
void MatchingTemplate::createVASIRTemplate(char* fileName, 
										   const char* templateFileName, 
										   int dataType)
{

	CreateTemplate::newCreateIrisTemplate(fileName, 
								&irisTemplate, 
								&irisMask, 
								&numAngularDivisions, 
								&numRadialDivisions,  
								dataType);
	
    Masek *masek = NULL;
	int a=0, b=0;
	Masek::MAT_DATA *m_data = NULL;
	m_data = (Masek::MAT_DATA*)malloc (sizeof(Masek::MAT_DATA)*2);
    
	strcpy(m_data[0].name, "mask_hd");
	m_data[0].dim[0] = numRadialDivisions;
	m_data[0].dim[1] = numAngularDivisions;
	m_data[0].ptr = malloc(m_data[0].dim[0]*m_data[0].dim[1]);

	for (a = 0; a<m_data[0].dim[0]; a++)
		for (b=0; b<m_data[0].dim[1]; b++)
			((unsigned char*)m_data[0].ptr)[b*m_data[0].dim[0]+a] 
		= (unsigned char)irisMask[a*m_data[0].dim[1]+b];
		
	strcpy(m_data[1].name, "template_hd");
	m_data[1].dim[0] = numRadialDivisions;
	m_data[1].dim[1] = numAngularDivisions;
	m_data[1].ptr = malloc(m_data[1].dim[0]*m_data[1].dim[1]);
	for (a = 0; a<m_data[0].dim[0]; a++)
		for (b=0; b<m_data[0].dim[1]; b++)
			((unsigned char*)m_data[1].ptr)[b*m_data[1].dim[0]+a] 
		= (unsigned char)irisTemplate[a*m_data[1].dim[1]+b];
		
	// Save the template
    masek->savetemplate(templateFileName, m_data);
	
	free(m_data[0].ptr);
	free(m_data[1].ptr);
	free(m_data);
    delete masek;
	
}



