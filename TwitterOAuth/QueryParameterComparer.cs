using System.Collections.Generic;

namespace TwitterOAuth
{
    public class QueryParameterComparer : IComparer<QueryParameter>
    {
        public int Compare(QueryParameter x, QueryParameter y)
        {
            return x.Name == y.Name ? System.String.CompareOrdinal(x.Value, y.Value) : System.String.CompareOrdinal(x.Name, y.Name);
        }

    }
}