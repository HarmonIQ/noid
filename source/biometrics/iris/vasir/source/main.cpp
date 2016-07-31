
#include <QApplication>
#include "YooIRIS.h"

int main(int argc, char** argv)
{
 
  QApplication app( argc, argv );

	// create a new instance of YooIRIS
	YooIRIS mainWindow;	
	mainWindow.show();

	// Enters the main event loop and waits until exit() is called 
	// or the main widget is destroyed, and Returns the value that 
	// was set via to exit() (which is 0 if exit() is called via quit()). 
	return app.exec();
}
