using System.Linq.Expressions;
using LinqKit;

namespace Macropus.ECS.Component.Filter.Builder;

internal static class FilterBuilderUtils
{
	public static Expression<Func<TFunc1, TFunc2, bool>> BuildElements<TElement, TFunc1, TFunc2>(
		EComponentsFilterType mergeType,
		IEnumerable<TElement> elements,
		Func<TElement, Expression<Func<TFunc1, TFunc2, bool>>> elementBuildFunc
	)
	{
		var parts = elements.Select(elementBuildFunc).ToArray();

		Expression<Func<TFunc1, TFunc2, bool>> expression;
		if (mergeType == EComponentsFilterType.None)
			expression = (c1, c2) => !parts[0].Invoke(c1, c2);
		else
			expression = parts[0];

		for (var i = 1; i < parts.Length; i++)
		{
			expression = Merge(mergeType, expression, parts[i]);
			expression = expression.Expand();
		}

		expression = expression.Expand();
		return expression;
	}

	public static Expression<Func<TFunc, bool>> BuildElements<TElement, TFunc>(
		EComponentsFilterType mergeType,
		IEnumerable<TElement> elements,
		Func<TElement, Expression<Func<TFunc, bool>>> elementBuildFunc
	)
	{
		var parts = elements.Select(elementBuildFunc).ToArray();

		Expression<Func<TFunc, bool>> expression;
		if (mergeType == EComponentsFilterType.None)
			expression = c1 => !parts[0].Invoke(c1);
		else
			expression = parts[0];

		for (var i = 1; i < parts.Length; i++)
		{
			var oldExpression = expression;
			expression = Merge(mergeType, oldExpression, parts[i]);
			expression = expression.Expand();
		}

		expression = expression.Expand();
		return expression;
	}

	public static Expression<Func<T, bool>> Merge<T>(
		EComponentsFilterType type,
		Expression<Func<T, bool>> old,
		Expression<Func<T, bool>> newPart
	)
	{
		return type switch
		{
			EComponentsFilterType.All => c1 => old.Invoke(c1) && newPart.Invoke(c1),
			EComponentsFilterType.None => c1 => old.Invoke(c1) && !newPart.Invoke(c1),
			EComponentsFilterType.Any => c1 => old.Invoke(c1) || newPart.Invoke(c1),
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	public static Expression<Func<T1, T2, bool>> Merge<T1, T2>(
		EComponentsFilterType type,
		Expression<Func<T1, T2, bool>> old,
		Expression<Func<T1, T2, bool>> newPart
	)
	{
		return type switch
		{
			EComponentsFilterType.All => (c1, c2) => old.Invoke(c1, c2) && newPart.Invoke(c1, c2),
			EComponentsFilterType.None => (c1, c2) => old.Invoke(c1, c2) && !newPart.Invoke(c1, c2),
			EComponentsFilterType.Any => (c1, c2) => old.Invoke(c1, c2) || newPart.Invoke(c1, c2),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}