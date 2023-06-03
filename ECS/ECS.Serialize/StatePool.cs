using ECS.Serialize.Deserialize.State.Impl;
using Macropus.CoolStuff.Collections.Pool;

namespace ECS.Serialize;

class StatePool
{
	public static StatePool Instance { get; } = new();
	
	public readonly Pool<ComponentDeserializeState> DeserializeStatePool = Pool<ComponentDeserializeState>.Instance;

	public void Release(IState state)
	{
		switch (state)
		{
			case ComponentDeserializeState cds:
				DeserializeStatePool.Release(cds);
				break;
		}
	}

}
