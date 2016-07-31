#include <qpen.h>
#include <qimage.h>
#include <qpainter.h>
#include <qcolor.h>
#include <qlist.h>

#include <cstring>

#include "ImageWidget.h"

using namespace std;

ImageWidget::ImageWidget(QWidget* parent)
 : QWidget(parent)
{
	// Initialize
    this->imgDataCopy = NULL;
	this->image = NULL;
    this->rectPen = new QPen(QColor::fromRgb(255, 0, 0));
    this->rectPen->setWidth(3);
}

ImageWidget::~ImageWidget()
{
    delete this->rectPen;
    releaseImage();
}

void ImageWidget::releaseImage() {
    if (image != NULL) {
        delete image;
        image = NULL;
        if (imgDataCopy != NULL) {
            delete[] imgDataCopy;
            imgDataCopy = NULL;
        }
    }
}

void ImageWidget::setImage(IplImage* img, bool copy)
{  
	this->rects.clear();
    releaseImage();

    iplToQImage(img, this->image, copy ? &(this->imgDataCopy) : NULL);
}

void ImageWidget::setImage(QImage* image)
{
	this->rects.clear();
    releaseImage();

    this->image = image;
}

void ImageWidget::reset() 
{
    releaseImage();
}

void ImageWidget::addRectange(CvRect& rect)
{
	this->rects.append(QRect(rect.x, rect.y, rect.width, rect.height));
}

void ImageWidget::paintEvent(QPaintEvent *event)
{
	QPainter painter(this);

	if (image != NULL) 
	{
    
		// Alternative: use "scale" Qt method
		float scaling = qMin((float)this->width() / image->width(),
                         (float)this->height()/image->height());

		// Do not upscale
		if (scaling > 1.0)
		{
			scaling = 1.0;
		}

        //NOTE: comment out here if you want to have fast speed
		// Draw resized image
		QRectF srcRect(0, 0, image->width(), image->height());
		QRectF targetRect(0, 0, image->width() * scaling, image->height() * scaling);
        painter.drawImage(targetRect, *image, srcRect);

		// Draw the rectangles  
        painter.setPen(*rectPen);
		painter.setBrush(Qt::NoBrush);
		for (int i = 0; i < this->rects.size(); ++i)
		{
			painter.drawRect(this->rects[i].x() * scaling,this->rects[i].y() * scaling, 
					 this->rects[i].width() * scaling, this->rects[i].height() * scaling);	  
		}
	}

	// Border of the retangle
    painter.setPen(QColor::fromRgb(160,160,160));
    painter.setBrush(Qt::NoBrush);
    painter.drawRect(QRect(0, 0, width() - 1, height() - 1));
}

QImage ImageWidget::iplToQImage(IplImage* img, QImage*& dest, uchar** destData)
{
	QImage::Format fmt;
	bool isGrayscale = false;
	QImage ret;
	int i;

	if(img->depth == IPL_DEPTH_8U)
	{
        if(img->nChannels == 3)//24 bit RGB
        {
           fmt = QImage::Format_RGB888;
            //mt = QImage::Format_RGB32;
        }
		else if(img->nChannels == 1) //Grayscale
		{
		  fmt = QImage::Format_Indexed8;
		  isGrayscale = true;
		}
		else if(img->nChannels == 2) //16bit RGB   
		  fmt = QImage::Format_RGB16;  
		else //Invalid format  
		  fmt = img->alphaChannel ?  QImage::Format_ARGB32 : QImage::Format_RGB32;
	 
        // Copy data
        const uchar* data;
        if (destData != NULL) {
            *destData = new uchar[img->imageSize];
            memcpy(*destData, img->imageDataOrigin, img->imageSize);
            data = *destData;
        } else {
            data = (const uchar*)img->imageDataOrigin;
        }

        // Create QImage
        dest = new QImage(data, img->width, img->height, fmt);
	}
	else
	{
		dest = new QImage();
	}

	// Generate colormap if grayscale
	if(isGrayscale)
	{
        dest->setColorCount(256);
		for(i=0; i < 256; i++)
		{
		  dest->setColor(i, qRgb(i, i, i));
		}
	}
	
	// Bottom-right - need to mirror
	ret = *dest;
	if(img->origin)
	{
		if(!isGrayscale)
		{
		  ret = ret.rgbSwapped();
		}
		ret = ret.mirrored(false, true);
	}

	return ret;
}
