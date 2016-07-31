#include <QFileDialog>

#include "YooIRIS.h"

#include "VASIR/EyeDetection.h"
#include "VASIR/ImageQuality.h"
#include "VASIR/MatchAlg.h"
#include "VASIR/EyeRegionExtraction.h"

#include "Analysis/Analysis.h"
#include "Analysis/GetROC.h"

#if _MSC_VER
    #ifndef sprintf
    #define sprintf sprintf_s
    #endif
#endif

//VASIR
YooIRIS::YooIRIS( QWidget* parent /*= 0*/, Qt::WindowFlags flags /*= 0*/ ) : QMainWindow(parent, flags)
{
	eyeDetection = new EyeDetection("..\\bin\\Cascade\\parojosG_45x11.xml");// set the default cascade
	leftFileName1 = NULL;
	leftFileName2 = NULL;
	rightFileName1 = NULL;
	rightFileName2 = NULL;
	gDataType1 = 0; gDataType2 = 0;
	pDataType1 = 0; pDataType2 = 0;

	setupUi(this);
}

YooIRIS::~YooIRIS()
{
	//Memory error here ==>
	if(eyeDetection != NULL)			
		delete eyeDetection;

	delete[] leftFileName1;
	delete[] leftFileName2;
	delete[] rightFileName1;
	delete[] rightFileName2;
}

void YooIRIS::SelectCascade()
{ 
	char* myCascadeFileName = openFileName("../bin/Cascade", "Open cascade XML file", "XML Files (*.xml)");
	if(myCascadeFileName != NULL)
	{    
		// Release previously loaded cascade
		if (eyeDetection != NULL)		
			delete eyeDetection;
		    
		// Try to load the cascade
		try
		{
			 eyeDetection = new EyeDetection(myCascadeFileName);
		}
		catch (const char* msg)
		{
			cout << "Error: Failed to load the cascade " << msg << endl;
		}		
    }
}

// MENU: Selects best quality image from eye images detected 
// in video file
void YooIRIS::selectBestEye(IplImage* currentImg, int index, int &bestIndex, IplImage*& bestImg, double& bestScore)
{
	// Convert image to the gray scale
	IplImage* grayImg = NULL;
	grayImg = ImageUtility::convertToGray(currentImg);
	
	// Calculate the quality score
	double score = ImageQuality::doProcess(grayImg, 0);//0: SOBEL
	cout << "Quality score: " << score << endl;
  
	// Select the best image quality score
	if (score > bestScore)
	{
		// Release previous - if any
		if (bestImg != NULL)		
			cvReleaseImage(&bestImg);
		
		// Copy the best image to bestImg
		bestImg = cvCloneImage(currentImg);
		bestScore = score;
        bestIndex = index;
	}

	cvReleaseImage(&grayImg);
}


// MENU: Selects best quality image from eye images detected in video file
// With the pupil position alignment
void YooIRIS::selectBestEyePairImage(IplImage* currentImg, IplImage* currentPairImg, int index, int& bestIndex, IplImage*& bestImg, double& bestScore)
{
	// Convert image to the gray scale
	IplImage* grayImg = NULL;
	grayImg = ImageUtility::convertToGray(currentImg);
	
	// Calculate the quality score
	double score = ImageQuality::doProcess(grayImg, 0);//0: SOBEL
	cout << "Quality score: " << score << endl;
  
	// Select the best image quality score
	if (score > bestScore)
	{
		// Release previous - if any
		if (bestImg != NULL)		
			cvReleaseImage(&bestImg);
		
		// Copy the pair image to bestImg that selected by the left or right quality score
		bestImg = cvCloneImage(currentPairImg);
		bestScore = score;
		bestIndex = index;
	}
	cvReleaseImage(&grayImg);
}



//MENU: Open Face Video file and detect the eye regions
void YooIRIS::OpenAVIFile()
{
	loadVideoFile();

}

// Loads video file from dialog box
void YooIRIS::loadVideoFile()
{
	const char* path = "../../../Samples/";
	char* videoFileName = openFileName(path, "Open AVI file","AVI Files (*.avi)");

	//Try to load the video files
	if(videoFileName != NULL)
	{
		try
		{
			// Clean up
			clearWidget();

			// Change the current tab
			tabWidget->setCurrentIndex(0);
            gDataType1 = NIR_FACE_VIDEO;
            gDataType2 = NIR_FACE_VIDEO;

			detectEyeRegion(videoFileName);			
		}
		catch (const char* msg)
		{
		  cout << "Error: Failed to load the video file " << msg << endl;
		}
	}	
}

