/*********************************************************//**
** @file YooIRIS.h
** Controller - functions to interface with the Qt UI.
** 
** @date 10/2010
** @author Yooyoung Lee 
**
** Note: Please send BUG reports to yooyoung@<NOSPAM>nist.gov. 
** For more information, refer to: http://www.nist.gov/itl/iad/ig/vasir.cfm
**
** @par Disclaimer 
** This software was developed at the National Institute of Standards 
** and Technology (NIST) by employees of the Federal Government in the
** course of their official duties. Pursuant to Title 17 Section 105 
** of the United States Code, this software is not subject to copyright 
** protection and is in the public domain. NIST assumes no responsibility 
** whatsoever for use by other parties of its source code or open source 
** server, and makes no guarantees, expressed or implied, about its quality, 
** reliability, or any other characteristic.
***************************************************************/

#ifndef YOO_IRIS_H
#define YOO_IRIS_H

#include "ui_YooIRIS.h"

#include "ModeDialog.h"
#include "InputDialog.h"

#include "CVImageSource.h"

class EyeDetection;

/**
 * Main controller.
 */
class YooIRIS : public QMainWindow, protected Ui_YooIRIS
{
	Q_OBJECT
public:

	/** Image and its filename. */
	typedef struct 
	{
    	char* 		matchFileName; ///< Filename
    	IplImage*	imgMatch; ///< Image instance
  	} MATCHDATA;

    YooIRIS(QWidget* parent = 0, Qt::WindowFlags flags = 0);
	virtual ~YooIRIS(); 
	
	/**
	 * Lets the user select a file in a dialog box.
	 *
	 * @param path		Default path
	 * @param title		Title of the dialog box.
	 * @param fileType 	file-type (extension).
	 * @return 			Selected filename, or \c NULL in case the user clicked "cancel"
	 */
	char* openFileName(const char* path, const char* title, const char* fileType);
	
	/**
	 * Lets the user enter option or string in command prompt.
	 *
	 * @param i			  Index
	 * @param currentPath Current path
	 * @param type Valid \c dataType values:
	 *               - \c 1 = classic still image
	 *               - \c 2 = video captured at a distance (distant-videos)
	 * @return Entered option or string
	 */
	char* getOutputFName(int i, std::string currentPath, std::string type);

public slots:
	/** 
    * Menu item: Load a face video and detects the eye regions.
    */
    void OpenAVIFile();
	
	/// \todo Future work
	/** 
    * Menu item: Load an iris video and detects the eye regions.
    */
	void OpenIrisAVIFile();

	/// \todo Future works
	/** 
    * Menu item: Capture a webcam video frame and detects the eye regions.
    */
    void OpenCAM();

	/** 
    * Menu item: Load a manually selected cascade.
    */
    void SelectCascade();

	/** 
    * Menu item: Load an image manually for quality measurement.
    */
	void checkQuality();

	/** 
    * Menu item: Load an image manually for matching.
    */
    void StartMatch();    
    
    /** 
    * Button item: Load a left eye image for target.
    */
    void openLeftEye1();

	/** 
    * Button item: Load a left eye image for query.
    */
    void openLeftEye2();

	/** 
    * Button item: Load a right eye image for target.
    */
    void openRightEye1();

	/** 
    * Button item: Load a right eye image for query.
    */
    void openRightEye2();
    
	/** 
    * Button item: Match the opened images.
    */
    void goMatch();

    /**
    * Menu item: Generate all four reports.
    * - matching scores.
    * - nonmatching scores.
    * - ROC scores.
    * - Reduced ROC scores.
    */
    void generateReports();


	
private:
	EyeDetection* eyeDetection;

	/// Images loaded and shown in imageWidget
	char* leftFileName1;
	char* leftFileName2;
	char* rightFileName1;
	char* rightFileName2;	
	/// select the data type options
	int gDataType1, gDataType2;
	int pDataType1, pDataType2;

	/**
	* Select the best quality and load an image.
	*
	* @param currentImg	Input frame from video
	* @param bestImg	(OUT) Used to return the image that was selected as
	* 					best quality image
    * @param bestIndex	(OUT) Used to return the selected index based on the best quality
    * @param bestImg	(OUT) Used to return the image that was selected as
    * 						best quality image
	* @param bestScore	(OUT) Used to return the best quality score 
	*/
    void selectBestEye(IplImage* currentImg, int index, int &bestIndex, IplImage*& bestImg, double& bestScore);
	

	/**
	* Select the pair image based on the best left or right quality and load an image.
	*
	* @param currentImg	    Input left or right frame from video
	* @param currentPairImg	Input pair eye frame from video
	* @param index			Detected eye index
	* @param bestIndex		(OUT) Used to return the selected index based on the best quality 
	* @param bestImg		(OUT) Used to return the image that was selected as
	* 						best quality image
	* @param bestScore		(OUT) Used to return the best quality score 
	*/
	void selectBestEyePairImage(IplImage* currentImg, IplImage* currentPairImg, int index, int& bestIndex, IplImage*& bestImg, double& bestScore);
   
   
		/**
	* Align the left and right eye position and then extract the eye region.
	*
	* @param currentLeftPairImg		The selected best eye-pair image for left eye
	* @param currentRightPairImg	The selected best eye-pair image for right eye
	* @param datatype 			Image Source
	* @param imageFormat		Image format to be saved
	* @param lImg				(OUT) Used to return the extracted left eye image 
	*                           after aligning the pupil position
	* @param rImg				(OUT) Used to return the extracted right eye image
	*							after aligning the pupil position
	*/ 
	void alignExractEyeImage(IplImage* currentLeftPairImg, 
							IplImage* currentRightPairImg,
							int dataType, 
							const char* imageFormat, 
							IplImage*& lImg, 
							IplImage*& rImg);

	/**
	* Draw an eye image on the widget.
	*
	* @param imgWidget	Image Widget that should display the image
	* @param title 		Title of the dialog box.
	* @param dir 		File's path
	* @return 			Filename of the displayed image
	*/  
	char* drawEyeImage(ImageWidget *imgWidget, const char* title, const char* dir);
	
	/**
	* Lets the user pick an image in a dialog and returns filename and image.
	*
	* @param title	Title of the dialog box.
	* @param dir 	Initial directory
	* @return 		Image and the image's filename.
	*
	* @see MATCHDATA
	*/
	MATCHDATA getEyeImage(const char* title, const char* dir);
	
	/**
	* Load an AVI format selected in a dialog.
	*/
	void loadVideoFile();

	/**
	* Detect eye regions and determine the best image quality image.
	*
	* @param fileName Input video name
	*/
	void detectEyeRegion(char* fileName);

	/**
	* Display the images selected as best quality images.
	*
	* @param fileName	Input video name to save the left or right image
	* @param indexLeft	Detected left eye index
	* @param indexRight	Detected right eye index
	* @param imgLeft	Input the left image selected as best quality
	* @param imgRight	Input the right image selected as best quality
	*/
	void drawBestImage(char* fileName,  int indexLeft, int indexRight, IplImage* imgLeft, IplImage* imgRight); 

	/** 
    * Clean up widgets.
    */
	void clearWidget();

	/**
	* Retrieve user input from command prompt.
	*
	* @param title Item that needs to be entered.
	*/
	int getString(std::string title);


	/**
	* Load the selected data type.
	*/
	int getDataType();

	/**
	 * Load the text depending on the selected data type.
	 *
	 * @param type Valid \c dataType values:
	 *               - \c 1 = classic still image
	 *               - \c 2 = video captured at a distance (distant-videos)
	 * @return string
	 */
	const char* txtDataType(int type);

};
#endif
