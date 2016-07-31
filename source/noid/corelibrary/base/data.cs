// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Linq;
using Couchbase;
using Couchbase.N1QL;
using Couchbase.Views;
using Couchbase.Core;
using Couchbase.IO;
using Newtonsoft.Json;

namespace NoID.Base
{
    /// <summary>Base Data Access Class For Couchbase Server.</summary>
    public class DataAccess
    {
        public DataAccess ()
        {
        }

        ~DataAccess ()
        {
        }
    }

    public interface IRepository<T> where T : IEntity
    {
        void Save(T entity);
        void Remove(T entity);
        IEnumerable<T> Select(IList<string> keys);
        IEnumerable<T> Select(IQueryRequest queryRequest);
        IEnumerable<T> Select(IViewQuery viewQuery);
        T Find(string key);
    }

    public static class EntityExtensions
    {
        public static IDocument<T> Wrap<T>(this T entity) where T : IEntity
        {
            return new Document<T>
            {
                Id = entity.Id,
                Cas = entity.Cas,
                Content = entity
            };
        }

        public static T UnWrap<T>(this IDocument<T> document) where T : IEntity
        {
            var entity = document.Content;
            entity.Cas = document.Cas;
            entity.Id = document.Id;
            return entity;
        }
    }
    
    public static class DocumentResultExtensions
    {
        public static void ThrowIfNotSuccess<T>(this IDocumentResult<T> result)
        {
            if (result.Success) return;
            switch (result.Status)
            {
            case ResponseStatus.KeyNotFound:
                throw new DocumentNotFoundException(result, result.Document.Id);
            case ResponseStatus.AuthenticationError:
                throw new CouchbaseAuthenticationException(result);
            case ResponseStatus.ItemNotStored:
            case ResponseStatus.VBucketBelongsToAnotherServer:
            case ResponseStatus.OutOfMemory:
            case ResponseStatus.InternalError:
            case ResponseStatus.Busy:
            case ResponseStatus.TemporaryFailure:
                throw new CouchbaseServerException(result, result.Document.Id);
            case ResponseStatus.ValueTooLarge:
                throw new CouchbaseDataException(result);
            case ResponseStatus.ClientFailure:
            case ResponseStatus.OperationTimeout:
                throw new CouchbaseClientException(result, result.Document.Id);
            default:
                throw new ArgumentOutOfRangeException();
            }
        }

        public static void ThrowIfNotSuccess(this IOperationResult result, string key)
        {
            if (result.Success) return;
            switch (result.Status)
            {
            case ResponseStatus.KeyNotFound:
                throw new DocumentNotFoundException(result, key);
            case ResponseStatus.AuthenticationError:
                throw new CouchbaseAuthenticationException(result);
            case ResponseStatus.ItemNotStored:
            case ResponseStatus.VBucketBelongsToAnotherServer:
            case ResponseStatus.OutOfMemory:
            case ResponseStatus.InternalError:
            case ResponseStatus.Busy:
            case ResponseStatus.TemporaryFailure:
                throw new CouchbaseServerException(result, key);
            case ResponseStatus.ValueTooLarge:
                throw new CouchbaseDataException(result);
            case ResponseStatus.ClientFailure:
            case ResponseStatus.OperationTimeout:
                throw new CouchbaseClientException(result, key);
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class Repository<T> : IRepository<T> where T : IEntity
    {
        public Repository(IBucket bucket)
        {
            Bucket = bucket;
        }

        protected  IBucket Bucket { get; set; }

        public void Save(T entity)
        {
            var result = Bucket.Upsert(entity.Wrap());
            result.ThrowIfNotSuccess();
        }

        public void Remove(T entity)
        {
            var result = Bucket.Remove(entity.Wrap());
            result.ThrowIfNotSuccess(entity.Id);
        }

        public IEnumerable<T> Select(IQueryRequest queryRequest)
        {
            var results = Bucket.Query<T>(queryRequest);
            if (!results.Success)
            {
                var message = JsonConvert.SerializeObject(results.Errors);
                throw new QueryRequestException(message, results.Status);
            }
            return results.Rows;
        }

        public IEnumerable<T> Select(IViewQuery viewQuery)
        {
            var results = Bucket.Query<T>(viewQuery);
            if (!results.Success)
            {
                var message = results.Error;
                throw new ViewRequestException(message, results.StatusCode);
            }
            return results.Values;
        }

        public T Find(string key)
        {
            var result = Bucket.GetDocument<T>(key);
            result.ThrowIfNotSuccess();
            return result.Document.UnWrap();
        }

        public IEnumerable<T> Select(IList<string> keys)
        {
            throw new System.NotImplementedException();
        }
    }
}