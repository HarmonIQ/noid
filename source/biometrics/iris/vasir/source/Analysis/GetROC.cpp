#include "GetROC.h"
#include "Utility.h"
#include <algorithm>
#include <string>
#include <cmath>

void GetROC::rocResult(int colNum, int skip, 
						const char* matchFName, 
						const char* nonMatchFName, 
						const char* outputFName,
						int pos, int index)
{
	vector<float> match, nonMatch, newMatch, newNonMatch; 	
	//read the match and nonmatch columns
	Utility::readSingleColumn(matchFName, colNum, skip, match);
	cout << "Loading for the match is done" << endl;
	Utility::readSingleColumn(nonMatchFName, colNum, skip, nonMatch);
	cout << "Loading for the nonMatch is done"<<endl;

	if(match.size() == 0 || nonMatch.size() == 0)
	{
		cout << "# of matching or nonmatching scores is zero" << endl;
		return;
	}
	//get the hamming distance
	doNewOrderedScores(match, nonMatch, newMatch, newNonMatch);
	createROC(outputFName, newMatch, newNonMatch, pos, index);

	match.clear();
	nonMatch.clear();
	newMatch.clear();
	newNonMatch.clear();
	cout << "It is done... " << endl;

}

//rescale the nonmatch size to the match size
void GetROC::doNewOrderedScores(vector<float> matchScores,
						vector<float> nonMatchScores,
						vector<float>& newMatchScores,
						vector<float>& newNonMatchScores)
{	

	double nDist = 0.0, mDist = 0.0;
	sort(matchScores.begin(), matchScores.end());
	sort(nonMatchScores.begin(), nonMatchScores.end());

	//if nonmatch size is bigger
	if(nonMatchScores.size() > matchScores.size())
	{
		nDist = (double)nonMatchScores.size()/(double)matchScores.size();
		double nIdx = 0.0;
		
		for(long unsigned i=0; i<matchScores.size(); i++)
		{
			nIdx = nIdx+nDist;
			if(matchScores[i] > 0.0)
			{				
				if((long unsigned)nIdx < nonMatchScores.size())
					newNonMatchScores.push_back(nonMatchScores[(long) nIdx]);
						
				newMatchScores.push_back(matchScores[i]);
				//printf("NonMatch: %.3f\n", newNonMatchScores[i]);
			}
		}

		if(newNonMatchScores.size() != newMatchScores.size())
			newNonMatchScores.push_back(nonMatchScores[nonMatchScores.size()-1]);
	}
	//if match size is bigger
	else if(matchScores.size() > nonMatchScores.size())
	{
		mDist = matchScores.size()/nonMatchScores.size();
		double mIdx = 0.0;
		for(long unsigned i=0; i<nonMatchScores.size(); i++)
		{
			mIdx = mIdx+mDist;
			if(nonMatchScores[i] > 0)
			{									
				if((long unsigned)mIdx < matchScores.size())
					newMatchScores.push_back(matchScores[(long unsigned)mIdx]);
		
				newNonMatchScores.push_back(nonMatchScores[i]);
				//printf("NonMatch: %.3f\n", newNonMatchScores[i]);
			}
		}
		if(newMatchScores.size() != newNonMatchScores.size())
			newMatchScores.push_back(matchScores[matchScores.size()-1]);
	}
}

