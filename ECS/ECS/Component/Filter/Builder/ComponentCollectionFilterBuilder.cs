using System.Linq.Expressions;
using LinqKit;

namespace Macropus.ECS.Component.Filter.Builder;

internal static class ComponentCollectionFilterBuilder
{
	public static Expression<Func<HashSet<string>, bool>> Build(ComponentsFilterBuilder filter)
	{
		var builtFilter = BuildInternal(filter);
		return builtFilter;
	}

	private static Expression<Func<HashSet<string>, bool>> BuildInternal(ComponentsFilterBuilder filter)
	{
		Expression<Func<HashSet<string>, bool>>? expression = null;

		if (filter.FilterComponents.Length > 0)
			expression = FilterBuilderUtils.BuildElements(filter.Type, filter.FilterComponents, BuildHasComponent);

		if (filter.SubFilters.Length > 0)
		{
			var subExpression = FilterBuilderUtils.BuildElements(filter.Type, filter.SubFilters, BuildInternal);
			if (expression != null)
			{
				var oldExpression = expression;
				expression = FilterBuilderUtils.Merge(filter.Type, oldExpression, subExpression);
			}
			else if (filter.Type == EComponentsFilterType.None)
				expression = components => !subExpression.Invoke(components);
			else
				expression = subExpression;
		}

		if (expression == null)
			// TODO
			throw new Exception();

		return expression.Expand();
	}

	private static Expression<Func<HashSet<string>, bool>> BuildHasComponent(Type componentType)
	{
		var componentName = componentType.FullName;

		var constName = Expression.Constant(componentName);
		var tmp = Expression.Lambda<Func<string>>(constName); // const magic

		Expression<Func<HashSet<string>, bool>> expression = components => components.Contains(tmp.Invoke());

		return expression.Expand();
	}
}