// Copyright (c) 2016 NoID Developers
// Distributed under the MIT/X11 software license, see the accompanying
// file COPYING or http://www.opensource.org/licenses/mit-license.php.

using System;
using Couchbase;

namespace NoID.Base.Data
{
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
}

