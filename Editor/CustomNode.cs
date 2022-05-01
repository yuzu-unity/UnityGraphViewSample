using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CustomGraph.Editor
{
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class NodeNameAttribute : Attribute
	{
		private readonly string _name;

		public NodeNameAttribute(string name) => this._name = name;

		public string Name => this._name;
	}
	
	// [NodeName("")]
	public abstract class CustomNode : Node
	{
		public Port[] Input;
		public Port[] Output;
		
		private CustomNodeData _customNodeData;
		public CustomNodeData CustomNodeData => _customNodeData;
		

		public void Setup(CustomNodeData customNodeData)
		{
			_customNodeData = customNodeData;

			title = GetType().GetCustomAttributes(typeof (NodeNameAttribute), false) is NodeNameAttribute[] { Length: > 0 } nameAttribute ?
				nameAttribute[0].Name : GetType().ToString();
			
			viewDataKey = _customNodeData.Guid;
			style.left = _customNodeData.Rect.x;
			style.top = _customNodeData.Rect.y;
			
			Input = new Port[_customNodeData.InCount];
			Output = new Port[_customNodeData.OutCount];
			
			for (int i = 0; i < _customNodeData.InCount; i++)
			{
				var port = CreateInputPort(i);
				Input[i] = port;
				inputContainer.Add(port);
			}

			for (int i = 0; i < _customNodeData.OutCount; i++)
			{
				var port = CreateOutputPort(i);
				Output[i] = port;
				outputContainer.Add(port);
			}
			
		}

		protected virtual Port CreateInputPort(int i)
		{
			return Port.Create<Edge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
				typeof(Port));
		}
		
		protected virtual Port CreateOutputPort(int i)
		{
			return Port.Create<Edge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi,
				typeof(Port));
		}


		public override void SetPosition(Rect newPos)
		{
			base.SetPosition(newPos);
			
			Undo.RecordObject(_customNodeData, $"{_customNodeData.GetType()} (Set Position");
			_customNodeData.Rect.x = newPos.xMin;
			_customNodeData.Rect.y = newPos.yMin;
			EditorUtility.SetDirty(_customNodeData);
		}
	}
}