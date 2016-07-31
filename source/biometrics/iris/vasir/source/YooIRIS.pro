QT += core gui
CONFIG += console

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = YooIRIS
TEMPLATE = app

SOURCES += \
	main.cpp \
	CVImageSource.cpp \
	ImageWidget.cpp \
	ModeDialog.cpp \
	YooIRIS.cpp \
	YooUtility.cpp \
        InputDialog.cpp \
        VASIR/Shifts.cpp \
        VASIR/Normalization.cpp \
        VASIR/MatchingTemplate.cpp \
        VASIR/MatchAlg.cpp \
        VASIR/MasekLee.cpp \
        VASIR/ImageUtility.cpp \
        VASIR/ImageQuality.cpp \
        VASIR/GetHammingDistance.cpp \
        VASIR/GaborConvolve.cpp \
        VASIR/FindTwoPupilCircles.cpp \
        VASIR/FindPupilCircleNew.cpp \
        VASIR/FindIrisCircle.cpp \
        VASIR/FindHighLights.cpp \
        VASIR/FindEyelidCurve.cpp \
        VASIR/EyeRegionExtraction.cpp \
        VASIR/EyeDetection.cpp \
        VASIR/EncodeLee.cpp \
        VASIR/EdgeDensity.cpp \
        VASIR/CreateTemplate.cpp \
        VASIR/AlignLRPupilPos.cpp \
        MasekAlg/adjgamma.cpp \
        MasekAlg/canny.cpp \
        MasekAlg/circlecoordinates.cpp \
        MasekAlg/createiristemplate.cpp \
        MasekAlg/encode.cpp \
        MasekAlg/findcircle.cpp \
        MasekAlg/findline.cpp \
        MasekAlg/gaborconvolve.cpp \
        MasekAlg/gauss.cpp \
        MasekAlg/gethammingdistance.cpp \
        MasekAlg/houcircle.cpp \
        MasekAlg/hysthresh.cpp \
        MasekAlg/imread.cpp \
        MasekAlg/imwrite.cpp \
        MasekAlg/interp2.cpp \
        MasekAlg/linecoords.cpp \
        MasekAlg/mymat.cpp \
        MasekAlg/nonmaxsup.cpp \
        MasekAlg/normalizeiris.cpp \
        MasekAlg/radon.cpp \
        MasekAlg/saveiristemplate.cpp \
        MasekAlg/segmentiris.cpp \
        MasekAlg/shiftbits.cpp \
        MasekAlg/utility.cpp \
        Analysis/AnScores.cpp \
        Analysis/GetROC.cpp \
        Analysis/Utility.cpp

HEADERS += \
	CVImageSource.h \
	ImageWidget.h \
	ModeDialog.h \
	YooIRIS.h \
        InputDialog.h \
        VASIR/Shifts.h \
        VASIR/Normalization.h \
        VASIR/MatchingTemplate.h \
        VASIR/MatchAlg.h \
        VASIR/MasekLee.h \
        VASIR/ImageUtility.h \
        VASIR/ImageQuality.h \
        VASIR/GetHammingDistance.h \
        VASIR/FindPupilCircleNew.h \
        VASIR/FindIrisCircle.h \
        VASIR/FindHighLights.h \
        VASIR/FindEyelidCurve.h \
        VASIR/EyeRegionExtraction.h \
        VASIR/EyeDetection.h \
        VASIR/EncodeLee.h \
        VASIR/CreateTemplate.h \
        VASIR/AlignLRPupilPos.h \
        MasekAlg/global.h \
        MasekAlg/imread.h \
        MasekAlg/Masek.h \
        Analysis/Analysis.h \
        Analysis/GetROC.h \
        Analysis/Utility.h

FORMS   += \
	ModeDialog.ui \
	YooIRIS.ui \
        InputDialog.ui
# VASIR, Analysis and MasekAlg libaries
INCLUDEPATH += $$PWD/Analysis $$PWD/MasekAlg $$PWD/VASIR
DEPENDPATH += $$PWD/Analysis $$PWD/MasekAlg $$PWD/VASIR

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
                # Add Mac path to OpenCV library and header files here
                LIBS += -L/opt/local/lib/ \
                -lopencv_core \
                -lopencv_imgproc \
                -lopencv_highgui \
                -lopencv_objdetect \
                -lopencv_legacy

                INCLUDEPATH += /opt/local/include
}
else {
                # Add Linux path to OpenCV library and header files here
                INCLUDEPATH += /usr/local/include
                LIBS += -L/usr/local/include/opencv2 \
                -lopencv_core \
                -lopencv_video \
                -lopencv_videoio \
                -lopencv_videostab \
                -lopencv_objdetect \
                -lopencv_imgproc \
                -lopencv_highgui \
                -lopencv_imgcodecs \
                -lopencv_flann \
                -lopencv_features2d \
                -lopencv_calib3d \
                -lopencv_ml \
                -lopencv_photo \
                -lopencv_shape
}


