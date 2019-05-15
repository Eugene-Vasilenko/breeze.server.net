using Breeze.WebApi2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace Breeze.ContextProvider.EFC
{
    /// <summary>
    /// Applies the BreezeEFCQueryableAttribute to all controller methods except those
    /// that already have a QueryableAttribute or an ODataQueryOptions parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BreezeEFCControllerAttribute : BreezeControllerAttribute
    {
        public BreezeEFCControllerAttribute()
        {
            this._queryableFilter = new BreezeEFCQueryableAttribute();
        }

        protected override IFilterProvider GetQueryableFilterProvider(EnableBreezeQueryAttribute defaultFilter)
        {
            return new BreezeEFCQueryableFilterProvider(defaultFilter);
        }

    }

    internal class BreezeEFCQueryableFilterProvider : IFilterProvider
    {
        public BreezeEFCQueryableFilterProvider(EnableBreezeQueryAttribute filter)
        {
            _filter = filter;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null ||
                (!IsIQueryable(actionDescriptor.ReturnType)) ||
                actionDescriptor.GetCustomAttributes<EnableQueryAttribute>().Any() || // if method already has a QueryableAttribute (or subclass) then skip it.
                actionDescriptor.GetParameters().Any(parameter => typeof(ODataQueryOptions).IsAssignableFrom(parameter.ParameterType))
            )
            {
                return Enumerable.Empty<FilterInfo>();
            }

            return new[] { new FilterInfo(_filter, FilterScope.Global) };
        }

        internal static bool IsIQueryable(Type type)
        {
            if (type == typeof(IQueryable)) return true;
            if (type != null && type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(IQueryable<>);
            }

            return false;
        }

        private readonly EnableBreezeQueryAttribute _filter;
    }

}
