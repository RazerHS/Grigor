using System;
using UnityEditor;
using UnityEngine;

namespace CardboardCore.Pooling
{
	[CreateAssetMenu(fileName = "PoolConfig", menuName = "CardboardCore/Pooling/PoolConfig")]
	public class PoolConfig : ScriptableObject
	{
		[SerializeField] private PoolEntry[] _poolEntries;

		public PoolEntry[] PoolEntries => _poolEntries;

#if UNITY_EDITOR
		private void OnValidate()
		{
			for(int i = 0; i < _poolEntries.Length; i++)
			{
				_poolEntries[i].OnValidate();
			}
		}

		public void Refresh()
		{
			try
			{
				Debug.Log("Start Generating Pool Names...");
				PoolNamesGenerator.Write(this, _poolEntries);
				Debug.Log("Generating Pool Names Successful!");

				AssetDatabase.Refresh(ImportAssetOptions.Default);
			}
			catch(Exception e)
			{
				Debug.LogException(e);
			}
		}
#endif
	}

}
