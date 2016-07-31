#ifndef MASEKLEE_H
#define MASEKLEE_H
#include "Masek.h"

class MasekLee
{
public:
    MasekLee();
    virtual ~MasekLee();
    static int getvalue(unsigned char *str, int len, int reverse);
    static void setvalue(unsigned char *str, int value, int len);
    static void gettypebytes(unsigned char *str, int reverse, int *type, int *bytes, int *next);
    static int loadtemplate(const char *file, Masek::MAT_DATA** m_data);
    static int savetemplate(const char *file, Masek::MAT_DATA* m_data);

    /**
     * Returns the coordinates of a circle in an image using the Hough
     * transform and Canny edge detection to create the edge map.
     *
     * The output depends on the input:
     * - \b circleiris centre coordinates and radius of the detected iris
     *      boundary
     * - \b circlepupil centre coordinates and radius of the detected pupil
     *      boundary
     * - \b imagewithnoise original eye image, but with location of noise
     *      marked with NaN values
     *
     * @param image     the image in which to find circles
     * @param lradius   lower radius to search for
     * @param uradius   upper radius to search for
     * @param scaling   scaling factor for speeding up the Hough transform
     * @param sigma     amount of Gaussian smoothing to apply for creating
     *                  edge map.
     * @param hithres   threshold for creating edge map
     * @param lowthres  threshold for connected edges
     * @param vert      vertical edge contribution (0-1)
     * @param horz      horizontal edge contribution (0-1)
     * @param _row      (OUT) center x-coordinate
     * @param _col      (OUT) center x-coordinate
     * @param _r        (OUT) radius
     */
    static void findcircle(Masek::IMAGE* image, int lradius, int uradius,
                            double scaling, double sigma, double hithres,
                            double lowthres, double vert, double horz,
                            int* _row, int* _col, int* _r);

    /**
     * Returns the coordinates of a line in an image using the linear Hough
     * transform and Canny edge detection to create the edge map.
     *
     * @param image the input image
     * @param lines (OUT) parameters of the detected line in polar form.\n
     *              0=cos(t), 1=sin(t), 2=r
     * @return      number of lines
     */
    static int findline(Masek::IMAGE* image, double* *lines);

    /**
     * Returns the x y coordinates of positions along a line.
     *
     * @param lines an array containing parameters of the line in form
     * @param row   height of the image, needed so that x y coordinates are
     *              within the image boundary
     * @param cols  width of the image
     * @param x     (OUT) x-coordinates
     * @param y     (OUT) y-coordinates
     */
    static void linescoords(double* lines, int row, int col, int* x, int* y);

private:
   // Masek *masek;
};

#endif // MASEKLEE_H
