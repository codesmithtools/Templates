using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Web.Http.Filters;
using Newtonsoft.Json;

namespace CodeSmith.Data.Web.Http
{
    /// <summary>
    /// http://blog.longle.net/2012/04/13/teleriks-html5-kendo-ui-grid-with-server-side-paging-sorting-filtering-with-mvc3-ef4-dynamic-linq/
    /// </summary>
    public class PagingAndFiltering : ActionFilterAttribute
    {
        public PagingAndFiltering(int resultLimit = 20)
        {
            ResultLimit = resultLimit;
        }

        protected int ResultLimit { get; set; }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            HttpRequestMessage request = actionExecutedContext.Request;
            HttpResponseMessage response = actionExecutedContext.Response;

            IQueryable query;
            if (response == null || !response.TryGetContentValue(out query)) 
                return;

            System.Collections.Specialized.NameValueCollection queryItems = request.RequestUri.ParseQueryString();
            string json = queryItems.AllKeys.FirstOrDefault(k => k.StartsWith("{"));
            if(String.IsNullOrWhiteSpace(json))
                return;

            var model = JsonConvert.DeserializeObject<KendoRequestModel>(json);
            if (model.Sort != null && model.Sort.Any())
            {
                try
                {
                    query = model.Sort.Aggregate(query, (current, sort) => current.OrderBy(sort.Field + " " + sort.Direction));
                }
                catch (Exception) { }
            }
            else
            {
                // The method 'Skip' is only supported for sorted input in LINQ to Entities. The method 'OrderBy' must be called before the method 'Skip'
                query = query.OrderBy("1");
            }

            if (model.Filter != null)
            {
                try
                {
                    ProcessFilters(model.Filter, ref query);
                } catch (Exception) { }
            }

            if (!String.IsNullOrWhiteSpace(model.Select)) {
                query = query.Select(String.Format("new ({0})", model.Select));
            }

            int totalCount = query.Count();

            if (model.Skip.HasValue)
                query = query.Skip(model.Skip.Value);

            query = query.Take(model.Take.HasValue ? model.Take.Value : ResultLimit);

            var enumerator = query.GetEnumerator();
            var l = new List<dynamic>();
            while (enumerator.MoveNext())
                l.Add(enumerator.Current);

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK,
                                                                                          new {
                                                                                                  Data = l,
                                                                                                  TotalCount = totalCount
                                                                                              });
        }

        internal static void ProcessFilters(GridFilter filter, ref IQueryable queryable)
        {
            var whereClause = string.Empty;
            var filters = filter.Filters;
            var parameters = new List<object>();
            for (int i = 0; i < filters.Count; i++)
            {
                GridFilter f = filters[i];

                if (f.Filters == null)
                {
                    if (i == 0)
                        whereClause += BuildWherePredicate(f, parameters) + " ";
                    if (i != 0)
                        whereClause += ToLinqOperator(filter.Logic) + BuildWherePredicate(f, parameters) + " ";
                    if (i == (filters.Count - 1))
                    {
                        TrimWherePredicate(ref whereClause);
                        queryable = queryable.Where(whereClause, parameters.ToArray());
                    }
                }
                else
                    ProcessFilters(f, ref queryable);
            }
        }

        internal static string TrimWherePredicate(ref string whereClause)
        {
            switch (whereClause.Trim().Substring(0, 2).ToLower())
            {
                case "&&":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
                case "||":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
            }

            return whereClause;
        }

        internal static string BuildWherePredicate(GridFilter filter, List<object> parameters)
        {
            var parameterIndex = parameters.Count;
            switch (filter.Operator.ToLower())
            {
                case "eq":
                case "neq":
                case "gte":
                case "gt":
                case "lte":
                case "lt":
                    DateTime date;
                    if (DateTime.TryParse(filter.Value, out date))
                    {
                        parameters.Add(date.Date);
                        return String.Format("EntityFunctions.TruncateTime(" + filter.Field + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                    }
                    int number;
                    if (int.TryParse(filter.Value, out number))
                    {
                        parameters.Add(number);
                        return String.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                    }

                    if (!String.IsNullOrEmpty(filter.Value) && filter.Value.StartsWith("'") && filter.Value.EndsWith("'"))
                        parameters.Add(filter.Value.Substring(1, filter.Value.Length - 2));
                    else
                        parameters.Add(filter.Value);

                    return String.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                case "startswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".StartsWith(" + "@" + parameterIndex + ")";
                case "endswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".EndsWith(" + "@" + parameterIndex + ")";
                case "contains":
                    parameters.Add(filter.Value);
                    return filter.Field + ".Contains(" + "@" + parameterIndex + ")";
                default:
                    throw new ArgumentException("This operator is not yet supported for this Grid", filter.Operator);
            }
        }

        internal static string ToLinqOperator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq":
                    return " == ";
                case "neq":
                    return " != ";
                case "gte":
                    return " >= ";
                case "gt":
                    return " > ";
                case "lte":
                    return " <= ";
                case "lt":
                    return " < ";
                case "or":
                    return " || ";
                case "and":
                    return " && ";
                default:
                    return null;
            }
        }

        internal static PropertyInfo GetNestedProp(String name, Type type)
        {
            PropertyInfo info = null;
            foreach (var prop in name.Split('.'))
            {
                info = type.GetProperty(prop);
                type = info.PropertyType;
            }
            return info;
        }

        [DataContract]
        internal class KendoRequestModel
        {
            [DataMember(Name = "take")]
            public int? Take { get; set; }

            [DataMember(Name = "skip")]
            public int? Skip { get; set; }

            [DataMember(Name = "page")]
            public int? Page { get; set; }

            [DataMember(Name = "pageSize")]
            public int? PageSize { get; set; }

            [DataMember(Name = "select")]
            public string Select { get; set; }

            [DataMember(Name = "filter")]
            public GridFilter Filter { get; set; }

            [DataMember(Name = "group")]
            public List<object> Group { get; set; }

            [DataMember(Name = "sort")]
            public List<GridSort> Sort { get; set; }
        }

        [DataContract]
        internal class GridFilter
        {
            [DataMember(Name = "operator")]
            public string Operator { get; set; }

            [DataMember(Name = "field")]
            public string Field { get; set; }

            [DataMember(Name = "value")]
            public string Value { get; set; }

            [DataMember(Name = "logic")]
            public string Logic { get; set; }

            [DataMember(Name = "filters")]
            public List<GridFilter> Filters { get; set; }
        }

        [DataContract]
        internal class GridSort
        {
            [DataMember(Name = "field")]
            public string Field { get; set; }

            [DataMember(Name = "dir")]
            public string Direction { get; set; }
        }
    }
}
