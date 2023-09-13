using System;
using UnityEngine;

namespace CardboardCore.Pooling
{
	[Serializable]
	public struct PoolEntry
	{
		[SerializeField] private MonoBehaviour _prefab;

		[Tooltip("The initial amount of this prefab to shove in the pool")]
		[SerializeField] private int _initialAmount;

		[Tooltip("The amount of this prefab to instantiate once the pool has run dry, don't go nuts with the amount!")]
		[SerializeField] private int _poolScaleAmount;

		public MonoBehaviour Prefab => _prefab;
		public int InitialAmount => _initialAmount;
		public int PoolScaleAmount => _poolScaleAmount;


#if UNITY_EDITOR
		internal void OnValidate()
		{
			if(_initialAmount < 1)
			{
				_initialAmount = 1;
			}

			if(_poolScaleAmount < 1)
			{
				_poolScaleAmount = 1;
			}
		}
#endif
	}
}
