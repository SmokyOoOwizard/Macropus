using Autofac;
using ECS.Tests.Filter.Components;
using ECS.Tests.Filter.Systems;
using Macropus.ECS.Component.Storage.Impl;
using Macropus.ECS.Systems;
using Xunit.Abstractions;

namespace ECS.Tests.Filter;

public class FilterTests : TestsWithSystems
{
	public FilterTests(ITestOutputHelper output) : base(output) { }

	protected override void Configure(ContainerBuilder builder)
	{
		base.Configure(builder);
		builder.RegisterType<FilterComponentUpdateSystem>()
			.AsImplementedInterfaces()
			.AsSelf()
			.As<ASystem>()
			.SingleInstance();
	}

	[Fact]
	public void FilterTest()
	{
		Fill();

		ExecuteSystems();
	}

	private void Fill()
	{
		var buffer = new ComponentsStorage();

		var counter = 0;

		while ((counter & (1 << 10)) == 0)
		{
			var entity = Guid.NewGuid();

			if ((counter & (1 << 0)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent1());

			if ((counter & (1 << 1)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent2());

			if ((counter & (1 << 2)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent3());

			if ((counter & (1 << 3)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent4());

			if ((counter & (1 << 4)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent5());

			if ((counter & (1 << 5)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent6());

			if ((counter & (1 << 6)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent7());

			if ((counter & (1 << 7)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent8());

			if ((counter & (1 << 8)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent9());

			if ((counter & (1 << 9)) != 0)
				buffer.ReplaceComponent(entity, new EmptyTestComponent10());

			counter++;
		}

		Context.ApplyChanges(buffer);
		Context.SaveChanges();
	}
}