//FUNCTION: Detects the eye region / extracts and saves it
void YooIRIS::detectEyeRegion(char* fileName)
{	
	// Video scales. 4: 2048x2048(MBGC Data), 2: 1024x1024, 1: 512x512
	const int scales = 4;
	// Eye region rectangl adjustment (pixel unit)
	const int val = 4;
	// Set the minimum size of the two eye regions to avoid false detection
	int w = 200, h = 45;

	// Draw the detected area
	ImageSource* imgSrc = NULL;

    // Use for the image quality measurement
    IplImage* imgForLeftEye = NULL;
    IplImage* imgForRightEye = NULL;
	IplImage* img = NULL;

	int indexLeft = -1;
	int indexRight = -1;
	double prevLeftScore = -1;
    double prevRightScore = -1;	
    try
    {      
		// Check whether or not the cascade was loaded
		if(!eyeDetection)
		{
			cerr << "Load a cascade first" << endl;
			return;
		}
		else
		{
			// Load Video file to Qt
			imgSrc = new CVVideoSource(fileName);
			cout << "Number of frames: " << imgSrc->getNumberOfImages() << endl;
 
			int i = 1; //Number of detected images
			int j = 1; //Frame number that contains the detected eye
			while (imgSrc->hasNextImage()) 
			{

				img = imgSrc->getNextImage();//\todo Confirm this one				

                if (img == NULL)
                {
                    cerr << "There is no more next image" << endl;
                }
                if(img != NULL)
				{ 					
				   EyeDetection::RESULT* res = NULL;
				   res = eyeDetection->detect(img, scales, val, w, h);	           
				   // Draw the current frame
				   imgWidget->setImage(img); // Don't forget this

				   if (res == NULL)
				   {
					 cerr << "No eyes were detected" << endl;
				   }
				   else
				   {  
						// Draw rectangles for each detected eye region  
						imgWidget->addRectange(res->leftRect);
                        imgWidget->addRectange(res->rightRect);
                        cout << "Index: " << i << endl;
						
                        //select the best-eye image without the pupil position alignment
                        selectBestEye(res->leftImg, i, indexLeft, imgForLeftEye, prevLeftScore);
                        selectBestEye(res->rightImg, i, indexRight, imgForRightEye, prevRightScore);

						//select the best-eye image with the pupil position alignment						
                        //selectBestEyePairImage(res->leftImg, res->bothImg, i, indexLeft, imgForLeftEye, prevLeftScore);
                        //selectBestEyePairImage(res->rightImg, res->bothImg, i, indexRight, imgForRightEye, prevRightScore);

						//@Analysis purpose
						// Save detected left and right image
                        #if 1
						ImageUtility::SaveImageOptions(res->leftImg, fileName, j, "L", i, imgSrc->getNumberOfImages());
						ImageUtility::SaveImageOptions(res->rightImg, fileName, j, "R", i, imgSrc->getNumberOfImages());
						#endif
												
						i++;						

						cvReleaseImage(&res->leftImg);
						cvReleaseImage(&res->rightImg);
                        cvReleaseImage(&res->bothImg);
						
					}
					imgWidget->repaint();				    
				}
                j++;
				
			}//while
            imgWidget->reset();
            imgWidget->repaint();

            // Change current tab
            tabWidget->setCurrentIndex(1);
						
			cout << "Selected Index for Left: " << indexLeft << "    Selected Index for Right: " << indexRight << endl;
			

            drawBestImage(fileName, indexLeft, indexRight, imgForLeftEye, imgForRightEye);
            cvReleaseImage(&imgForLeftEye);
            cvReleaseImage(&imgForRightEye);

            //This is an option
            /*//Align the left and right eye position using pupil information and then extract the eye region
            IplImage* extractedLeftImg = NULL;
            IplImage* extractedRightImg = NULL;
            alignExractEyeImage(imgForLeftEye, imgForRightEye, NIR_FACE_VIDEO, "bmp", extractedLeftImg, extractedRightImg);
            cvReleaseImage(&imgForLeftEye);
            cvReleaseImage(&imgForRightEye);
			
			// Draw best-eye image
            drawBestImage(fileName, indexLeft, indexRight, extractedLeftImg, extractedRightImg);//with pupil alignment
            cvReleaseImage(&extractedLeftImg);
            cvReleaseImage(&extractedRightImg);*/

		}
    }
    catch (const char* msg)
    {
      cout << "Error: " << msg << endl;
    }		
    
	if(imgSrc != NULL)
		delete imgSrc; 
}

