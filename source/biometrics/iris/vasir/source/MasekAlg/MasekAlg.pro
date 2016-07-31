#-------------------------------------------------
#
# Project created by QtCreator 2013-08-20T15:34:00
#
#-------------------------------------------------

QT       -= core gui

TARGET = MasekAlg
TEMPLATE = lib
CONFIG += staticlib

# Also generate static library
#CONFIG += staticlib

DEFINES += MASEKALG_LIBRARY

SOURCES +=  adjgamma.cpp \
            canny.cpp \
            circlecoordinates.cpp \
            createiristemplate.cpp \
            encode.cpp \
            findcircle.cpp \
            findline.cpp \
            gaborconvolve.cpp \
            gauss.cpp \
            gethammingdistance.cpp \
            houcircle.cpp \
            hysthresh.cpp \
            imread.cpp \
            imwrite.cpp \
            interp2.cpp \
            linecoords.cpp \
            mymat.cpp \
            nonmaxsup.cpp \
            normalizeiris.cpp \
            radon.cpp \
            saveiristemplate.cpp \
            segmentiris.cpp \
            shiftbits.cpp \
    utility.cpp


HEADERS +=  \
            Masek.h \
            global.h \
            imread.h

unix:!symbian {
    maemo5 {
        target.path = /opt/usr/lib
    } else {
        target.path = /usr/lib
    }
    INSTALLS += target
}
