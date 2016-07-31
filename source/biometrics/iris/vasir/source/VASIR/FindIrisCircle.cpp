#include "FindIrisCircle.h"
#include "Masek.h"
#include "ImageUtility.h"

void FindIrisCircle::doDetect(IplImage *img, int rPupil, int rIrisMax,
                                double scaling, double lowThres, double highThres,
                                int* destVal)
{
    // Values returned by the Hough Transform
    int rowIris, colIris, rIris;

    //Initialize
    destVal[0] = 1; //x
    destVal[1] = 1; //y
    destVal[2] = 1; //radius

    int rIrisMin = rPupil + (rPupil/4);

    // (Development purposes)
    if(rIrisMin > rIrisMax)
    {
        printf("Error => lIrisRadius > uIrisRadius: %d\n", rIrisMax);
        rIrisMin = rIrisMax;//then store the max radius
    }

    // Convert the IplImage to IMAGE
    Masek::IMAGE * eyeImg = ImageUtility::convertIplToImage(img);

	// Find the iris boundaries for both video and still images
    Masek *masek = NULL;
    masek->findcircle(eyeImg, rIrisMin, rIrisMax, scaling, 2, highThres, lowThres, 1.00, 0.00, &rowIris, &colIris, &rIris);
    delete masek;
    free(eyeImg->data);
    free(eyeImg);

    // Set the return values of the iris circle
    destVal[0] = colIris;//x
    destVal[1] = rowIris;//y
    destVal[2] = rIris;//r
    //printf("Iris is %d %d %d\n", destVal[0], destVal[1], destVal[2]);

    /// \todo Is this debugging information really necessary?
    if(destVal[0] < 2)
    {
        printf("Failed to load the iris X center position\n");
        destVal[0] = img->width/2;
    }

    /// \todo Is this debugging information really necessary?
    if(destVal[1] < 2)
    {
        printf("Failed to load the iris Y center position\n");
        destVal[1] = img->height/2;

    }
    if(destVal[2] < 2 || destVal[2] > rIrisMax)
    {
        printf("Failed to load the iris's radius\n");
        destVal[2] = rIrisMax;
    }
}

CvPoint FindIrisCircle::getOriginPoints(CvPoint xyPupil, CvPoint xyIris, CvPoint setPt, int val)
{
    // Calculate the difference between pupil center and iris center
    int diffX, diffY;
    diffX = xyIris.x - setPt.x;
    diffY = xyIris.y - setPt.y;

    // Limit iris center distance from pupil center points based on the given argument
    if(diffX > val || diffX < -val)
        diffX = 0;
    if(diffY > val || diffY < -val)
      diffY = 0;

    // Find the iris center within the original image
    xyIris.x = xyPupil.x + diffX;
    xyIris.y = xyPupil.y + diffY;
    return xyIris;
}
