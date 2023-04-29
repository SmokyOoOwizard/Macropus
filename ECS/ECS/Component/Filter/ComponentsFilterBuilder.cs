using System.Linq.Expressions;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Systems.Exceptions;

namespace Macropus.ECS.Component.Filter;

public readonly struct ComponentsFilterBuilder
{
	private readonly EComponentsFilterType type;

	private readonly ComponentsFilterBuilder[] subFilters;
	private readonly Type[] filterComponents;

	public ComponentsFilterBuilder(
		EComponentsFilterType type,
		params Type[] filterComponents
	)
	{
		if (filterComponents.Length == 0)
			throw new ArgumentOutOfRangeException();

		CheckComponents(filterComponents);

		this.type = type;
		subFilters = Array.Empty<ComponentsFilterBuilder>();
		this.filterComponents = filterComponents;
	}

	public ComponentsFilterBuilder(
		EComponentsFilterType type,
		params ComponentsFilterBuilder[] subFilters
	)
	{
		if (subFilters.Length == 0)
			throw new ArgumentOutOfRangeException();

		this.type = type;
		this.subFilters = subFilters;
		filterComponents = Array.Empty<Type>();
	}

	public ComponentsFilter Build()
	{
		var entityIdParameter = Expression.Parameter(typeof(Guid), "entityId");
		var componentsParameter = Expression.Parameter(typeof(IReadOnlyComponentsStorage), "components");

		var componentFilter = BuildInternal(entityIdParameter, componentsParameter);


		var lambda = Expression.Lambda<Func<Guid, IReadOnlyComponentsStorage, bool>>(componentFilter,
			entityIdParameter, componentsParameter);

		return new(lambda.Compile());
	}

	private Expression BuildInternal(
		ParameterExpression entityIdParameter,
		ParameterExpression componentsParameter
	)
	{
		Expression? expression = null;

		Func<Expression, Expression, BinaryExpression> mergeFunc;
		switch (type)
		{
			case EComponentsFilterType.All:
			case EComponentsFilterType.None:
				mergeFunc = Expression.AndAlso;
				break;
			case EComponentsFilterType.Any:
				mergeFunc = Expression.OrElse;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if (filterComponents.Length > 0)
		{
			expression = BuildSelfFilter(mergeFunc, entityIdParameter, componentsParameter);
		}

		if (subFilters.Length > 0)
		{
			var subExpression = BuildSubFilters(mergeFunc, entityIdParameter, componentsParameter);

			expression = expression == null ? subExpression : mergeFunc(expression, subExpression);
		}

		if (expression == null)
			// TODO
			throw new Exception();

		return expression;
	}

	private Expression BuildSubFilters(
		Func<Expression, Expression, BinaryExpression> mergeFunc,
		ParameterExpression entityIdParameter,
		ParameterExpression componentsParameter
	)
	{
		var subExpression = subFilters[0].BuildInternal(entityIdParameter, componentsParameter);
		if (type == EComponentsFilterType.None)
			subExpression = Expression.Not(subExpression);

		for (int i = 1; i < subFilters.Length; i++)
		{
			if (type == EComponentsFilterType.None)
				subExpression = mergeFunc(subExpression,
					Expression.Not(subFilters[i].BuildInternal(entityIdParameter, componentsParameter)));
			else
				subExpression = mergeFunc(subExpression,
					subFilters[i].BuildInternal(entityIdParameter, componentsParameter));
		}

		return subExpression;
	}

	private Expression BuildSelfFilter(
		Func<Expression, Expression, BinaryExpression> mergeFunc,
		ParameterExpression entityIdParameter,
		ParameterExpression componentsParameter
	)
	{
		var expression = BuildHasComponent(filterComponents[0], entityIdParameter, componentsParameter);

		for (var i = 1; i < filterComponents.Length; i++)
		{
			expression = mergeFunc(expression,
				BuildHasComponent(filterComponents[i], entityIdParameter, componentsParameter));
		}

		return expression;
	}

	private Expression BuildHasComponent(
		Type componentType,
		ParameterExpression entityIdParameter,
		ParameterExpression componentsParameter
	)
	{
		Expression expression = Expression.Call(componentsParameter, nameof(IReadOnlyComponentsStorage.HasComponent),
			new[] { componentType }, entityIdParameter);

		if (type == EComponentsFilterType.None)
			expression = Expression.Not(expression);

		return expression;
	}

	private static void CheckComponents(Type[] components)
	{
		List<Type> nonComponents = new();
		foreach (var type in components)
		{
			if (!type.IsAssignableTo(typeof(IComponent)))
				nonComponents.Add(type);
		}

		if (nonComponents.Count > 0)
			throw new TypesAreNotComponentsException(nonComponents.ToArray());
	}
}