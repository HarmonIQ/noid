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
    public enum ErrorSeverity { User, Application, Environment, Chain, Protocol }

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

    public class NoIDException : System.Exception
    {
        public const int DEFAULT_EXCEPTION_HRESULT = -351100030;
        public const int ApplicationError = 351100;
        public const string DefaultMessageNotFound = @"NoID Exception: Message ({0}) not found in ""{1}"".";
        public const string DefaultManifestNotFound = @"NoID Exception: Message ({0}) Manifest not found for BaseName ""{1}"".";

        public NoIDException(string message) : base(message)
        {
            _code = ApplicationError;
            _severity = ErrorSeverity.Application;
            HResult = DEFAULT_EXCEPTION_HRESULT;
        }

        public NoIDException(int errorCode, string message) : base(message)
        {
            _code = errorCode;
            _severity = ErrorSeverity.Application;
            HResult = DEFAULT_EXCEPTION_HRESULT;
        }

        public NoIDException(ErrorSeverity severity, int errorCode, string message) : base(message)
        {
            _code = errorCode;
            _severity = severity;
            HResult = DEFAULT_EXCEPTION_HRESULT;
        }

        public NoIDException(ErrorSeverity severity, int errorCode, string message, Exception innerException) : base(message, innerException)
        {
            _code = errorCode;
            _severity = severity;
            HResult = DEFAULT_EXCEPTION_HRESULT;
        }

        public NoIDException(Exception exception) : base(exception.Message)
        {
            HResult = DEFAULT_EXCEPTION_HRESULT;
            NoIDException dataphorException = exception as NoIDException;
            _serverContext = exception.StackTrace;
            if (dataphorException != null)
            {
                _code = dataphorException.Code;
                _severity = dataphorException.Severity;
                _details = dataphorException.GetDetails();
            }
            else
            {
                _code = ApplicationError;
                _severity = ErrorSeverity.Application;
            }
        }

        public NoIDException(Exception exception, Exception innerException) : base(exception.Message, innerException)
        {
            HResult = DEFAULT_EXCEPTION_HRESULT;
            NoIDException dataphorException = exception as NoIDException;
            _serverContext = exception.StackTrace;
            if (dataphorException != null)
            {
                _code = dataphorException.Code;
                _severity = dataphorException.Severity;
                _details = dataphorException.GetDetails();
            }
            else
            {
                _code = ApplicationError;
                _severity = ErrorSeverity.Application;
            }
        }

        protected NoIDException(ResourceManager resourceManager, int errorCode, ErrorSeverity severity, Exception innerException, params object[] paramsValue) : base(paramsValue == null ? GetMessage(resourceManager, errorCode) : String.Format(GetMessage(resourceManager, errorCode), paramsValue), innerException)
        {
            _code = errorCode;
            _severity = severity;
            HResult = DEFAULT_EXCEPTION_HRESULT;
        }
            
        private int _code;
        public int Code 
        { 
            get { return _code; } 
            set { _code = value; }
        }

        private ErrorSeverity _severity;
        public ErrorSeverity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        private string _details;
        public string Details
        {
            get { return _details; }
            set { _details = value; }
        }

        private string _serverContext;
        public string ServerContext
        {
            get { return _serverContext; }
            set { _serverContext = value; }
        }

        public string CombinedMessages
        {
            get
            {
                string message = String.Empty;
                Exception exception = this;
                while (exception != null)
                {
                    message += exception.InnerException != null ? exception.Message + ", " : exception.Message;
                    exception = exception.InnerException;
                }
                return message;
            }
        }

        public virtual string GetDetails()
        {
            return _details != null ? _details : String.Empty;
        }

        public string GetServerContext()
        {
            return _serverContext != null ? _serverContext : (StackTrace != null ? StackTrace : String.Empty);
        }

        public static string GetMessage(ResourceManager resourceManager, int errorCode)
        {
            string result = null;
            try
            {
                result = resourceManager.GetString(errorCode.ToString());
            }
            catch
            {
                result = String.Format(DefaultManifestNotFound, errorCode, resourceManager.BaseName);
            }

            if (result == null)
                result = String.Format(DefaultMessageNotFound, errorCode, resourceManager.BaseName);
            return result;
        }

        public NoIDException(ErrorSeverity severity, int code, string message, string details, string serverContext, NoIDException innerException) : base(message, innerException)
        {
            _severity = severity;
            _code = code;
            _details = details;
            _serverContext = serverContext;
            HResult = DEFAULT_EXCEPTION_HRESULT;
        }
    }

    public class BaseException : NoIDException
    {
        public enum Codes : int
        {
            /// <summary>Error code 350100: "UnableToConnectToDatabase ({0})."</summary>
            UnableToConnectToDatabase = 350100,
            /// <summary>Error code 350100: "UnableToConnectToPatientHub ({0})."</summary>
            UnableToConnectToPatientHub = 350101,
            /// <summary>Error code 350102: "UnableToConnectToBlochChain ({0})."</summary>
            UnableToConnectToBlochChain = 350102
        };

        // Resource manager for this exception class
        private static ResourceManager _resourceManager = new ResourceManager("Alphora.Dataphor.BaseException", typeof(BaseException).Assembly);

        // Default constructor
        public BaseException(Codes errorCode) : base(_resourceManager, (int)errorCode, ErrorSeverity.Application, null, null) {}
        public BaseException(Codes errorCode, params object[] paramsValue) : base(_resourceManager, (int)errorCode, ErrorSeverity.Application, null, paramsValue) {}
        public BaseException(Codes errorCode, Exception innerException) : base(_resourceManager, (int)errorCode, ErrorSeverity.Application, innerException, null) {}
        public BaseException(Codes errorCode, Exception innerException, params object[] paramsValue) : base(_resourceManager, (int)errorCode, ErrorSeverity.Application, innerException, paramsValue) {}
        public BaseException(Codes errorCode, ErrorSeverity severity) : base(_resourceManager, (int)errorCode, severity, null, null) {}
        public BaseException(Codes errorCode, ErrorSeverity severity, params object[] paramsValue) : base(_resourceManager, (int)errorCode, severity, null, paramsValue) {}
        public BaseException(Codes errorCode, ErrorSeverity severity, Exception innerException) : base(_resourceManager, (int)errorCode, severity, innerException, null) {}
        public BaseException(Codes errorCode, ErrorSeverity severity, Exception innerException, params object[] paramsValue) : base(_resourceManager, (int)errorCode, severity, innerException, paramsValue) {}

        public BaseException(ErrorSeverity severity, int code, string message, string details, string serverContext, NoIDException innerException) 
            : base(severity, code, message, details, serverContext, innerException)
        {
        }
    }

    public class AbortException : Exception
    {
        public AbortException() : base() {}
    }

    public class AggregateException : NoIDException
    {
        public AggregateException(ErrorList errors) : this(errors, ErrorSeverity.Application, null) { }
        public AggregateException(ErrorList errors, ErrorSeverity severity) : this(errors, severity, null) { }
        public AggregateException(ErrorList errors, ErrorSeverity severity, Exception innerException) : base(severity, NoIDException.ApplicationError, errors.ToString(), innerException)
        {
            _errors = errors;
        }

        private ErrorList _errors;
        public ErrorList Errors { get { return _errors; } }
    }
}

