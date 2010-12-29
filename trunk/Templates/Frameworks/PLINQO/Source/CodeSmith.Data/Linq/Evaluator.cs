using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CodeSmith.Data.Linq
{
    /// <summary>
    /// Enables the partial evalutation of queries.
    /// From http://msdn.microsoft.com/en-us/library/bb546158.aspx
    /// </summary>
    internal static class Evaluator
    {
        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        ///<param name="expression">The root of the expression tree.</param>
        ///<param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation and replacement of independent sub-trees
        /// </summary>
        ///<param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private HashSet<Expression> candidates;
            private bool cannotBeEvaluated;
            private Func<Expression, bool> fnCanBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                candidates = new HashSet<Expression>();
                Visit(expression);
                return candidates;
            }

            protected override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = cannotBeEvaluated;
                    cannotBeEvaluated = false;
                    base.Visit(expression);
                    if (!cannotBeEvaluated)
                    {
                        if (fnCanBeEvaluated(expression))
                        {
                            candidates.Add(expression);
                        }
                        else
                        {
                            cannotBeEvaluated = true;
                        }
                    }
                    cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }

        /// <summary>
        /// Evaluates and replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            private HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return Visit(exp);
            }

            protected override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (candidates.Contains(exp))
                {
                    return Evaluate(exp);
                }
                return base.Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant)
                {
                    return e;
                }
                LambdaExpression lambda = Expression.Lambda(e);
                Delegate fn = lambda.Compile();
                object value = fn.DynamicInvoke(null);

                if (e.Type.IsValueType || e.Type == typeof(string))
                    return Expression.Constant(value, e.Type);

                if (e.Type.IsArray)
                    return EvaluateArray(e, value);

                var enumerable = e.Type.GetInterface("System.Collections.Generic.IEnumerable`1");
                if (enumerable != null)
                    return EvaluateEnumerable(e, value, enumerable);

                return Expression.Constant(value, e.Type);
            }

            private Expression EvaluateArray(Expression e, object value)
            {
                var itemType = e.Type.GetElementType();
                var list = value as IEnumerable;
                if (list == null)
                    return Expression.Constant(value, e.Type);

                var initializers = list.Cast<object>()
                    .Select(o => Expression.Constant(o, itemType))
                    .Cast<Expression>();

                return Expression.NewArrayInit(itemType, initializers);
            }

            private Expression EvaluateEnumerable(Expression e, object value, Type enumerable)
            {
                var itemType = enumerable
                    .GetGenericArguments()
                    .FirstOrDefault();

                if (itemType == null)
                    return Expression.Constant(value, e.Type);

                var list = value as IEnumerable;
                if (list == null)
                    return Expression.Constant(value, e.Type);

                var newExpression = Expression.New(e.Type);
                var initializers = list.Cast<object>()
                    .Select(o => Expression.Constant(o, itemType))
                    .Cast<Expression>();

                return Expression.ListInit(newExpression, initializers);
            }
        }
    }
}