using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace CodeSmith.Data.Linq
{
    public static class QueryResultCache
    {
        private static Func<Expression, bool> CanBeEvaluatedLocally
        {
            get
            {
                return expression =>
                           {
                               // don't evaluate parameters
                               if (expression.NodeType == ExpressionType.Parameter)
                                   return false;

                               // can't evaluate queries
                               if (typeof (IQueryable).IsAssignableFrom(expression.Type))
                                   return false;

                               return true;
                           };
            }
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// The cache entry has a one minute sliding expiration with normal priority.
        /// </summary>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query) where T : class
        {
            return query.FromCache(CacheItemPriority.Normal, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        /// <param name="duration">The amount of time, in seconds, that a cache entry is to remain in the output cache.</param>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, int duration) where T : class
        {
            return query.FromCache(DateTime.UtcNow.AddSeconds(duration), Cache.NoSlidingExpiration, CacheItemPriority.Normal);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, CacheItemPriority priority, TimeSpan slidingExpiration) where T : class
        {
            return query.FromCache(Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1), CacheItemPriority.Normal);
        }

        /// <summary>
        /// Returns the result of the query; if possible from the cache, otherwise
        /// the query is materialized and the result cached before being returned.
        /// </summary>
        public static IEnumerable<T> FromCache<T>(this IQueryable<T> query, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority)
            where T : class
        {
            // locally evaluate as much of the query as possible
            Expression expression = Evaluator.PartialEval(
                query.Expression,
                CanBeEvaluatedLocally);

            // use the string representation of the query for the cache key
            string key = expression.ToString();

            // the key is potentially very long, so use an md5 fingerprint
            // (fine if the query result data isn't critically sensitive)
            key = key.ToMd5Fingerprint();

            // try to get the query result from the cache
            var result = HttpRuntime.Cache.Get(key) as List<T>;

            if (result != null)
                return result;

            // materialize the query
            result = query.ToList();

            //detach for cache
            foreach (T item in result)
            {
                var entity = item as ILinqEntity;
                if (entity != null)
                    entity.Detach();
            }


            HttpRuntime.Cache.Insert(
                key,
                result,
                null, // no cache dependency
                absoluteExpiration,
                slidingExpiration,
                priority,
                null); // no removal notification

            return result;
        }

        /// <summary>
        /// Creates an MD5 fingerprint of the string.
        /// </summary>
        private static string ToMd5Fingerprint(this string s)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s.ToCharArray());
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(bytes);

            // concat the hash bytes into one long string
            return hash.Aggregate(new StringBuilder(32),
                (sb, b) => sb.Append(b.ToString("X2")))
                .ToString();
        }
    }

    /// <summary>
    /// Enables the partial evalutation of queries.
    /// From http://msdn.microsoft.com/en-us/library/bb546158.aspx
    /// </summary>
    internal static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        ///<param name="expression">The root of the expression tree.</param>
        ///<param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
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

        #region Nested type: Nominator

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

        #endregion

        #region Nested type: SubtreeEvaluator

        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
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
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }

        #endregion
    }
}