//Align the left and right eye position using pupil information and then extract the eye region
void YooIRIS::alignExractEyeImage(IplImage* currentLeftPairImg, IplImage* currentRightPairImg,
		                            int dataType, const char* imageFormat, IplImage*& lImg, IplImage*& rImg)
{
	IplImage* rImg1 = NULL;
	IplImage* lImg1 = NULL;

	float nScale = 1.0;		
	const int speed_m = 1;// Default 1
	const int alpha = 20; // Alpha value for contrast threshold
	// Setup the parameters to avoid that noise caused by reflections and 
    // eyelashes covers the pupil
    double ratio4Circle = 1.0;
    // Initialize for Closing and Opening process
	int closeItr = 2;//dilate->erode
	int openItr = 3;//erode->dilate
	double norm = 200.0;

	if(dataType == NIR_IRIS_STILL) // Classic still images
	{
		nScale = 2.0;
		ratio4Circle = 0.90;
		closeItr = 2;//best for noScaled Still
		openItr = 3;//best for noScaled Still
	}
	else if(dataType == NIR_FACE_VIDEO) // Distant video imagery
	{
		nScale = 1.0;
		// Still need to find an optimal value
		ratio4Circle = 0.65;
		closeItr = 0;
		openItr = 3;
	}

	int rPupilMax = (int) (42*nScale); 
	int rIrisMax = (int) (82*nScale);
	
	//Extract the left eye image-> best left or right image can be from different frame
	EyeRegionExtraction::doExtract(currentLeftPairImg, rPupilMax, rIrisMax, ratio4Circle, closeItr, openItr, speed_m, alpha, norm, nScale, imageFormat, lImg, rImg1);
	//Extract the right eye image
	EyeRegionExtraction::doExtract(currentRightPairImg, rPupilMax, rIrisMax,  ratio4Circle, closeItr, openItr, speed_m, alpha, norm, nScale, imageFormat, lImg1, rImg);
	
	cvReleaseImage(&lImg1);
	cvReleaseImage(&rImg1);
}

// Widget: Draw an image on the widget
void YooIRIS::drawBestImage(char* fileName, int indexLeft, int indexRight, IplImage* imgLeft, IplImage* imgRight)
{
	if(imgLeft != NULL)
	{ 	
        Ui_YooIRIS::imgLeftWidget1->setImage(imgLeft, true);
		Ui_YooIRIS::imgLeftWidget1->repaint();
		if (leftFileName1 != NULL)
			delete[] leftFileName1;
		// display the data type
		txtLeftDataType1->setText(txtDataType(gDataType1));

		char buffer[10];
        sprintf(buffer,"BEST_L%d", indexLeft);
		const char* temp = buffer;
		// Save best left eye image
		leftFileName1 = ImageUtility::SaveEyeImages(imgLeft, fileName, temp, "bmp");
	}
	if(imgRight != NULL)
	{        
        Ui_YooIRIS::imgRightWidget1->setImage(imgRight, true);
		Ui_YooIRIS::imgRightWidget1->repaint();
		if (rightFileName1 != NULL)
			delete[] rightFileName1;
		// display the data type
		txtRightDataType1->setText(txtDataType(gDataType2));

		char buffer[10];
        sprintf(buffer,"BEST_R%d", indexRight);
		const char* temp = buffer;
		// Save best right eye image
		rightFileName1 = ImageUtility::SaveEyeImages(imgRight, fileName, temp, "bmp");
	}	
}

//MENU:  \todo Future work
void YooIRIS::OpenIrisAVIFile()
{ 
}

