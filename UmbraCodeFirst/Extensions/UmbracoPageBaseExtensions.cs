using System;
using System.Linq.Expressions;
using UmbraCodeFirst.Exceptions;

namespace UmbraCodeFirst.Extensions
{
    public static class UmbracoPageBaseExtensions
    {
        public static TProperty GetPropertyValue<TUmbracoPageBase, TProperty>(this TUmbracoPageBase page, Expression<Func<TUmbracoPageBase, TProperty>> expression)
            where TUmbracoPageBase : UmbracoPageBase
        {
            var memberExpression = GetMemberExpression(expression);

            return page.GetPropertyValue<TProperty>(Utility.FormatPropertyAlias(memberExpression.Member.Name));
        }

        public static void SetPropertyValue<TUmbracoPageBase, TProperty>(this TUmbracoPageBase page, Expression<Func<TUmbracoPageBase, TProperty>> expression, TProperty value)
    where TUmbracoPageBase : UmbracoPageBase
        {
            var memberExpression = GetMemberExpression(expression);

            page.SetPropertyValue(Utility.FormatPropertyAlias(memberExpression.Member.Name), value);
        }

        private static MemberExpression GetMemberExpression<TPageData, TProperty>(Expression<Func<TPageData, TProperty>> expression)
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
