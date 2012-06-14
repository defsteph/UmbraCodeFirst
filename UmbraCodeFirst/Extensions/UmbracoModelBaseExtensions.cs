using System;
using System.Linq.Expressions;
using UmbraCodeFirst.Exceptions;

namespace UmbraCodeFirst.Extensions
{
    public static class UmbracoModelBaseExtensions
    {
        public static TProperty GetPropertyValue<TUmbracoModelBase, TProperty>(this TUmbracoModelBase page, Expression<Func<TUmbracoModelBase, TProperty>> expression)
            where TUmbracoModelBase : UmbracoModelBase
        {
            var memberExpression = GetMemberExpression(expression);

            return page.GetPropertyValue<TProperty>(Utility.FormatPropertyAlias(memberExpression.Member.Name));
        }

        public static void SetPropertyValue<TUmbracoModelBase, TProperty>(this TUmbracoModelBase page, Expression<Func<TUmbracoModelBase, TProperty>> expression, TProperty value)
    where TUmbracoModelBase : UmbracoModelBase
        {
            var memberExpression = GetMemberExpression(expression);

            page.SetPropertyValue(Utility.FormatPropertyAlias(memberExpression.Member.Name), value);
        }

        private static MemberExpression GetMemberExpression<TUmbracoModelBase, TProperty>(Expression<Func<TUmbracoModelBase, TProperty>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body is MemberExpression)
            {
                memberExpression = (MemberExpression)expression.Body;
            }
            else if (expression.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)expression.Body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            if (memberExpression == null)
                throw new UmbraCodeFirstException("The body of the expression must be either a MemberExpression of a UnaryExpression.");
            return memberExpression;
        }
    }
}
