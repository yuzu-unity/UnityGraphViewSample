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
	public abstract class CustomGraphView : GraphView 
	{
		private CustomNodeDataList _customNodeDataList;

		//edgeをguid管理に
		private Dictionary<Edge,string> _edgeGuid = new Dictionary<Edge,string>();

		/// <summary>
		/// 対応ノードデータ、ノード {typeof(CustomNodeData),typeof(CustomNode)}
		/// </summary>
		protected abstract Dictionary<Type, Type> _dataNodeType { get; } 

		public CustomGraphView()
		{
			this.AddManipulator(new ContentZoomer());
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());
			
			Undo.undoRedoPerformed += OnUndoRedo;
		}
		
		private void OnUndoRedo() {
			LoadData(_customNodeDataList);
			AssetDatabase.SaveAssets();
		}
		
		public virtual void LoadData(CustomNodeDataList customNodeDataList)
		{
			_customNodeDataList = customNodeDataList;
			
			graphViewChanged -= OnGraphViewChanged;
			DeleteElements(graphElements.ToList());
			graphViewChanged += OnGraphViewChanged;
			if (_customNodeDataList == null)
			{
				return;
			}
			_customNodeDataList.Nodes.ForEach(CreateCustomNode);
			_customNodeDataList.Edges.ForEach(CreateEdge);
			
		}

		protected virtual GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
		{
			if (graphViewChange.elementsToRemove != null) {
				graphViewChange.elementsToRemove.ForEach(elem => {
					if (elem is CustomNode customNode) {
						_customNodeDataList.DeleteNode(customNode.CustomNodeData);
					}

					if (elem is Edge edge)
					{
						var guid = _edgeGuid[edge];
						_customNodeDataList.DeleteEdge(guid);
					}
				});
			}

			if (graphViewChange.edgesToCreate != null)
			{
				graphViewChange.edgesToCreate.ForEach(edge =>
				{
					var inputNode = (CustomNode)edge.input.node;
					var inputGuid = inputNode.CustomNodeData.Guid;
					var inputNum = Array.IndexOf(inputNode.Input, edge.input);

					var outputNode = (CustomNode)edge.output.node;
					var outputGuid = outputNode.CustomNodeData.Guid;
					var outputNum = Array.IndexOf(outputNode.Output, edge.output);

					var edgeData = _customNodeDataList.CreateEdge(outputGuid, outputNum, inputGuid, inputNum);
					_edgeGuid.Add(edge, edgeData.Guid);
				});
			}
			
			return graphViewChange;
		}
		
	
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatiblePorts = new List<Port>();
			foreach (var port in ports.ToList())
			{
				if (startAnchor.node == port.node || startAnchor.direction == port.direction || startAnchor.portType != port.portType)
				{
					continue;
				}

				compatiblePorts.Add(port);
			}
			return compatiblePorts;
		}

		protected void CreateNode<TData>(Vector2 position) where TData : CustomNodeData 
		{
			if (_customNodeDataList == null)
			{
				return;
			}
			CustomNodeData customNodeData = _customNodeDataList.CreateNode<TData>();
			customNodeData.Rect = position;
			CreateCustomNode(customNodeData);
		}

		private void CreateCustomNode(CustomNodeData customNodeData)
		{
			if (_dataNodeType.TryGetValue(customNodeData.GetType(), out Type nodeType))
			{
				if (Activator.CreateInstance(nodeType) is CustomNode customNode)
				{
					customNode?.Setup(customNodeData);
					AddElement(customNode);
				}
			}
		}

		private void CreateEdge(EdgeData edgeData)
		{
			var inputNode= nodes
				.Select(x => (CustomNode)x)
				.FirstOrDefault(x => x.CustomNodeData.Guid == edgeData.InputGuid);

			var input= inputNode?.Input[edgeData.InputNum];
			
			var outputNode= nodes
				.Select(x => (CustomNode)x)
				.FirstOrDefault(x => x.CustomNodeData.Guid == edgeData.OutputGuid);

			var output= outputNode?.Output[edgeData.OutputNum];

			if (input == null || output == null)
			{
				Debug.LogError($"Not Found Port");
				return;
			}
			
			var edge = new Edge
			{
				output = output,
				input = input
			};
			edge.input.Connect(edge);
			edge.output.Connect(edge);
			
			_edgeGuid.Add(edge,edgeData.Guid);
			
			AddElement(edge); 
		}
	}
}