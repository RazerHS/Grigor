using UnityEditor;
using UnityEngine;

namespace CardboardCore.Pooling.Editor
{
	[CustomEditor(typeof(PoolConfig))]
	public class PoolConfigEditor : UnityEditor.Editor
	{
		private PoolConfig _poolConfig;

		private void OnEnable()
		{
			_poolConfig = (PoolConfig)target;
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			EditorGUILayout.Space();

			if(GUILayout.Button("Save & Generate Names"))
			{
				_poolConfig.Refresh();
			}
		}
	}
}
