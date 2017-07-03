﻿// Copyright © 2016-2017 NoID Developers. All rights reserved.
// Distributed under the MIT software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace NoID.Database.Wrappers
{

    /// <summary>
    /// NoID Wrapper class for MongoDB.
    /// TODO: Thread Async call to MongoDB
    /// </summary>

    public class MongoDBWrapper
    {
        private string _noidMongoAddress;
        private string _sparkMongoAddress;
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        private List<Exception> _exceptions = new List<Exception>();

        public MongoDBWrapper(string noidMongoAddress, string sparkMongoAddress)
        {
            try
            {
                _noidMongoAddress = noidMongoAddress;
                _sparkMongoAddress = sparkMongoAddress;
                _client = new MongoClient(_noidMongoAddress);
                _database = _client.GetDatabase("NoID"); //Creates the NoID MongoDB database if it doesn't exist.
                CreateUnCappedCollection();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        ~MongoDBWrapper()
        {
            try
            {
                _client = null;
                _database = null;
            }
            catch { }
        }

        public List<Exception> Exceptions
        {
            get { return _exceptions; }
        }

        void CreateUnCappedCollection()
        {
            _database.CreateCollectionAsync("SessionQueue", new CreateCollectionOptions
            {
                Capped = false
            });
        }

        public bool UpdateSessionQueueRecord(string _id, string approvalStatus, string reviewUser, string computerName)
        {
            bool successful = false;
            try
            {
                //TODO: Add index on SparkReference and ApprovalStatus
                //TODO: Update document with one line.
                IMongoCollection<SessionQueue> collection = _database.GetCollection<SessionQueue>("SessionQueue");
                // Update Patient Status Type
                var update = Builders<SessionQueue>.Update.Set(a => a.ApprovalStatus, approvalStatus);
                var result = collection.UpdateOneAsync(model => model.SparkReference == _id, update);
                // Update Accept Deny Date
                update = Builders<SessionQueue>.Update.Set(a => a.AcceptDenyDate, DateTime.UtcNow);
                result = collection.UpdateOneAsync(model => model.SparkReference == _id, update);
                // Update Review User
                update = Builders<SessionQueue>.Update.Set(a => a.ReviewUser, reviewUser);
                result = collection.UpdateOneAsync(model => model.SparkReference == _id, update);
                // Update Session Computer Name
                update = Builders<SessionQueue>.Update.Set(a => a.SessionComputerName, reviewUser);
                result = collection.UpdateOneAsync(model => model.SparkReference == _id, computerName);

                successful = true;
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
            }
            return successful;
        }


        public bool AddPendingPatient(SessionQueue seq)
        {
            bool successful = false;
            try
            {
                IMongoCollection<SessionQueue> collection = _database.GetCollection<SessionQueue>("SessionQueue");
                collection.InsertOneAsync(seq);
                successful = true;
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
            }
            return successful;
        }

        public List<SessionQueue> GetPendingPatients()
        {
            List<SessionQueue> listSessionQueue = null;
            try 
            {
                var _collection = _database.GetCollection<SessionQueue>("SessionQueue");
                var filter = Builders<SessionQueue>.Filter.Eq("ApprovalStatus", "pending");
                listSessionQueue = _collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
            }
            return listSessionQueue;
        }
    }
}
