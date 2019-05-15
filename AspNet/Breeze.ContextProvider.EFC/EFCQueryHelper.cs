using Breeze.WebApi2;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Web.Http.OData.Query;

namespace Breeze.ContextProvider.EFC
{
    public class EFCQueryHelper : QueryHelper
    {
        public EFCQueryHelper(bool enableConstantParameterization, bool ensureStableOrdering, HandleNullPropagationOption handleNullPropagation, int pageSize)
            : base(enableConstantParameterization, ensureStableOrdering, handleNullPropagation, pageSize)
        {
        }

        public EFCQueryHelper(ODataQuerySettings querySettings) : base(querySettings)
        {
        }

        public EFCQueryHelper() : base()
        {
        }

        public override bool ManuallyExpand
        {
            get { return true; }
        }


        public override IQueryable ApplyExpand(IQueryable queryable, ODataQueryOptions queryOptions)
        {
            var expandQueryString = queryOptions.RawValues.Expand;
            if (string.IsNullOrEmpty(expandQueryString)) return queryable;
            expandQueryString.Split(',').Select(s => s.Trim()).ToList().ForEach(expand => { queryable = EntityFrameworkQueryableExtensions.Include((dynamic)queryable, expand.Replace('/', '.')); });
            return queryable;
        }
    }
}