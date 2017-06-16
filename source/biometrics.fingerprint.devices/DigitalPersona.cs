// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
using DPUruNet;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

namespace NoID.Biometrics.Managers
{
    public class DigitalPersona : BiometricDevice
    {
        private string _name = "U are U 4500";
        private string _manufacturer = "Digital Persona";
        private string _vendorID = "";
        private string _productID = "";
        private string _driver = "";
        private string _driverLocation = "";

        private ReaderCollection _readers;
        private Reader _reader;
        private Exception _exception;
        private bool reset = false;

        public DigitalPersona()
        {
            _readers = ReaderCollection.GetReaders();
            if (_readers.Count > 0)
            {
                _reader = _readers[0];
                if (!OpenReader())
                {
                    _exception = new Exception("");
                }
                else
                {
                    if (!(_reader is null))
                    {
                        _name = _reader.Description.Id.ProductName;
                        _manufacturer = _reader.Description.Id.VendorName;
                        _vendorID = _reader.Description.Id.VendorId.ToString();
                        _productID = _reader.Description.Id.ProductId.ToString();
                    }
                    else
                    {
                        _exception = new Exception("DigitalPersona reader is null.");
                    }
                }
            }
        }

        ~DigitalPersona() { }
        public override string Name
        {
            get { return _name; }
        }
        public override string Manufacturer
        {
            get { return _manufacturer; }
        }
        public override string VendorID
        {
            get { return _vendorID; }
        }
        public override string ProductID
        {
            get { return _productID; }
        }
        public override string Driver
        {
            get { return _driver; }
        }
        public override string DriverLocation
        {
            get { return _driverLocation; }
        }

        public Exception Exception
        {
            get { return _exception; }
            private set { _exception = value; }
        }
        public Reader CurrentReader
        {
            get { return _reader; }
        }

        public override int Count
        {
            get
            {
                if (!(_readers is null))
                {
                    return _readers.Count;
                }
                else
                {
                    return -1;
                }
            }
        }

        private bool OpenReader()
        {
            reset = false;
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            // Open reader
            try
            {
                result = _reader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);
            }
            catch (Exception ex)
            {
                _exception = ex;
                reset = true;
                return false;
            }
            
            if (result != Constants.ResultCode.DP_SUCCESS)
            {
                reset = true;
                return false;
            }

            return true;
        }

        public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            // Activate capture handler
            _reader.On_Captured += new Reader.CaptureCallback(OnCaptured);

            // Call capture
            if (!CaptureFingerAsync())
            {
                return false;
            }
            return true;
        }

        public bool CheckCaptureResult(CaptureResult captureResult)
        {
            if (captureResult.Data == null || captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
            {
                if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    _exception = new Exception(captureResult.ResultCode.ToString());
                }

                // Send message if quality shows fake finger
                if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                {
                    _exception = new Exception("DigitalPersona.CheckCaptureResult() Quality Error - " + captureResult.Quality);
                }
                return false;
            }
            return true;
        }

        public void CancelCaptureAndCloseReader(Reader.CaptureCallback OnCaptured)
        {
            if (_reader != null)
            {
                _reader.CancelCapture();

                // Dispose of reader handle and unhook reader events.
                _reader.Dispose();

                if (reset)
                {
                    _reader = null;
                }
            }
        }

        private bool CaptureFingerAsync()
        {
            try
            {
                GetStatus();

                Constants.ResultCode captureResult = _reader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, _reader.Capabilities.Resolutions[0]);
                if (captureResult != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    _exception = new Exception("Error in DigitalPersona.CaptureFingerAsync(): " + captureResult);
                }

                return true;
            }
            catch (Exception ex)
            {
                _exception = ex;
                return false;
            }
        }

        private void GetStatus()
        {
            Constants.ResultCode result = _reader.GetStatus();

            if ((result != Constants.ResultCode.DP_SUCCESS))
            {
                reset = true;
                _exception = new Exception("Error in DigitalPersona.GetStatus(): " + result);
            }

            if ((_reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
            {
                Thread.Sleep(50);
            }
            else if ((_reader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                _reader.Calibrate();
            }
            else if ((_reader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                _exception = new Exception("Reader Status - " + _reader.Status.Status);
            }
        }
    }
}
