// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using Couchbase.N1QL;
using Couchbase.Views;

namespace NoID.Base.Data
{
    public interface IRepository<T> where T : IEntity
    {
        void Save(T entity);
        void Remove(T entity);
        IEnumerable<T> Select(IList<string> keys);
        IEnumerable<T> Select(IQueryRequest queryRequest);
        IEnumerable<T> Select(IViewQuery viewQuery);
        T Find(string key);
    }
}