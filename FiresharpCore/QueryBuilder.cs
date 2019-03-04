using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresharpCore
{
    public class QueryBuilder
    {
        private static Dictionary<string, object> InternalQuery = new Dictionary<string, object>();
        private readonly string InitialQuery;

        private readonly string LimitToFirstParam = "limitToFirst";
        private readonly string LimitToLastParam = "limitToLast";
        private readonly string ShallowParam = "shallow";
        private readonly string OrderByParam = "orderBy";
        private readonly string StartAtParam = "startAt";
        private readonly string EqualToParam = "equalTo";
        private readonly string FormatParam = "format";
        private readonly string PrintParam = "print";
        private readonly string EndAtParam = "endAt";
        private readonly string FormatVal = "export";

        private QueryBuilder(string initialQuery = "")
        {
            InitialQuery = initialQuery;
            InternalQuery = new Dictionary<string, object>();
        }

        public static QueryBuilder New(string initialQuery = "")
        {
            return new QueryBuilder(initialQuery);
        }

        public QueryBuilder StartAt(string value)
        {
            return AddToQueryDictionary(StartAtParam, value);
        }

        public QueryBuilder StartAt(long value)
        {
            return AddToQueryDictionary(StartAtParam, value);
        }

        public QueryBuilder EndAt(string value)
        {
            return AddToQueryDictionary(EndAtParam, value);
        }

        public QueryBuilder EndAt(long value)
        {
            return AddToQueryDictionary(EndAtParam, value);
        }

        public QueryBuilder EqualTo(string value)
        {
            return AddToQueryDictionary(EqualToParam, value);
        }

        public QueryBuilder OrderBy(string value)
        {
            return AddToQueryDictionary(OrderByParam, value);
        }

        public QueryBuilder LimitToFirst(int value)
        {
            return AddToQueryDictionary(LimitToFirstParam, value > 0 ? value.ToString() : string.Empty, skipEncoding: true);
        }

        public QueryBuilder LimitToLast(int value)
        {
            return AddToQueryDictionary(LimitToLastParam, value > 0 ? value.ToString() : string.Empty, skipEncoding: true);
        }

        public QueryBuilder Shallow(bool value)
        {
            return AddToQueryDictionary(ShallowParam, value ? "true" : string.Empty, skipEncoding: true);
        }

        public QueryBuilder Print(string value)
        {
            return AddToQueryDictionary(PrintParam, value, skipEncoding: true);
        }

        public QueryBuilder IncludePriority(bool value)
        {
            return AddToQueryDictionary(FormatParam, value ? FormatVal : string.Empty, skipEncoding: true);
        }

        private QueryBuilder AddToQueryDictionary(string parameterName, string value, bool skipEncoding = false)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                InternalQuery.Add(parameterName, skipEncoding ? value : EscapeString(value));
            }
            else
            {
                InternalQuery.Remove(StartAtParam);
            }

            return this;
        }

        private QueryBuilder AddToQueryDictionary(string parameterName, long value)
        {
            InternalQuery.Add(parameterName, value);
            return this;
        }

        private string EscapeString(string value)
        {
            return $"\"{Uri.EscapeDataString(value).Replace("%20", "+").Trim('\"')}\"";
        }

        public string ToQueryString()
        {
            if (!InternalQuery.Any() && !string.IsNullOrEmpty(InitialQuery))
            {
                return InitialQuery;
            }

            return !string.IsNullOrEmpty(InitialQuery)
                ? $"{InitialQuery}&{string.Join("&", InternalQuery.Select(pair => $"{pair.Key}={pair.Value}").ToArray())}"
                : string.Join("&", InternalQuery.Select(pair => $"{pair.Key}={pair.Value}").ToArray());
        }
    }
}