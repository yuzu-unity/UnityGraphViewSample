using System.Collections;
using System.Collections.Generic;
using CustomGraph.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CustomGraph.Sample
{

	public class SampleCustomGraphEditor : CustomGraphEditor<SampleCustomGraphView>
	{
		[MenuItem("Window/Open SampleCustomGraphEditor")]
		public static void Open()
		{
			OpenWindow<SampleCustomGraphEditor>();
		}

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceId, int line)
		{
			return OnOpenAsset<SampleCustomGraphEditor,SampleCustomNodeDataList>(instanceId, line);
		}
	}
}