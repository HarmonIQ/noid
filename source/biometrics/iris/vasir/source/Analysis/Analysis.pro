QT -= core gui

TARGET = Analysis
TEMPLATE = lib
CONFIG += staticlib

# Not used at the moment
DEFINES += ANALYSIS_LIBRARY

SOURCES += AnScores.cpp\
   GetROC.cpp\
   Utility.cpp

HEADERS += analysis.h\
	GetROC.h\
	Utility.h

# MasekAlg, VASIR dependency

CONFIG(debug) {
	LIBS += -L$$PWD/../Build/VASIR/debug/ -lVASIR
} else {
	LIBS += -L$$PWD/../Build/VASIR/release/ -lVASIR
}

INCLUDEPATH += $$PWD/../MasekAlg $$PWD/../VASIR
DEPENDPATH += $$PWD/../MasekAlg $$PWD/../VASIR

# OpenCV dependency


win32 {
        # Add Windows path to OpenCV library and header files here

        #LIBS += -LC:\\QtVSCV\\OpenCV-2.3.1\\opencv_binaries\\lib\\Debug \
        LIBS += -LC:\\OpenCV-2.3.1\\opencv_binaries\\lib\\Debug \
            -lopencv_core231d \
            -lopencv_imgproc231d \
            -lopencv_highgui231d \
            -lopencv_objdetect231d \
            -lopencv_legacy231d

        #INCLUDEPATH += C:\\QtVSCV\\OpenCV-2.3.1\\build\\include
        INCLUDEPATH += C:\\OpenCV-2.3.1\\build\\include
}
else:mac {
    # Add Mac or Linux path to OpenCV library and header files here
        LIBS += -L/opt/local/lib/ \
        -lopencv_core \
        -lopencv_imgproc \
        -lopencv_highgui \
        -lopencv_objdetect \
        -lopencv_legacy

        INCLUDEPATH += /opt/local/include
}
