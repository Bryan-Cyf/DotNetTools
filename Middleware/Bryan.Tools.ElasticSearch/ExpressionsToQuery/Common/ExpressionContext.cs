using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nest;

namespace Tools.Elastic
{
    public class ExpressionContext
    {
        public ExpressionContext(MappingIndex mappingIndex)
        {
            Mapping = mappingIndex;
        }

        private MappingIndex Mapping { get; }

        public QueryContainer QueryContainer { get; set; }

        public ExpressionType LastOperator { get; set; }

        public string LastFiled { get; set; }

        public object LastValue { get; set; }

        public QueryBase LastQueryBase { get; set; }

        public void SetQuery()
        {
            HandleField();

            switch (LastQueryBase)
            {
                case TermQuery termQuery:
                    termQuery.Field = LastFiled;
                    termQuery.Value = LastValue;
                    break;
                case BoolQuery boolQuery:
                    boolQuery.MustNot = new List<QueryContainer>
                    {
                        new TermQuery
                        {
                            Field = LastFiled,
                            Value = LastValue
                        }
                    };
                    break;
                case TermRangeQuery termRangeQuery:
                    termRangeQuery.Field = LastFiled;
                    termRangeQuery.GreaterThan = !string.IsNullOrWhiteSpace(termRangeQuery.GreaterThan) ? LastValue.ToString() : null;
                    termRangeQuery.GreaterThanOrEqualTo = !string.IsNullOrWhiteSpace(termRangeQuery.GreaterThanOrEqualTo) ? LastValue.ToString() : null;
                    termRangeQuery.LessThan = !string.IsNullOrWhiteSpace(termRangeQuery.LessThan) ? LastValue.ToString() : null;
                    termRangeQuery.LessThanOrEqualTo = !string.IsNullOrWhiteSpace(termRangeQuery.LessThanOrEqualTo) ? LastValue.ToString() : null;
                    break;
                case MatchPhraseQuery matchPhraseQuery:
                    matchPhraseQuery.Field = LastFiled;
                    matchPhraseQuery.Query = LastValue.ToString();
                    break;
                case QueryStringQuery queryStringQuery:
                    queryStringQuery.Fields = new[] {LastFiled};
                    queryStringQuery.Query = "*" + LastValue + "*";
                    break;
            }

            if (QueryContainer == null)
                QueryContainer = LastQueryBase;
            else
                switch (LastOperator)
                {
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        QueryContainer &= LastQueryBase;
                        break;
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        QueryContainer |= LastQueryBase;
                        break;
                }
        }

        private void HandleField()
        {
            LastFiled = Mapping.Columns.FirstOrDefault(x => x.PropertyName == LastFiled)?.SearchName ?? LastFiled;
        }
    }
}