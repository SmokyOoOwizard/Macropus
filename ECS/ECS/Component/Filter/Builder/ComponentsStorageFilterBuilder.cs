using System.Linq.Expressions;
using LinqKit;
using Macropus.ECS.Component.Storage;

namespace Macropus.ECS.Component.Filter.Builder;

internal static class ComponentsStorageFilterBuilder
{
	public static Expression<Func<Guid, IReadOnlyComponentsStorage, bool>> Build(ComponentsFilterBuilder filter)
	{
		var componentFilter = BuildInternal(filter);
		return componentFilter;
	}

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, bool>> BuildInternal(ComponentsFilterBuilder filter)
	{
		Expression<Func<Guid, IReadOnlyComponentsStorage, bool>>? expression = null;

		if (filter.FilterComponents.Length > 0)
		{
			var dummies = filter.FilterComponents
				.Select(c => Activator.CreateInstance(c)!);

			expression = FilterBuilderUtils.BuildElements(filter.Type, dummies, BuildHasComponent);
		}

		if (filter.SubFilters.Length > 0)
		{
			var subExpression = FilterBuilderUtils.BuildElements(filter.Type, filter.SubFilters, BuildInternal);
			if (expression != null)
			{
				var oldExpression = expression;
				expression = FilterBuilderUtils.Merge(filter.Type, oldExpression, subExpression);
			}
			else if (filter.Type == EComponentsFilterType.None)
				expression = (id, components) => !subExpression.Invoke(id, components);
			else
				expression = subExpression;
		}

		if (expression == null)
			// TODO
			throw new Exception();

		return expression.Expand();
	}

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, bool>> BuildHasComponent(object dummyComponent)
	{
		return BuildHasComponent((dynamic)dummyComponent);
	}

	// ReSharper disable once UnusedParameter.Local
	private static Expression<Func<Guid, IReadOnlyComponentsStorage, bool>> BuildHasComponent<T>(T _) where T : struct, IComponent
	{
		Expression<Func<Guid, IReadOnlyComponentsStorage, bool>> expression = (id, components) => components.HasComponent<T>(id);

		return expression.Expand();
	}
}