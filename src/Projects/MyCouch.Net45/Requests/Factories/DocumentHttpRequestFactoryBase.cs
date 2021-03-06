﻿using System;
using System.Collections.Generic;
using System.Linq;
using MyCouch.Extensions;

namespace MyCouch.Requests.Factories
{
    public abstract class DocumentHttpRequestFactoryBase : HttpRequestFactoryBase
    {
        protected DocumentHttpRequestFactoryBase(IConnection connection) : base(connection) {}

        protected virtual string GenerateRequestUrl(string id = null, string rev = null, bool batch = false)
        {
            var queryParameters = new List<string>();

            if (rev != null)
                queryParameters.Add(string.Format("rev={0}", rev));

            if (batch)
                queryParameters.Add("batch=ok");

            return string.Format("{0}/{1}{2}",
                Connection.Address,
                id != null ? Uri.EscapeDataString(id) : string.Empty,
                queryParameters.Any() ? string.Join("&", queryParameters).PrependWith("?") : string.Empty);
        }
    }
}