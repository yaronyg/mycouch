using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using EnsureThat;
using MyCouch.Extensions;
using MyCouch.Net;

namespace MyCouch.Requests.Factories
{
    public abstract class HttpRequestFactoryBase
    {
        protected readonly IConnection Connection;

        protected HttpRequestFactoryBase(IConnection connection)
        {
            Ensure.That(connection, "connection").IsNotNull();

            Connection = connection;
        }

        protected virtual HttpRequest CreateFor<T>(HttpMethod method, string url) where T : Request
        {
            return new HttpRequest(method, url).SetRequestType(typeof(T));
        }

        protected virtual bool HasValue(object value)
        {
            return value != null;
        }

        protected virtual bool HasValue(string value)
        {
            return value != null;
        }

        protected virtual bool HasValue(object[] value)
        {
            return value != null && value.Any();
        }

        protected virtual bool HasValue(IList<string> value)
        {
            return value != null && value.Any();
        }

        protected virtual string FormatValue(object value)
        {
            //Since NetFX does not support IConvertible, we need to treat individual types
            //as short, int, long..., ...

            if (value is string)
                return FormatValue(value as string);

            if (value is Enum)
                return FormatValue(value.ToString());

            if (value is Array)
                return FormatValues(value as object[]);

            if (value is short)
                return value.To<short>().ToString(MyCouchRuntime.NumberFormat);

            if (value is int)
                return value.To<int>().ToString(MyCouchRuntime.NumberFormat);

            if (value is long)
                return value.To<long>().ToString(MyCouchRuntime.NumberFormat);

            if (value is float)
                return value.To<float>().ToString(MyCouchRuntime.NumberFormat);

            if (value is double)
                return value.To<double>().ToString(MyCouchRuntime.NumberFormat);

            if (value is decimal)
                return value.To<decimal>().ToString(MyCouchRuntime.NumberFormat);

            if (value is ushort)
                return value.To<ushort>().ToString(MyCouchRuntime.NumberFormat);

            if (value is uint)
                return value.To<uint>().ToString(MyCouchRuntime.NumberFormat);

            if (value is ulong)
                return value.To<ulong>().ToString(MyCouchRuntime.NumberFormat);

            if (value is DateTime)
                return FormatValue(value.To<DateTime>().ToString(MyCouchRuntime.DateTimeFormatPattern));

            if (value is bool)
                return value.ToString().ToLower();

            return value.ToString();
        }

        protected virtual string FormatValue(string value)
        {
            return string.Format("\"{0}\"", value);
        }

        protected virtual string FormatValues(object[] value)
        {
            return string.Format("[{0}]", string.Join(",", value.Select(v => FormatValue(v))));
        }

        protected virtual string FormatValues(IList<string> values)
        {
            return string.Format("[{0}]", string.Join(",", values.Select(v => FormatValue(v))));
        }
    }
}