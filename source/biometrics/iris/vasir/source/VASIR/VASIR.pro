QT -= core gui

TARGET = VASIR
TEMPLATE = lib
CONFIG += staticlib

# Not used at the moment
DEFINES += VASIR_LIBRARY

SOURCES += AlignLRPupilPos.cpp\
	CreateTemplate.cpp\
	EdgeDensity.cpp\
	EncodeLee.cpp\
	EyeDetection.cpp\
	EyeRegionExtraction.cpp\
	FindEyelidCurve.cpp\
	FindHighLights.cpp\
	FindIrisCircle.cpp\
	FindPupilCircleNew.cpp\
	FindTwoPupilCircles.cpp\
	GaborConvolve.cpp\
	GetHammingDistance.cpp\
	ImageQuality.cpp\
	ImageUtility.cpp\
	MatchAlg.cpp\
	MatchingTemplate.cpp\
	Normalization.cpp\
	Shifts.cpp \
    MasekLee.cpp

HEADERS +=\
	AlignLRPupilPos.h\
	CreateTemplate.h\
	EncodeLee.h\
	EyeDetection.h\
	EyeRegionExtraction.h\
	FindEyelidCurve.h\
	FindHighLights.h\
	FindIrisCircle.h\
	FindPupilCircleNew.h\
	GetHammingDistance.h\
	ImageQuality.h\
	ImageUtility.h\
	MasekLee.h\
	MatchAlg.h\
	MatchingTemplate.h\
	Normalization.h\
	Shifts.h

# MasekAlg dependency

CONFIG(debug) {
	LIBS += -L$$PWD/../Build/MasekAlg/debug/ -lMasekAlg
} else {
	LIBS += -L$$PWD/../Build/MasekAlg/release/ -lMasekAlg
}

INCLUDEPATH += $$PWD/../MasekAlg
DEPENDPATH += $$PWD/../MasekAlg

# OpenCV dependency

#LIBS += \
#	-lopencv_core \
#	-lopencv_imgproc \
#	-lopencv_highgui \
#	-lopencv_objdetect \
#	-lopencv_legacy

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


QT -= core gui

TARGET = VASIR
TEMPLATE = lib
CONFIG += staticlib

# Not used at the moment
DEFINES += VASIR_LIBRARY

SOURCES += AlignLRPupilPos.cpp\
	CreateTemplate.cpp\
	EdgeDensity.cpp\
	EncodeLee.cpp\
	EyeDetection.cpp\
	EyeRegionExtraction.cpp\
	FindEyelidCurve.cpp\
	FindHighLights.cpp\
	FindIrisCircle.cpp\
	FindPupilCircleNew.cpp\
	FindTwoPupilCircles.cpp\
	GetHammingDistance.cpp\
        GaborConvolve.cpp\
	ImageQuality.cpp\
	ImageUtility.cpp\
        Normalization.cpp\
	MatchAlg.cpp\
	MatchingTemplate.cpp\
	Shifts.cpp

HEADERS +=\
	AlignLRPupilPos.h\
	CreateTemplate.h\
	EncodeLee.h\
	EyeDetection.h\
	EyeRegionExtraction.h\
	FindEyelidCurve.h\
	FindHighLights.h\
	FindIrisCircle.h\
	FindPupilCircleNew.h\
	GetHammingDistance.h\
	ImageQuality.h\
	ImageUtility.h\
        Normalization.h\
	MatchAlg.h\
	MatchingTemplate.h\
	Shifts.h

# MasekAlg dependency

CONFIG(debug) {
	LIBS += -L$$PWD/../Build/MasekAlg/debug/ -lMasekAlg
} else {
	LIBS += -L$$PWD/../Build/MasekAlg/release/ -lMasekAlg
}

INCLUDEPATH += $$PWD/../MasekAlg
DEPENDPATH += $$PWD/../MasekAlg

# OpenCV dependency

#LIBS += \
#	-lopencv_core \
#	-lopencv_imgproc \
#	-lopencv_highgui \
#	-lopencv_objdetect \
#	-lopencv_legacy

win32 {
        # Add Windows path to OpenCV library and header files here
        #LIBS += -LC:\\QtVSCV\\OpenCV-2.4.5\\opencv_binaries\\lib\\Debug \
        #    -lopencv_core245d \
        #    -lopencv_imgproc245d \
        #    -lopencv_highgui245d \
        #    -lopencv_objdetect245d \
        #    -lopencv_legacy245d

        #INCLUDEPATH += C:\\QtVSCV\\OpenCV-2.4.5\\build\\include

        LIBS += -LC:\\QtVSCV\\OpenCV-2.3.1\\opencv_binaries\\lib\\Debug \
            -lopencv_core231d \
            -lopencv_imgproc231d \
            -lopencv_highgui231d \
            -lopencv_objdetect231d \
            -lopencv_legacy231d

        INCLUDEPATH += C:\\QtVSCV\\OpenCV-2.3.1\\build\\include
}
else:mac {
    # Add Mac or Linux path to OpenCV library and header files here

        LIBS += -LC:/QtVSCV/OpenCV-2.3.1/opencv_binaries/lib/Debug \
            -lopencv_core231d \
            -lopencv_imgproc231d \
            -lopencv_highgui231d \
            -lopencv_objdetect231d \
            -lopencv_legacy231d

        INCLUDEPATH += C:/QtVSCV/OpenCV-2.3.1/build/include
}
else{
    # Add Linux path to OpenCV library and header files here

        INCLUDEPATH += /usr/include
}

