using System.Linq.Expressions;

namespace Berry.Spider.Core.Helpers;

/// <summary>
/// 表达式目录树方式实现深拷贝（不适用集合，只适用于对象拷贝）
/// </summary>
public class ExpressionDeepCloneHelper<TIn, TOut>
{
    private static Func<TIn, TOut> _FUNC = null;

    static ExpressionDeepCloneHelper()
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
        List<MemberBinding> memberBindingList = new List<MemberBinding>();
        foreach (var item in typeof(TOut).GetProperties())
        {
            MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
            MemberBinding memberBinding = Expression.Bind(item, property);
            memberBindingList.Add(memberBinding);
        }

        foreach (var item in typeof(TOut).GetFields())
        {
            MemberExpression property = Expression.Field(parameterExpression, typeof(TIn).GetField(item.Name));
            MemberBinding memberBinding = Expression.Bind(item, property);
            memberBindingList.Add(memberBinding);
        }

        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
        Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[]
        {
            parameterExpression
        });
        _FUNC = lambda.Compile();
    }

    public static TOut Clone(TIn t)
    {
        return _FUNC(t);
    }
}