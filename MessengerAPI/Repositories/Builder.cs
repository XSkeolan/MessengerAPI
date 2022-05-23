namespace MessengerAPI.Repositories
{
    public class Builder
    {
        public static ConditionBuilder Condition = new ConditionBuilder(null);
    }

    public class ConditionBuilder
    {
        readonly SqlOperator _operator;

        public ConditionBuilder(SqlOperator operator1)
        {
            _operator = operator1;
        }

        public ConditionBuilder EqualOperation(string field, object value, EqualOperations equalOperations)
        {
            return new ConditionBuilder(new EqualSqlOperator(field, value, equalOperations));
        }

        public ConditionBuilder EqualOperation(string field, string fieldType, object value, string valueType, EqualOperations equalOperations)
        {
            return new ConditionBuilder(new EqualSqlOperator(field, fieldType, value, valueType, equalOperations));
        }

        public ConditionBuilder AndOperation(ConditionBuilder builder1, ConditionBuilder builder2)
        {
            return new ConditionBuilder(new AndSqlOperator(builder1, builder2));
        }

        public ConditionBuilder LikeOperation(string field, string pattern)
        {
            return new ConditionBuilder(new LikeOperator(field, pattern));
        }

        public string BuildQuery()
        {
            return $"{_operator?.Build()}";
        }

        internal void RegisterArgument(QueryArguments query)
        {
            _operator?.RegisterArgument(query);
        }

        public Result Build()
        {
            QueryArguments arguments = new QueryArguments();

            RegisterArgument(arguments);
            return new Result
            {
                Query = BuildQuery(),
                Args = arguments.GetParameters()
            };
        }
    }

    public abstract class SqlOperator
    {
        public abstract string Build();
        internal abstract void RegisterArgument(QueryArguments argument);
    }

    public class EqualSqlOperator : SqlOperator
    {
        private readonly string _field;
        private readonly object _value;
        private readonly string _equalOperation;
        private readonly string _fieldType;
        private readonly string _valueType;
        private string _key;

        public EqualSqlOperator(string field, object value, EqualOperations equalsType)
        {
            _field = field;
            _value = value;
            _fieldType = "";
            _valueType = "";
            _equalOperation = equalsType switch
            {
                EqualOperations.Equal => "=",
                EqualOperations.NotEqual => "<>",
                EqualOperations.MoreEqual => ">=",
                EqualOperations.More => ">",
                EqualOperations.Less => "<",
                EqualOperations.LessEqual => "<=",
                _ => throw new ArgumentOutOfRangeException(equalsType.ToString())
            };
        }

        public EqualSqlOperator(string field, string fieldType, object value, string valueType, EqualOperations equalsType) : this(field,value,equalsType)
        {
            _fieldType = fieldType;
            _valueType = valueType;
        }

        public override string Build()
        {
            string typeArg1 = _fieldType != string.Empty ? "::" + _fieldType : string.Empty;
            string typeArg2 = _valueType != string.Empty ? " ::" + _valueType : string.Empty;
            return $"{_field}{typeArg1}{_equalOperation}{_key}{typeArg2}";
        }

        internal override void RegisterArgument(QueryArguments query)
        {
            _key = query.AddArgument(_value);
        }
    }

    public class AndSqlOperator : SqlOperator
    {
        private readonly ConditionBuilder _builder1;
        private readonly ConditionBuilder _builder2;

        public AndSqlOperator(ConditionBuilder builder1, ConditionBuilder builder2)
        {
            _builder1 = builder1;
            _builder2 = builder2;
        }

        public override string Build()
        {
            return $"{_builder1.BuildQuery()} AND {_builder2.BuildQuery()}";
        }

        internal override void RegisterArgument(QueryArguments argument)
        {
            _builder1.RegisterArgument(argument);
            _builder2.RegisterArgument(argument);
        }
    }

    public class LikeOperator : SqlOperator
    {
        private readonly string _field;
        private readonly string _pattern;
        private string _key;

        public LikeOperator(string field, string pattern)
        {
            _field = field;
            _pattern = pattern;
        }

        public override string Build()
        {
            return $"{_field} LIKE {_key}";
        }

        internal override void RegisterArgument(QueryArguments query)
        {
            _key = query.AddArgument(_pattern);
        }
    }

    public enum EqualOperations
    {
        Equal,
        NotEqual,
        MoreEqual,
        LessEqual,
        More,
        Less
    }

    public class QueryArguments
    {
        private readonly Dictionary<string, object> args;

        public QueryArguments()
        {
            args = new Dictionary<string, object>();
        }

        public string AddArgument(object value)
        {
            string key = "field" + args.Keys.Count;
            args.Add(key, value);
            return "@" + key;
        }

        public Dictionary<string, object> GetParameters() => args;
    }

    public class Result
    {
        public IDictionary<string, object> Args { get; set; }
        public string Query { get; set; }
    }
}