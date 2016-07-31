// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Resources;
using System.Net;
using System.Runtime.Serialization;
using Couchbase;
using Couchbase.N1QL;
using Couchbase.IO;
using Couchbase.IO.Operations;

namespace NoID.Base
{
    public class CouchbaseAuthenticationException : CouchbaseDataException
    {
        public CouchbaseAuthenticationException()
        {
        }

        public CouchbaseAuthenticationException (string message)
            : base(message)
        {
        }

        public CouchbaseAuthenticationException (string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CouchbaseAuthenticationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CouchbaseAuthenticationException(IOperationResult result)
            : base(result)
        {
        }

        public CouchbaseAuthenticationException(IDocumentResult result)
            : base(result)
        {
        }
    }

    public class CouchbaseServerException : CouchbaseDataException
    {
        public CouchbaseServerException()
        {
        }

        public CouchbaseServerException(string message)
            : base(message)
        {
        }

        public CouchbaseServerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CouchbaseServerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CouchbaseServerException(IOperationResult result, string key)
            : base(result, key)
        {
        }

        public CouchbaseServerException(IDocumentResult result, string key)
            : base(result, key)
        {
        }
    }

    public class CouchbaseClientException : CouchbaseDataException
    {
        public CouchbaseClientException()
        {
        }

        public CouchbaseClientException(string message)
            : base(message)
        {
        }

        public CouchbaseClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CouchbaseClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CouchbaseClientException(IOperationResult result, string key)
            : base(result, key)
        {
        }

        public CouchbaseClientException(IDocumentResult result, string key)
            : base(result, key)
        {
        }
    }

    public class CouchbaseDataException : Exception
    {
        public CouchbaseDataException()
        {
        }

        public CouchbaseDataException(IOperationResult result)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
        }

        public CouchbaseDataException(IDocumentResult result)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
        }

        public CouchbaseDataException(IOperationResult result, string key)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
            Key = key;
        }

        public CouchbaseDataException(IDocumentResult result, string key)
            : this(result.Message, result.Exception)
        {
            Status = result.Status;
            Key = key;
        }

        public CouchbaseDataException(string message)
            : base(message)
        {
        }

        public CouchbaseDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CouchbaseDataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public ResponseStatus Status { get; set; }

        public string Key { get; set; }
    }

    public class DocumentNotFoundException : CouchbaseDataException
    {
        public DocumentNotFoundException()
        {
        }

        public DocumentNotFoundException(string message)
            : base(message)
        {
        }

        public DocumentNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DocumentNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public DocumentNotFoundException(IOperationResult result, string key)
            : base(result, key)
        {
        }

        public DocumentNotFoundException(IDocumentResult result, string key)
            : base(result, key)
        {
        }
    }

    public class QueryRequestException : Exception
    {
        public QueryRequestException()
        {
        }

        public QueryRequestException(string message, QueryStatus queryStatus)
            : base(message)
        {
            QueryStatus = queryStatus;
        }

        public QueryRequestException(string message)
            : base(message)
        {
        }

        public QueryRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected QueryRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public QueryStatus QueryStatus { get; set; }
    }

    public class ViewRequestException : Exception
    {
        public ViewRequestException()
        {
        }

        public ViewRequestException(string message)
            : base(message)
        {
        }

        public ViewRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }


        public ViewRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ViewRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public HttpStatusCode StatusCode { get; set; }
    }
}