//MENU:  \todo Future work
void YooIRIS::OpenCAM()
{     
}

//MENU: Image Quality Measurement using Sobel operator
void YooIRIS::checkQuality()
{
	const char* path = "../../../Samples/";
	const char* myFileName = openFileName(path, "Open the image","images (*.bmp *.tiff *.jpg *.pgm)");
	if(myFileName != NULL)
	{
		try
		{
			IplImage* img = NULL;
			img = cvLoadImage(myFileName,0);

			// If it is NULL, release the image, and stop processing
			if(img == NULL)
			{
				printf("Failed to load the file: %s\n", myFileName);
				return;
			}
			double score = ImageQuality::doProcess(img, 0);//0:Sobel
			cout << "Quality score: " << score << endl;
			cvReleaseImage(&img);
		}
		catch(const char* msg)
		{
			cout << "ERROR: Failed to load the file" << msg << endl;
		}
	}
}

//MENU: Matches templates
void YooIRIS::StartMatch()
{
	const double thresholdHD = 0.39;

	clearWidget();

	//Load the target image
	openLeftEye1();
	if(leftFileName1 != NULL && gDataType1 != 0)
	{
		//Load the query image
		openLeftEye2();
		
		if(leftFileName2 != NULL && pDataType1 != 0)
		{				
			double leftHD = 0.0;
			leftHD = MatchAlg::mainMatchAlg((char*)leftFileName1, (char*)leftFileName2,
										gDataType1, pDataType1);
			if(leftHD != -1 && leftHD < thresholdHD)
			{
				txtLeftResult->setText("Match");
				txtLeftResult->setStyleSheet("QLabel {background-color: #33CC00}"); 
			}
			else
			{
				txtLeftResult->setText("No Match");
				txtLeftResult->setStyleSheet("QLabel {background-color: #FF3300}"); 
			}		
		}
	}
}

//BUTTON: Matching
void YooIRIS::goMatch()
{
	// \todo Possible to optimize?
	const double thresholdHD = 0.39;
	double leftHD = 0.0;
	double rightHD = 0.0;	
	
	if((leftFileName1==NULL && leftFileName2==NULL 
		&& rightFileName1==NULL && rightFileName2==NULL)
		|| (leftFileName1 != NULL && leftFileName2 == NULL)
		|| (leftFileName1 == NULL && leftFileName2 != NULL)
		|| (rightFileName1 != NULL && rightFileName2 == NULL)
		|| (rightFileName1 == NULL && rightFileName2 != NULL))
	{
		cout << "Failed to load the target or query image file" << endl;
		return;
	}

	if(leftFileName1 != NULL && leftFileName2 != NULL)
	{
		
		leftHD = MatchAlg::mainMatchAlg((char*)leftFileName1, (char*)leftFileName2,
										gDataType1, pDataType1);
		if(leftHD != -1 && leftHD < thresholdHD)
		{
			 
			txtLeftResult->setText("Match");
			txtLeftResult->setStyleSheet("QLabel {background-color: #33CC00}"); 
		}
		else
		{
			
			txtLeftResult->setText("No Match");
			txtLeftResult->setStyleSheet("QLabel {background-color: #FF3300}"); 
		}			
	
	}
	if(rightFileName1 != NULL && rightFileName2 != NULL)
	{
		rightHD = MatchAlg::mainMatchAlg((char*)rightFileName1, (char*)rightFileName2,
										gDataType2, pDataType2);
		
		if(rightHD != -1 && rightHD < thresholdHD)
		{
			txtRightResult->setText("Match");
			txtRightResult->setStyleSheet("QLabel {background-color: #33CC00}");                  
		}
		else
		{
			txtRightResult->setText("No Match");
			txtRightResult->setStyleSheet("QLabel {background-color: #FF3300}"); 
		}
	}	
}

//BUTTON: Opens the image selected in dialog
void YooIRIS::openLeftEye1()
{  
	gDataType1 = 0;
	gDataType1 = getDataType();	
	if(gDataType1 == 0)
	{
		cout << "Failed to load the data type" << endl;
		return;
	}
	leftFileName1 = NULL;  
	const char* dir = "../../../Samples/";
	leftFileName1 = drawEyeImage(this->imgLeftWidget1, "Open target image", dir);
	
	if(leftFileName1==NULL)
	{
		cout << "Failed to load the target image file" << endl;
		return;
	}
	txtLeftDataType1->setText(txtDataType(gDataType1));
	
}

