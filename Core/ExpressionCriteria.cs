using System.Linq.Expressions;
using System.Reflection;

namespace DynamicLambda.Core;

    public class ExpressionCriteria<T>
    {
        List<ExpressionCriterion> _expressionCriterion = new
            List<ExpressionCriterion>();

        private string _andOr = "And";


        public ExpressionCriteria<T> And()
        {
            _andOr = "And";
            return this;
        }

        public ExpressionCriteria<T> Or()
        {
            _andOr = "Or";
            return this;
        }

        public ExpressionCriteria<T> Add(string propertyName,
            object value, ExpressionType op)
        {
            var newCriterion = new
                ExpressionCriterion(propertyName, value, op, _andOr);
            _expressionCriterion.Add(newCriterion);
            return this;
        }

        Expression GetExpression(ParameterExpression
                parameter, ExpressionCriterion
                ExpressionCriteria)
        {
            Expression expression = parameter;
            foreach (var member in
                ExpressionCriteria.PropertyName.Split('.'))
            {
                expression =
                    Expression.PropertyOrField(expression, member);
            }
            return Expression.MakeBinary(
                ExpressionCriteria.Operator,
                expression,
                Expression.Constant(ExpressionCriteria.Value));
        }


        public Expression<Func<T, bool>>
            GetLambda()
        {
            Expression expression = null;
            var parameterExpression =
                Expression.Parameter(typeof(T),
                    typeof(T).Name.ToLower());
            foreach (var item in
                _expressionCriterion)
            {
                if (expression == null)
                {
                    expression = GetExpression(parameterExpression, item);
                }
                else
                {
                    expression =
                        item.AndOr == "And" ?
                            Expression.And(
                                expression,
                                GetExpression(parameterExpression, item)) :
                            Expression.Or(
                                expression,
                                GetExpression(parameterExpression, item));
                }
            }
            return expression != null ?
                Expression.Lambda<Func<T,
                    bool>>(expression, parameterExpression) :
                null;
        }


        class ExpressionCriterion
        {
            public ExpressionCriterion(
                string propertyName,
                object value,
                ExpressionType op,
                string andOr = "And")
            {
                AndOr = andOr;
                PropertyName = propertyName;
                Value = value;
                Operator = op;
                validateProperty(typeof(T), propertyName);
            }

            PropertyInfo validateProperty(
                Type type, string propertyName)
            {
                string[] parts = propertyName.Split('.');
                var info = (parts.Length > 1)
                    ? validateProperty(
                        type.GetProperty(
                            parts[0]).PropertyType,
                        parts.Skip(1).Aggregate((a, i) =>
                            a + "." + i))
                    : type.GetProperty(propertyName);
                if (info == null)
                    throw new ArgumentException(propertyName,
                        $"Property {propertyName} is not a member of  {type.Name}");
                return info;
            }

            public string PropertyName { get; }
            public object Value { get; }
            public ExpressionType Operator { get; }
            public string AndOr { get; }
        }
    }

