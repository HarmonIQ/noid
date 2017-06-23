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

namespace NoID.Match.Database
{
    internal class DBreezeWrapper
    {
        /// <summary>
        /// A manager providing operations on wallets.
        /// </summary>
        /// 
        private const string MINUTIAE_TABLE_NAME = "MinutiaeTable";
        private readonly string _databaseDirectory;
        private static DBreezeEngine _dBreezeEngine;
        private static DBreezeConfiguration _configDBreeze;
        private static Backup _backup;
        private uint _backupInterval;
        private List<Exception> _exceptionList;
        private uint _lastIndex = 0;

        public DBreezeWrapper(string databaseDirectory, uint backupInterval = 60)
        {
            _databaseDirectory = databaseDirectory;
            _backupInterval = backupInterval;
            DirectoryInfo directoryInfo = new DirectoryInfo(databaseDirectory);

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
                _backup.BackupFolderName = GetBackupDirectoryName();
            }
            catch (Exception ex)
            {
                throw ex;
            } 
        }

        ~DBreezeWrapper()
        {
            try
            {
                if (_configDBreeze != null)
                {
                    _configDBreeze.Dispose();
                }
                if (_dBreezeEngine != null)
                {
                    _dBreezeEngine.Dispose();
                }
                if (_backup != null)
                {
                    _dBreezeEngine.Dispose();
                } 
            }
            catch { }
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

        private byte[] GetMinutiaBytes(Template minutia)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, minutia);
            return ms.ToArray();
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
            catch(Exception ex)
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

        private string GetBackupDirectoryName()
        {
            string backupDatabaseDirectory = _databaseDirectory;
            try
            {
                // Directory Delimitor. Unix/Linux/Mac/ect = / and Windows = \
                string dirDelimitor = @"/"; // default to Unix
                if (OperationSystemString.Contains("Win") == true)
                {
                    dirDelimitor = @"\";
                }

                backupDatabaseDirectory += dirDelimitor + @"minutia.data.backups";
                DirectoryInfo directoryInfo = new DirectoryInfo(backupDatabaseDirectory);
                if (directoryInfo.Exists == false)
                {
                    directoryInfo.Create();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return backupDatabaseDirectory;
        }

        private static string OperationSystemString
        {
            get { return Environment.OSVersion.ToString(); }
        }
    }
}
