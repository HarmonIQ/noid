#include "InputDialog.h"
#include "ui_InputDialog.h"
#include <QFileDialog>

#include <iostream>
using namespace std;
#include "Analysis.h"
#include "GetROC.h"
#include "Utility.h"


InputDialog::InputDialog( QWidget* parent /*= 0*/, Qt::WindowFlags flags /*= 0*/ ) : QDialog(parent, flags)
{
    setupUi(this);

    stdPath = "";
    queryListFile = NULL;
    targetListFile = NULL;
    outputFName = NULL;
    qDataCode = 0;
    tDataCode = 0;
    eyePosition = 0;
    this->radioLeft->click();


}

InputDialog::~InputDialog()
{
    if(queryListFile != NULL)
        freeBuffer(queryListFile);
    if(targetListFile != NULL)
        freeBuffer(targetListFile);
    if(outputFName != NULL)
        freeBuffer(outputFName);
}

void InputDialog::freeBuffer(const char* buffer)
{
    delete[] buffer;
}


//Dialog: open a dialog to select the file
QString InputDialog::openFileName(QString path, const char* title, const char* fileType)
{
    QString fileName = NULL;
    if(path.isEmpty())
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

    return fileName;


}

char* InputDialog::getOutputFName(string currentPath, string str)
{
    std::string fName = currentPath+str;
    char* myFileName = new char[fName.size()+1];
    strcpy(myFileName, fName.c_str());
    return myFileName;
}

char* InputDialog::cvtQStringToChar(QString qs)
{
    if (!qs.isEmpty())
    {
        QByteArray enc =qs.toUtf8(); //or toLocal8Bit or whatever
        //then allocate enough memory:
        char* myFileName = new char[enc.size()+1];
        //and copy it
        strcpy(myFileName, enc.data());
        return myFileName;
    }
    return NULL;
}

void InputDialog::btnLoadingFilePathClicked()
{
    QString pathName = QFileDialog::getExistingDirectory(this, tr("Open Directory"),
                                                         "/home",
                                                         QFileDialog::ShowDirsOnly
                                                         | QFileDialog::DontResolveSymlinks);
    this->txtInputFilePath->setText(pathName+"//");
    queryListFile = cvtQStringToChar(pathName);
}



void InputDialog::btnLoadingQueryClicked()
{
    QString path = this->txtInputFilePath->toPlainText();
    QString fName = openFileName(path, "Open file for the list of query files", "TXT files (*.txt)");
    this->txtFileList4Query->setText(fName);
    queryListFile = cvtQStringToChar(fName);
}

void InputDialog::btnLoadingTargetClicked()
{
    QString path = this->txtInputFilePath->toPlainText();
    QString fName = openFileName(path, "Open file for the list of target files", "TXT files (*.txt)");
    this->txtFileList4Target->setText(fName);
    targetListFile = cvtQStringToChar(fName);
}


void InputDialog::btnAcceptClicked()
{

    stdPath = this->txtInputFilePath->toPlainText().toStdString();
    //this->txtInputFilePath->setTextBackgroundColor(QColor(0, 255, 0));

    queryListFile = cvtQStringToChar(this->txtFileList4Query->toPlainText());
    targetListFile = cvtQStringToChar(this->txtFileList4Target->toPlainText());
    qDataCode = this->txtCode4Query->toPlainText().toInt();
    tDataCode = this->txtCode4Target->toPlainText().toInt();
    QString qsOutputFName = this->txtOutputFileName->toPlainText();
    outputFName = getOutputFName(stdPath, qsOutputFName.toStdString());

    if(this->radioLeft->isChecked())
        this->pos = LEFT;
    else if(this->radioRight->isChecked())
        this->pos = RIGHT;

    eyePosition = this->pos;

    if(!this->txtInputFilePath->toPlainText().isEmpty()
            && !this->txtCode4Query->toPlainText().isEmpty()
            && !this->txtCode4Target->toPlainText().isEmpty()
            && !this->txtFileList4Query->toPlainText().isEmpty()
            && !this->txtFileList4Target->toPlainText().isEmpty()
            && !this->txtOutputFileName->toPlainText().isEmpty())
    {
        int index = 1;
        int row2skip = 3;
        allReports(queryListFile, targetListFile, outputFName,
                   stdPath, eyePosition, index, row2skip,
                   qDataCode, tDataCode);


        this->accept();

    }
    else
    {
        cout << "Enter inputs in the yellow box" << endl;

        detectEmpty();
    }

}

