#include "ModeDialog.h"


ModeDialog::ModeDialog( QWidget* parent /*= 0*/, Qt::WindowFlags flags /*= 0*/ ) : QDialog(parent, flags)
{
	setupUi(this);
	this->stillRadioButton->click();
}


ModeDialog::~ModeDialog()
{
}

void ModeDialog::modeButtonClicked() {
	QRadioButton* sender = qobject_cast<QRadioButton*>(QObject::sender());

	if (sender == this->stillRadioButton) 
	{
		this->mode = CLASSICAL_STILL;
	} else if (sender == this->videoRadioButton) 
	{
		this->mode = DISTANT_VIDEO;
	}
}

