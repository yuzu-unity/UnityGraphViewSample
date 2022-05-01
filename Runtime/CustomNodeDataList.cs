using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CustomGraph
{
	//[CreateAssetMenu(fileName = "CustomNodeDataList.asset", menuName = "CustomNodeDataList")]

	public abstract class CustomNodeDataList : ScriptableObject
	{
		[SerializeField]
		private List<CustomNodeData> _nodes = new List<CustomNodeData>();

		public List<CustomNodeData> Nodes => _nodes;

		[SerializeField]
		private List<EdgeData> _edges = new List<EdgeData>();

		public List<EdgeData> Edges => _edges;

		public CustomNodeDataList Clone() {
			CustomNodeDataList customNodeDataList = Instantiate(this);
			customNodeDataList.Nodes.Clear();
			customNodeDataList.Edges.Clear();
			foreach (var node in _nodes)
			{
				customNodeDataList.Nodes.Add(node.Clone());
			}
			
			foreach (var edge in _edges)
			{
				customNodeDataList.Edges.Add(edge.Clone());
			}
			return customNodeDataList;
		}
		
		#if UNITY_EDITOR
		
		public EdgeData CreateEdge(
			string outputGuid,
			int outputNum,
			string inputGuid,
			int inputNum)
		{
			var guid = GUID.Generate().ToString();
			var edgeData = new EdgeData(guid, outputGuid, outputNum, inputGuid, inputNum);
			
			Undo.RecordObject(this, $"{GetType()} (CreateEdge)");
			_edges.Add(edgeData);

			Undo.RegisterCreatedObjectUndo(this, $"{GetType()} (CreateEdge)");

			AssetDatabase.SaveAssets();
			
			return edgeData;
		}

		public void DeleteEdge(string guid)
		{
			var edgeData = _edges.Find(x => x.Guid == guid);
			if (edgeData == null)
			{
				return;
			}

			Undo.RecordObject(this, $"{GetType()} (DeleteEdge)");

			_edges.Remove(edgeData);
			AssetDatabase.SaveAssets();
		}

		public CustomNodeData CreateNode<T>() where T : CustomNodeData
		{
			CustomNodeData customNode = CreateInstance<T>();
			customNode.name = typeof(T).ToString();
			customNode.Guid = GUID.Generate().ToString();

			Undo.RecordObject(this, $"{GetType()} (CreateNode)");
			_nodes.Add(customNode);

			if (!Application.isPlaying) {
				AssetDatabase.AddObjectToAsset(customNode, this);
			}

			Undo.RegisterCreatedObjectUndo(customNode, $"{GetType()} (CreateNode)");

			AssetDatabase.SaveAssets();
			return customNode;
		}

		public void DeleteNode(CustomNodeData customNode) {
			Undo.RecordObject(this, $"{GetType()} (DeleteNode)");
			_nodes.Remove(customNode);

			//AssetDatabase.RemoveObjectFromAsset(node);
			Undo.DestroyObjectImmediate(customNode);
			AssetDatabase.SaveAssets();
		}
		
		
		#endif
	}
	
	[Serializable]
	public sealed class EdgeData
	{
		public string Guid;
		public string OutputGuid;
		public int OutputNum;
		public string InputGuid;
		public int InputNum;

		public EdgeData(
			string guid,
			string outputGuid,
			int outputNum,
			string inputGuid,
			int inputNum)
		{
			Guid = guid;
			OutputGuid = outputGuid;
			OutputNum = outputNum;
			InputGuid = inputGuid;
			InputNum = inputNum;
		}

		public EdgeData Clone()
		{
			return new EdgeData(Guid,OutputGuid, OutputNum, InputGuid, InputNum);
		}
	}
}
