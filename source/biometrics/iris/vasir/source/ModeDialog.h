/*********************************************************//**
** @file ModeDialog.h
** Controller - functions to select datatype with the Qt UI.
** 
** @date 10/2011
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
#ifndef MODE_DIALOG_H
#define MODE_DIALOG_H

#include "ui_ModeDialog.h"

/**
 * Qt Dialog to select the appropriate datatype.
 */
class ModeDialog : public QDialog, protected Ui_ModeDialog
{
	Q_OBJECT
public:

	/** Supported type */
	typedef enum {
		CLASSICAL_STILL = 88,
		DISTANT_VIDEO = 99
	} Mode;

    ModeDialog(QWidget* parent = 0, Qt::WindowFlags flags = 0);
	virtual ~ModeDialog();
	Mode getMode() { return this->mode; }

public slots:
	/** 
    * Event handler - triggered when radio button item is clicked.
	* Sets the datatype.
    */
	void modeButtonClicked();

private:
	Mode	mode;
};
#endif // !MODE_DIALOG_H