// BUTTON: Opens the image selected in dialog
void YooIRIS::openLeftEye2()
{
	pDataType1 = 0;
	pDataType1 = getDataType();
	if(pDataType1 == 0)
	{
		cout << "Failed to load the data type" << endl;
		return;
	}
	leftFileName2 = NULL;
	const char* dir = "../../../Samples/";
	leftFileName2 = drawEyeImage(this->imgLeftWidget2, "Open query image", dir);
	if(leftFileName2==NULL)
	{
		cout << "Failed to load the query image file" << endl;
		return;
	}
	txtLeftDataType2->setText(txtDataType(pDataType1)); 
}

// BUTTON: Opens the image selected in dialog
void YooIRIS::openRightEye1()
{
	gDataType2 = 0;
	gDataType2 = getDataType();
	if(gDataType2 == 0)
	{
		cout << "Failed to load the data type" << endl;
		return;
	}
	rightFileName1 = NULL;
	const char* dir = "../../../Samples/";
	rightFileName1 = drawEyeImage(this->imgRightWidget1, "Open target image", dir);
	if(rightFileName1==NULL)
	{
		cout << "Failed to load the target image file" << endl;
		return;
	}
	txtRightDataType1->setText(txtDataType(gDataType2));
}

// BUTTON: Opens the image selected in dialog
void YooIRIS::openRightEye2()
{
	pDataType2 = 0;
	pDataType2 = getDataType();
	if(pDataType2 == 0)
	{
		cout << "Failed to load the data type" << endl;
		return;
	}
	rightFileName2 = NULL;
	const char* dir = "../../../Samples/";
	rightFileName2 = drawEyeImage(this->imgRightWidget2, "Open query image", dir);
	if(leftFileName2==NULL)
	{
		cout << "Failed to load the query image file" << endl;
		return;
	}
	txtRightDataType2->setText(txtDataType(pDataType2));
}

const char* YooIRIS::txtDataType(int type)
{
	const char* str ="Data Type here";
	if(type==NIR_IRIS_STILL)
		str = "Classical Still Image";
	else if(type==NIR_FACE_VIDEO)
		str= "Distant Video Frame";

	return str;
}

// Widget: Draws the opened image file
char* YooIRIS:: drawEyeImage(ImageWidget *imgWidget, const char* title, const char* dir)
{
	YooIRIS::MATCHDATA matchData = getEyeImage(title, dir);
	if(matchData.imgMatch != NULL && matchData.matchFileName != NULL)
	{
		IplImage* img = NULL;
		img = matchData.imgMatch;
		char* eyeFileName = matchData.matchFileName;
		imgWidget->setImage(img);		
		imgWidget->repaint();
		cvReleaseImage(&img);
		return eyeFileName;		 
	}
	return NULL;
}

// Dialog & Widget: Get the eye image
YooIRIS::MATCHDATA YooIRIS::getEyeImage(const char* title, const char* dir)
{
	YooIRIS::MATCHDATA matchData;
	matchData.imgMatch = NULL;	
	matchData.matchFileName = NULL;		
	char* myFileName = openFileName(dir, title, "images (*.bmp *.tiff *.jpg *.pgm)");

	if(myFileName != NULL)
	{
		// Repaint dialog
		this->repaint();
		matchData.matchFileName = myFileName;
		matchData.imgMatch = cvLoadImage(myFileName,0);
		if(matchData.imgMatch == NULL)
		{
			cout << "Faliled to load the image file" << endl;
		}			
		return matchData;		
	}
	return matchData;
}


// Dialog & Widget: Get the eye image
int YooIRIS::getDataType()
{
	int mode = 0;
	//select the finger mode
	ModeDialog* dlg = new ModeDialog(this);
	int ret = dlg->exec();
	if (ret == QDialog::Accepted) 
	{
		mode = dlg->getMode();
	}
	return mode;
}


/**
* Menu item: Generate all reports.
*/
void YooIRIS::generateReports()
{

   InputDialog* dlg = new InputDialog(this);
   dlg->exec();
   delete dlg;
}

