## VASIR Home Page: [VASIR](http://www.nist.gov/itl/iad/ig/vasir.cfm) 

## VASIR (Video-based Automatic System for Iris Recognition)
VASIR is a NIST-implemented iris recognition system designed for both conventional iris images and iris images collected at a distance.

VASIR is a fully automated system for video-based iris recognition, capable of handling videos that were 1) captured under less-constrained environmental conditions and 2) contained other features (e.g., face, head, hair, neck, shoulder) apart from the eye/iris region, such as a person walking through a portal at a distance. The VASIR system was designed, developed, and optimized to be robust—to address the challenge of recognizing a person using the human iris in less-than-optimal environments.

Although VASIR was developed with less-constrained video-based iris recognition in mind, VASIR also robustly accommodates traditional constrained still-based iris recognition. VASIR hence supports multiple matching scenarios:
* Less-constrained (non-ideal) video to less constrained (non-ideal) video (VV),
* Less-constrained (non-ideal) video to constrained (ideal) still image (VS), and
* Constrained (ideal) still to constrained (ideal) still image (SS) iris recognition.

In VV matching, the extracted iris region of video frames taken at distance is matched to frames from a different video sequence of the same person. The VS matching scenario means that the video frame (e.g., taken at a distance using an IOM system) is compared to classical still-images (e.g., via a LG2200 system)—matching samples captured by a different device. SS matching is the traditional iris recognition scenario in which a classical still image is matched against other classical still images of the same person—all of which were captured by the same device.

VASIR has the capacity to load both still images and video sequences, to automatically detect and extract the eye region, and to subsequently assess and select the best quality iris image from NIR face-visible video captured at a certain distance. After this process, VASIR carries out a comprehensive image comparison analysis that in turn yields a state-of-the-art individual verification.

The VASIR system has into three primary modules:
* Image Acquisition,
* Video Processing, and
* Iris Recognition.

## VASIR_Architecture
In the Image Acquisition module, VASIR loads the image or video sequence. For evaluation, we use publicly available datasets.

In the Video Processing module, VASIR automatically distinguishes the eye region from face, hair, ear, and shoulder that may be visible in video frames and subsequently extracts the left/right iris sub-images separately. VASIR then automatically calculates an image quality score of the extracted iris sub-images. Based on the quality score, the highest quality iris images—one for left and one for right—are automatically chosen out of all available frames. The Video Processing module consists of four sub-components: Eye Region Detection, Eye Region Image Extraction, Image Quality Measurement, and Best Quality Image Selection.

The Iris Recognition module is then fed either the resulting iris images from the Video Processing module or the loaded still image. For both video and still iris images, VASIR segments the iris region based on VASIR’s segmentation approach. The segmented iris regions are then extracted and normalized based on polar coordinates transformations and bilinear interpolation. Next, VASIR extracts the features from the normalized iris images and encodes the extracted features as binary strings along with a VASIR-developed noise-masking scheme. At the end, VASIR matches the encoded biometric iris template to other existing iris templates. The Iris Recognition module consists of four components: segmentation, normalization, feature extraction/encoding, and similarity distance metric. All three module procedures are completely auto­matic.

To assess VASIR's performance and practical feasibility, the Iris Challenge Evaluation (ICE) dataset and the Multiple Biometric Grand Challenge (MBGC) dataset were employed, with results as described in the publications below (Please cite the following paper when using the VASIR system).

* Yooyoung Lee, Ross J. Micheals, P. J. Phillips, James J. Filliben, “VASIR: An Open-Source Research Platform for Advanced Iris Recognition Technologies", Journal of Research of NIST, Volume 118, p218-259, 2013 

The detailed evaluation protocol and performance for each dataset are described in Section 6 of this paper. 

Currently, the VASIR source code beta version 2.2 and its user guide are available. This source code is incomplete. Note that you may get a known warning or memory related messages.

We plan to update each component of VASIR incrementally, after evaluating its performance.
