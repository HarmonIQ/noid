// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.IO;
using DBreeze;
using System.Collections.Generic;
using SourceAFIS.Templates;
using DBreeze.Storage;
using DBreeze.DataTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace NoID.Match.Database
{
    internal class DBreezeWrapper
    {
        /// <summary>
        /// A wrapper for DBreeze used to store minutias templates
        /// Estimated 140 GB for 20 million minutia templates with the backups.
        /// </summary>
        ///

        #region "Private Variables"

        private const string MINUTIAE_TABLE_NAME = "MinutiaeTable";
        private readonly string _databaseDirectory;
        private readonly string _backupDirectoryPath;
        private static DBreezeEngine _dBreezeEngine;
        private static DBreezeConfiguration _configDBreeze;
        private static Backup _backup;
       
        private uint _backupInterval;
        private List<Exception> _exceptionList;
        private uint _lastIndex = 0;

        #endregion

        #region "Constructor/Destructor"

        public DBreezeWrapper(string databaseDirectory, string backupDirectoryPath, uint backupInterval = 300)
        {
            _databaseDirectory = databaseDirectory;
            _backupDirectoryPath = backupDirectoryPath;
            _backupInterval = backupInterval;

            if (IsDirectoryLocked(databaseDirectory) == true)
            {
                throw new Exception("Error in DBreezeWrapper::DBreezeWrapper.  Database directory is locked!");
            }
            if (IsDirectoryLocked(backupDirectoryPath) == true)
            {
                throw new Exception("Error in DBreezeWrapper::DBreezeWrapper.  Backup database directory is locked!");
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(databaseDirectory);

            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
            }

            directoryInfo = new DirectoryInfo(backupDirectoryPath);
            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
            }

            try
            {
                _configDBreeze = new DBreezeConfiguration();
                _configDBreeze.DBreezeDataFolderName = databaseDirectory;
                _dBreezeEngine = new DBreezeEngine(_configDBreeze);
                _backup = _configDBreeze.Backup;
                _backup.IncrementalBackupFileIntervalMin = _backupInterval;
                _backup.BackupFolderName = BackupDirectoryPath;
            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            } 
        }

        ~DBreezeWrapper()
        {
            Dispose();
        }

        #endregion

        #region "Public Functions"

        public void Dispose()
        {
            cleanDBreezeObjects();
        }

        public List<Template> GetMinutiaList()
        {
            List<Template> listMinutiaTemplates = new List<Template>();
            try
            {
                if (_dBreezeEngine != null)
                {
                    using (var tranaction = _dBreezeEngine.GetTransaction())
                    {
                        int item = 0;
                        foreach (var row in tranaction.SelectForward<byte[], byte[]>(MINUTIAE_TABLE_NAME))
                        {
                            Template result;
                            using (var stream = new MemoryStream(row.Value))
                            {
                                BinaryFormatter deserializer = new BinaryFormatter();
                                result = (Template)deserializer.Deserialize(stream);
                            }
                            listMinutiaTemplates.Add(result);
                            _lastIndex += 1;
                            item += 1;
                        }
                    }
                }
                else
                {
                    if (_exceptionList == null)
                    {
                        _exceptionList = new List<Exception>();
                    }
                    _exceptionList.Add(new Exception("DBreezeWrapper.GetMinutiaList error. DBreezeEngine is null."));
                }
            }
            catch (Exception ex)
            {
                if (_exceptionList == null)
                {
                    _exceptionList = new List<Exception>();
                }
                _exceptionList.Add(ex);
            }
            return listMinutiaTemplates;
        }
        public bool AddMinutia(Template newMinutia)
        {
            bool result = false;
            try
            {
                if (_dBreezeEngine != null)
                {
                    byte[] byteMinutia = GetMinutiaBytes(newMinutia);
                    using (var tran = _dBreezeEngine.GetTransaction())
                    {
                        _lastIndex += 1;
                        tran.Insert<uint, byte[]>(MINUTIAE_TABLE_NAME, _lastIndex, byteMinutia);
                        tran.Commit();
                    }
                    result = true;
                }
                else
                {
                    if (_exceptionList == null)
                    {
                        _exceptionList = new List<Exception>();
                    }
                    _exceptionList.Add(new Exception("DBreezeWrapper.AddMinutia error. DBreezeEngine is null."));
                }
            }
            catch (Exception ex)
            {
                if (_exceptionList == null)
                {
                    _exceptionList = new List<Exception>();
                }
                _exceptionList.Add(ex);
            }
            return result;
        }

        public bool BackupDatabase()
        {
            bool result = false;
            try
            {
                if (_configDBreeze != null && _backup != null && _dBreezeEngine != null)
                {
                    _backup.Flush();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (_exceptionList == null)
                {
                    _exceptionList = new List<Exception>();
                }
                _exceptionList.Add(ex);
            }
            return result;
        }

        public bool DeleteMatchDatabase()
        {
            bool result = false;
            try
            {
                cleanDBreezeObjects();
                DirectoryInfo databaseDirInfo = new DirectoryInfo(_databaseDirectory);
                DirectoryInfo backupDirInfo = new DirectoryInfo(_backupDirectoryPath);
                if (backupDirInfo.Exists == true)
                {
                    backupDirInfo.Delete(true);
                }
                if (databaseDirInfo.Exists == true)
                {
                    databaseDirInfo.Delete(true);
                }
                result = true;
            }
            catch (Exception ex)
            {
                _exceptionList.Add(ex);
            }
            return result;
        }
        #endregion

        #region "Private Functions"

        bool IsDirectoryLocked(string directoryPath)
        {
            bool result = true;
            try
            {
                string testFilePath = directoryPath + @"\test.write.lock";
                FileInfo testFile = new FileInfo(testFilePath);
                if (testFile.Exists == true)
                {
                    FileStream fs = testFile.Create();
                    byte[] testBytes = Encoding.ASCII.GetBytes("NoID testing write access for ths directory: " + directoryPath + "/n");
                    fs.Write(testBytes, 0, testBytes.Length);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                    testFile.Delete();
                }
                testFile = null;
                result = false;
            }
            catch (System.IO.IOException ioEx)
            {
                // file used by another process or other IO Exception
                _exceptionList.Add(ioEx);
            }
            catch (Exception ex)
            {
                _exceptionList.Add(ex);
            }
            return result;
        }

        void cleanDBreezeObjects()
        {
            try
            {
                if (_configDBreeze != null)
                {
                    _configDBreeze.Dispose();
                }
                if (_backup != null)
                {
                    _backup.Dispose();
                }
                if (_dBreezeEngine != null)
                {
                    _dBreezeEngine.Dispose();
                }
            }
            catch { }
        }

        private byte[] GetMinutiaBytes(Template minutia)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, minutia);
            return ms.ToArray();
        }

        #endregion

        #region "Public Properties"

        public string DatabaseDirectory
        {
            get { return _databaseDirectory; }
        }

        public string BackupDirectoryPath
        {
            get { return _backupDirectoryPath;  }
        }

        #endregion

    }
}
