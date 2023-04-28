using Macropus.CoolStuff.Collections.Pool;
using Macropus.ECS.Serialize.Deserialize.State.Impl;
using Macropus.ECS.Serialize.Serialize.State.Impl;

namespace Macropus.ECS.Serialize;

class StatePool
{
	public static StatePool Instance { get; } = new();
	
	public readonly Pool<ComponentDeserializeState> DeserializeStatePool = Pool<ComponentDeserializeState>.Instance;
	public readonly Pool<RefComponentDeserializeState> RefDeserializeStatePool = Pool<RefComponentDeserializeState>.Instance;
	public readonly Pool<CollectionRefDeserializeState> RefCollectionDeserializeStatePool = Pool<CollectionRefDeserializeState>.Instance;

	public readonly Pool<ComponentSerializeState> ComponentSerializeStatePool = Pool<ComponentSerializeState>.Instance;
	public readonly Pool<ParallelSerializeState> ParallelSerializeStatePool = Pool<ParallelSerializeState>.Instance;

	public void Release(IState state)
	{
		switch (state)
		{
			case CollectionRefDeserializeState crds:
				RefCollectionDeserializeStatePool.Release(crds);
				break;
			case RefComponentDeserializeState rcds:
				RefDeserializeStatePool.Release(rcds);
				break;
			case ComponentDeserializeState cds:
				DeserializeStatePool.Release(cds);
				break;
			
			case ComponentSerializeState css:
				ComponentSerializeStatePool.Release(css);
				break;
			case ParallelSerializeState pss:
				ParallelSerializeStatePool.Release(pss);
				break;
		}
	}

}
