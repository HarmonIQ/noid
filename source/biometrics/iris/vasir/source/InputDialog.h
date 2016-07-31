#ifndef INPUTDIALOG_H
#define INPUTDIALOG_H

#include <QDialog>
#include <string>
#include "ui_InputDialog.h"

class InputDialog : public QDialog, protected Ui_InputDialog
{
    Q_OBJECT

public:

    /** Supported type */
    typedef enum {
        LEFT = 1,
        RIGHT = 2
    } EyePos;

    InputDialog(QWidget* parent = 0, Qt::WindowFlags flags = 0);
    virtual ~InputDialog();


private slots:
    void btnLoadingQueryClicked();

    void btnLoadingTargetClicked();

    void btnLoadingFilePathClicked();

    void btnAcceptClicked();


private:
    QString openFileName(QString path, const char* title, const char* fileType);
    char* getOutputFName(std::string currentPath, std::string str);
    char* cvtQStringToChar(QString qs);
    void freeBuffer(const char* buffer);
    void detectEmpty();    
    void allReports(const char* qList,
                    const char* tList,
                    const char* outputFName,
                    std::string path,
                    int pos, int idx, int skip,
                    int qDataCode, int tDataCode);

    EyePos pos;
    std::string stdPath;
    const char* queryListFile;
    const char* targetListFile;
    const char* outputFName;
    int qDataCode;
    int tDataCode;
    int eyePosition;
};

#endif // INPUTDIALOG_H
