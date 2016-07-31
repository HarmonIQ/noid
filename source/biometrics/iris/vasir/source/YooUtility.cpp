#include <iostream>
#include <string>
#include <QFileDialog>
#include "YooIRIS.h"

using namespace std;

//Widget: Clean up before new image widget starts
void YooIRIS::clearWidget()
{
  leftFileName1 = NULL;
  rightFileName1 = NULL;
  leftFileName2 = NULL;
  rightFileName2 = NULL;
  
  imgLeftWidget1->reset();
  imgLeftWidget2->reset();
  imgRightWidget1->reset();
  imgRightWidget2->reset();
  
  txtLeftResult->setText("?");
  txtLeftResult->setStyleSheet("QLabel {background-color: window}"); 
  txtRightResult->setText("?");
  txtRightResult->setStyleSheet("QLabel {background-color: window}"); 

  txtLeftDataType1->setText("Data Type here");
  txtLeftDataType2->setText("Data Type here");
  txtRightDataType1->setText("Data Type here");
  txtRightDataType2->setText("Data Type here");

  this->repaint();
}

//Dialog: open a dialog to select the file
char* YooIRIS::openFileName(const char* path, const char* title, const char* fileType)
{
	QString fileName = NULL;
    //cout << "Before QFileDialog" << endl;
	if(path == NULL)
	{
        // Native Dialogs cause issue on Mac OS X - force Qt built-in dialog
		fileName = QFileDialog::getOpenFileName(this,
            tr(title), QDir::currentPath(), tr(fileType),
            0, QFileDialog::DontUseNativeDialog);
	}else
	{
		fileName = QFileDialog::getOpenFileName(this,
            tr(title), path, tr(fileType),
            0, QFileDialog::DontUseNativeDialog);
    }
    cout << "Loaded: " << fileName.toStdString() << endl;
 
	if (!fileName.isEmpty())
	{		
		QByteArray enc =fileName.toUtf8(); //or toLocal8Bit or whatever
		//then allocate enough memory:
		char* myFileName = new char[enc.size()+1];
		//and copy it
		strcpy(myFileName, enc.data()); 
		return myFileName;
	}
	return NULL;
}

//Enter the output name in commend
char* YooIRIS::getOutputFName(int i, string currentPath, string type)
{
	std::string str;
	cout << i <<"Enter " << type <<": ";
	std::getline(cin, str);
	if(str=="")
	{
		cout << "Please input the output name here." << endl;
		cout << i <<"Enter " << type <<": ";
		std::getline(cin, str);
		if(str=="")
		{
			cout << "ERROR: Failed to enter the input" << endl;
			return NULL;
		}
	}
	if(currentPath=="")
	{
        currentPath = (string)QDir::currentPath().append("/../output/").toLatin1();
	}
	std::string fName = currentPath+str;
	char* myFileName = new char[fName.size()+1];
	strcpy(myFileName, fName.c_str());
	return myFileName;
}

int YooIRIS::getString(std::string title)
{
	std::string str;
	int val = 0;
	cout << "Enter "<< title <<": ";
	std::getline(cin, str);
	val = atoi(str.c_str());
	return val;
}
