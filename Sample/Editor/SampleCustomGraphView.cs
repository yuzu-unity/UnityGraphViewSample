using System;
using System.Collections;
using System.Collections.Generic;
using CustomGraph.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomGraph.Sample
{
	public class SampleCustomGraphView : CustomGraphView
	{
		protected override Dictionary<Type, Type> _dataNodeType { get; } = new Dictionary<Type, Type>()
		{
			{ typeof(SampleCustomNodeData), typeof(SampleCustomNode) }
		};

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
			evt.menu.AppendAction($"SampleCustomNode", (a) => CreateNode<SampleCustomNodeData>(nodePosition));
		}
	}
}