//no rescale
void GetROC::createROC(const char* outputFName, 
					   vector<float> matchScores,
					   vector<float> nonMatchScores, 
					   int pos, int index)//use va_list=> how to?
{
	vector<float> vecMatch, vecNonMatch, vecThres;
	vector<float> vecFRR, vecFAR, vecTRR, vecTAR;	
	//note that matchScores.size() and nonMatchScores.size() are the same.
	long totalSize = matchScores.size();
	float tar = 0.0f, frr = 1.0f, farx = 0.0f, trr = 1.0f;
	float pfrr = 1.0f;
	float pfar = 0.0f;
	float eer = 0.0f;
	bool foundEER = false;
    //int a = 0, b = 0;
	
	Utility *util = new Utility();

	util->appendToFile(outputFName, "DOC# L/R DEX DESIGN Filename\n");
	util->appendToFile(outputFName, "%d    %d    %d    %d  %s\n",
		3, pos, index, 0,(const char*)util->getFilenamePart((char*)outputFName));
	util->appendToFile(outputFName, "Matches NonMatches Thres  FRR     FAR     TRR     TAR\n");

	//if the list is too big, this will reduce time.
	int dist = 1;
	 if(totalSize > 50000)
		dist = (int) (totalSize / 50000);

	for(int j = 0; j < totalSize; j = j + dist)
	{		
		for(int i = 0; i < totalSize; i = i + dist)
		{			
			if(matchScores[i] >= 0.0f && matchScores[i] <= matchScores[j])//this should be "<="
			{
				tar = (float)(i+1)/(float)totalSize;
				frr = 1.0f - tar;
			}
			if(nonMatchScores[i] >= 0.0f && nonMatchScores[i] <= matchScores[j])//this should be "<="
			{
				farx = (float)(i+1)/(float)totalSize;
				trr = 1.0f - farx; 
			}
		}
			
		//printf("frr: %.5f, far: %.5f, trr: %.5f, tar: %.5f\n", frr, farx, trr, tar);	

		vecMatch.push_back(matchScores[j]);
		vecNonMatch.push_back(nonMatchScores[j]);
		vecThres.push_back(matchScores[j]);
		vecFRR.push_back(frr);
		vecFAR.push_back(farx);
		vecTRR.push_back(trr);
		vecTAR.push_back(tar);

		if(!foundEER)
		{
			if(frr < farx)
			{
				float x = (pfar- pfrr)/ (frr-pfrr-farx+pfar);
				eer = ((farx-pfar)*x+pfar);
				foundEER = true;
				//printf("eer: %.5f\n", eer);
			}
		}
		pfrr = frr;
		pfar = farx;
	}
	int nSize = vecFAR.size();
	if(nSize < 200)
	{
		for(int i=0; i<nSize; i++)
		{
			util->appendToFile(outputFName, "%.5f\t%.5f\t%.5f\t%.5f\t%.5f\t%.5f\t%.5f\n",
			vecMatch[i], vecNonMatch[i], vecThres[i], vecFRR[i], vecFAR[i], vecTRR[i], vecTAR[i]);
		}
	}
	else
	{
		for(int i=0; i<nSize; i++)
		{
            if((unsigned int)(i+1) < vecFAR.size() && vecFAR[i] != vecFAR[i+1])
			{
				util->appendToFile(outputFName, "%.5f\t%.5f\t%.5f\t%.5f\t%.5f\t%.5f\t%.5f\n",
				vecMatch[i], vecNonMatch[i], vecThres[i], vecFRR[i], vecFAR[i], vecTRR[i], vecTAR[i]);
			}
		}
		util->appendToFile(outputFName, "%.5f\t%.5f\t%.5f\t%.5f\t%.5f\t%.5f\t%.5f\n",
				vecMatch[nSize-1], vecNonMatch[nSize-1], vecThres[nSize-1], 
				vecFRR[nSize-1], vecFAR[nSize-1], vecTRR[nSize-1], vecTAR[nSize-1]);
	}
	
	if(eer == 0.0f)
		util->appendToFile(outputFName, "%.5f\t-999.00\t-999.00\t-999.00\t-999.00\t-999.00\t-999.00\tEER\n", eer);
	else
		util->appendToFile(outputFName, "%.5f\t-999.00\t-999.00\t-999.00\t-999.00\t-999.00\t-999.00\tEER\n", eer);

	delete util;
	vecMatch.clear();
	vecNonMatch.clear();
	vecThres.clear();
	vecFRR.clear();
	vecFAR.clear();
	vecTRR.clear();
	vecTAR.clear();

	cout <<"The score calculation is done..." << endl;
}

void GetROC::reducedScores1(int colNum, int skip, 
							const char* inputFName, 
							const char* outputFName, 
							int pos, int index)
{
	vector<float> TAR, FRR, TRR, FARx;
	bool anyFAR = false;
	
	Utility::readFourScores(inputFName, colNum, skip, FRR, FARx, TRR, TAR);
	///cout <<"Loading for the file is done" << endl;

	Utility *util = new Utility();
	
	util->appendToFile(outputFName, "DOC# L/R DEX DESIGN Filename\n");
	util->appendToFile(outputFName, "%d    %d    %d    %d  %s\n",
		4, pos, index, 0,(const char*)util->getFilenamePart((char*)outputFName));
	util->appendToFile(outputFName, " FRR     FAR     TRR     TAR\n");
	
	for(float v=0.0001f; v<1.0f; v=v*10.0f)
	{
		for(unsigned int i=0; i< FARx.size(); i++)
		{
			if(FARx[i] >= v && FARx[i] < (v*10.0f) && FARx[i] != -999)
			//if(FARx[i] >= v && FARx[i] != -999)
			{
				if(FARx[i] > 0.99999f)
					v=1.0f;

				if(i != 0)
				{
					anyFAR = true;
					float prev=v-FARx[i-1];
					float next=FARx[i]-v;
					if(prev==0.0f)
					{
						util->appendToFile(outputFName, "%.5f\t%.5f\t%.5f\t%.5f\n", 
							FRR[i-1], FARx[i-1], TRR[i-1], TAR[i-1]);
						break;
					}
					else
					{
                        float y1=0.0f, y2=0.0f;
						y1=TAR[i-1]+((TAR[i]-TAR[i-1])/(prev+next)*prev);
						y2=FRR[i-1]+((FRR[i]-FRR[i-1])/(prev+next)*prev);
						util->appendToFile(outputFName, "%.5f\t%.5f\t%.5f\t%.5f\n", 
															y2, v, 1.0-v, y1);					
						break;
					}
				}
			}
			
		}
		if(anyFAR == false)
		{
			util->appendToFile(outputFName, "%4.2f\t%4.2f\t%4.2f\t%4.2f\n", 
								-999.00, -999.00, -999.00, -999.00);
		}
	}

	if(anyFAR == false && TAR.size() > 2)
	{
		int arr = TAR.size()-2;//ignore last (-999)
		util->appendToFile(outputFName, "%.5f\t%.5f\t%.5f\t%.5f\n", 
							FRR[arr], FARx[arr], TRR[arr], TAR[arr]);
	}

	TAR.clear();
	FRR.clear();
	TRR.clear();
	FARx.clear();
	delete util;
	cout <<"The reduced ROC is done..." << endl;
}