void InputDialog::detectEmpty()
{
    QPalette p;

    if(this->txtInputFilePath->toPlainText().isEmpty())
    {
        p = this->txtInputFilePath->palette();
        p.setColor(QPalette::Base, QColor(255, 255, 0));
        this->txtInputFilePath->setPalette(p);
    }
    if(this->txtCode4Query->toPlainText().isEmpty())
    {
        p = this->txtCode4Query->palette();
        p.setColor(QPalette::Base, QColor(255, 255, 0));
        this->txtCode4Query->setPalette(p);
    }
    if(this->txtCode4Target->toPlainText().isEmpty())
    {
        p = this->txtCode4Target->palette();
        p.setColor(QPalette::Base, QColor(255, 255, 0));
        this->txtCode4Target->setPalette(p);
    }
    if(this->txtFileList4Query->toPlainText().isEmpty())
    {
        p = this->txtFileList4Query->palette();
        p.setColor(QPalette::Base, QColor(255, 255, 0));
        this->txtFileList4Query->setPalette(p);
    }
    if(this->txtFileList4Target->toPlainText().isEmpty())
    {
        p = this->txtFileList4Target->palette();
        p.setColor(QPalette::Base, QColor(255, 255, 0));
        this->txtFileList4Target->setPalette(p);
    }

    if(this->txtOutputFileName->toPlainText().isEmpty())
    {
        p = this->txtOutputFileName->palette();
        p.setColor(QPalette::Base, QColor(255, 255, 0));
        this->txtOutputFileName->setPalette(p);
    }
}


void InputDialog::allReports(const char* qList,
                               const char* tList,
                               const char* outputFName,
                               string path,
                               int pos, int idx, int skip,
                               int qDataCode, int tDataCode)
{
    Analysis *an = NULL;
    //STOP ==> NEED TO CHECK HERE
    char* matchOutputFName = an->newFileName(outputFName, idx, "_Matching", ".txt");
    char* nonmatchOutputFName = an->newFileName(outputFName, idx, "_NonMatching", ".txt");
    char* rocOutputFName = an->newFileName(outputFName, idx, "_ROC", ".txt");
    char* reducedRocOutputFName = an->newFileName(outputFName, idx, "_reducedROC", ".txt");

    if(qList == NULL || tList == NULL)
    {
        cout <<"Query or Target list files are empty" << endl;
        return;
    }
    else
    {
        cout << "Loading matching scores..." << endl;
        an->doNewMatchListFile(qList, tList, matchOutputFName, path,
                    pos, qDataCode, tDataCode);

        cout << "Loading nonmatching scores..." << endl;
        an->doNewRandomNonMatch(qList, tList, nonmatchOutputFName, path,
                    pos, qDataCode, tDataCode);
    }

    GetROC::rocResult(-1, skip, matchOutputFName, nonmatchOutputFName, rocOutputFName, pos, idx);
    cout << "Calculating ROC scores are done..." << endl;

    //the matching score is the threshold
    GetROC::reducedScores1(2, skip, rocOutputFName, reducedRocOutputFName, pos, idx);
    cout << "Reducing ROC scores are done..." << endl;

    delete[] matchOutputFName;
    delete[] nonmatchOutputFName;
    delete[] rocOutputFName;
    delete[] reducedRocOutputFName;
    delete an;
    cout << "Index " <<idx << " is done!!!" << endl;

}

