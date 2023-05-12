using System;

namespace Macropus.ECS.Component.Filter;

public static class ComponentsFilterBuilderExtensions
{
	public static ComponentsFilterBuilder AllOf(this ComponentsFilterBuilder builder, ComponentsFilterBuilder[] filters)
		=> new(EComponentsFilterType.All, builder, new(EComponentsFilterType.All, filters));

	public static ComponentsFilterBuilder AllOf(this ComponentsFilterBuilder builder, params Type[] components)
		=> new(EComponentsFilterType.All, builder, new(EComponentsFilterType.All, components));

	public static ComponentsFilterBuilder AnyOf(this ComponentsFilterBuilder builder, ComponentsFilterBuilder[] filters)
		=> new(EComponentsFilterType.All, builder, new(EComponentsFilterType.Any, filters));

	public static ComponentsFilterBuilder AnyOf(this ComponentsFilterBuilder builder, params Type[] components)
		=> new(EComponentsFilterType.All, builder, new(EComponentsFilterType.Any, components));

	public static ComponentsFilterBuilder NoneOf(this ComponentsFilterBuilder builder, ComponentsFilterBuilder[] filters)
		=> new(EComponentsFilterType.All, builder, new(EComponentsFilterType.None, filters));

	public static ComponentsFilterBuilder NoneOf(this ComponentsFilterBuilder builder, params Type[] components)
		=> new(EComponentsFilterType.All, builder, new(EComponentsFilterType.None, components));
}