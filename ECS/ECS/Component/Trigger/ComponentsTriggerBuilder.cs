using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Macropus.ECS.Component.Storage;
using Macropus.ECS.Systems.Exceptions;
using LinqKit;

namespace Macropus.ECS.Component.Trigger;

public readonly struct ComponentsTriggerBuilder
{
	private readonly EComponentsTriggerType type;
	private readonly TriggerDefine[] triggerDefines;
	private readonly ComponentsTriggerBuilder[] subTriggers;

	public ComponentsTriggerBuilder(EComponentsTriggerType type, params TriggerDefine[] triggerDefines)
	{
		if (triggerDefines.Length == 0)
			throw new ArgumentOutOfRangeException();

		CheckTriggersType(triggerDefines);
		this.type = type;
		this.triggerDefines = triggerDefines;
		subTriggers = Array.Empty<ComponentsTriggerBuilder>();
	}

	public ComponentsTriggerBuilder(EComponentsTriggerType type, params ComponentsTriggerBuilder[] subTriggers)
	{
		if (subTriggers.Length == 0)
			throw new ArgumentOutOfRangeException();

		this.type = type;
		triggerDefines = Array.Empty<TriggerDefine>();
		this.subTriggers = subTriggers;
	}

	public ComponentsTrigger Build()
	{
		var expression = BuildInternal();

		var func = expression.Compile();

		return new(func);
	}

	private Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> BuildInternal()
	{
		Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> expression = null;

		if (triggerDefines.Length > 0)
		{
			if(expression == null)
				expression = BuildSelf(triggerDefines[0]).Expand();
			else
			{
				var oldExp = expression;
				var newExp= BuildSelf(triggerDefines[0]).Expand();
				expression = (id, components, changes) => oldExp.Invoke(id, components, changes) && newExp.Invoke(id, components, changes);
			}

			for (var i = 1; i < triggerDefines.Length; i++)
			{
				var oldExp = expression;
				var newExp = BuildSelf(triggerDefines[i]);

				expression = type switch
				{
					EComponentsTriggerType.All => (id, components, changes) => oldExp.Invoke(id, components, changes) && newExp.Invoke(id, components, changes),
					EComponentsTriggerType.Any => (id, components, changes) => oldExp.Invoke(id, components, changes) || newExp.Invoke(id, components, changes),
					EComponentsTriggerType.None => (id, components, changes) => oldExp.Invoke(id, components, changes) && newExp.Invoke(id, components, changes),
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		if (subTriggers.Length > 0)
		{
			if (expression == null)
				expression = subTriggers[0].BuildInternal();
			else
			{
				var oldExp = expression;
				var newExp =  subTriggers[0].BuildInternal();
				expression = (id, components, changes) => oldExp.Invoke(id, components, changes) && newExp.Invoke(id, components, changes);
			}

			for (var i = 1; i < subTriggers.Length; i++)
			{
				var oldExp = expression;
				var newExp = subTriggers[i].BuildInternal();

				expression = type switch
				{
					EComponentsTriggerType.All => (id, components, changes) => oldExp.Invoke(id, components, changes) && newExp.Invoke(id, components, changes),
					EComponentsTriggerType.Any => (id, components, changes) => oldExp.Invoke(id, components, changes) || newExp.Invoke(id, components, changes),
					EComponentsTriggerType.None => (id, components, changes) => oldExp.Invoke(id, components, changes) && newExp.Invoke(id, components, changes),
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		if (type == EComponentsTriggerType.None && expression != null)
		{
			var tmp = expression;
			expression = (id, components, changes) => !tmp.Invoke(id, components, changes);
		}
		return expression;
	}

	private Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> BuildSelf(TriggerDefine trigger)
	{
		var expression = trigger.triggerType switch
		{
			ETriggerType.Added => GetMethod(nameof(Added), trigger.componentType).Invoke(null, null),
			ETriggerType.Removed => GetMethod(nameof(Removed), trigger.componentType).Invoke(null, null),
			ETriggerType.Replaced => GetMethod(nameof(Replaced), trigger.componentType).Invoke(null, null),
			ETriggerType.AddedOrRemoved => GetMethod(nameof(AddedOrRemoved), trigger.componentType).Invoke(null, null),
			ETriggerType.AddedOrReplaced => GetMethod(nameof(AddedOrReplaced), trigger.componentType).Invoke(null, null),
			ETriggerType.RemovedOrReplaced => GetMethod(nameof(RemovedOrReplaced), trigger.componentType).Invoke(null, null),
			ETriggerType.Any => GetMethod(nameof(Any), trigger.componentType).Invoke(null, null),
			_ => throw new ArgumentOutOfRangeException()
		};

		return (Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>>) expression;
	}

	private MethodInfo GetMethod(string name, Type type)
	{
		return GetType().GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type);
	}

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> Added<T>() where T : struct, IComponent
		=> (id, components, changes) => !components.HasComponent<T>(id) && changes.HasComponent<T>(id);

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> Removed<T>() where T : struct, IComponent
		=> (id, components, changes) => components.HasComponent<T>(id) && changes.HadComponent<T>(id);

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> Replaced<T>() where T : struct, IComponent
		=> (id, components, changes) => components.HasComponent<T>(id) && changes.HasComponent<T>(id);

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> AddedOrRemoved<T>() where T : struct, IComponent
		=> (id, components, changes) => !components.HasComponent<T>(id) && changes.HasComponent<T>(id) || components.HasComponent<T>(id) && changes.HadComponent<T>(id);

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> AddedOrReplaced<T>() where T : struct, IComponent
		=> (id, components, changes) => changes.HasComponent<T>(id);

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> RemovedOrReplaced<T>() where T : struct, IComponent
		=> (id, components, changes) => components.HasComponent<T>(id) && (changes.HadComponent<T>(id) || changes.HasComponent<T>(id));

	private static Expression<Func<Guid, IReadOnlyComponentsStorage, IReadOnlyComponentsChangesStorage, bool>> Any<T>() where T : struct, IComponent
		=> (id, components, changes) => (!components.HasComponent<T>(id) && changes.HasComponent<T>(id)) || changes.HadComponent<T>(id) || changes.HasComponent<T>(id);

	private static void CheckTriggersType(TriggerDefine[] triggers)
	{
		List<Type> nonComponents = new();
		foreach (var trigger in triggers)
		{
			var type = trigger.componentType;
			if (!type.IsAssignableTo(typeof(IComponent)))
				nonComponents.Add(type);
		}

		if (nonComponents.Count > 0)
			throw new TypesAreNotComponentsException(nonComponents.ToArray());
	}
}