using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace CustomGraph.Editor
{
	public interface ICustomGraphEditor
	{
		void SetCustomNodeList(CustomNodeDataList customNodeDataList);
	}

	public abstract class CustomGraphEditor<TGraphView> : EditorWindow ,ICustomGraphEditor where TGraphView : CustomGraphView, new() 
	{
		private CustomNodeDataList _customNode;
		private ObjectField _objectField;
		private TGraphView _customGraphView;
	
		/*
		[MenuItem("Window/Open CustomGraphEditor")]
		public static void Open()
		{
			OpenWindow<#Class#,#NodeDataListClass#>();
		}

		[OnOpenAsset]
		public static bool OnOpenAsset(int instanceId, int line)
		{
			return OnOpenAsset<#Class#>(instanceId,line);
			return false;
		}
		*/
		
		protected static bool OnOpenAsset<TEditor,TNodeDataList>(int instanceId, int line)where TEditor : EditorWindow, ICustomGraphEditor
		{
			var obj = Selection.activeObject;
			if (obj.GetType()==typeof(TNodeDataList)) {
				
				OpenWindow<TEditor>((CustomNodeDataList)obj);
				return true;
			}
			return false;
		}

		protected static void OpenWindow<TEditor>(CustomNodeDataList customNodeDataList = null) where TEditor : EditorWindow, ICustomGraphEditor
		{
			var graphEditorWindow = GetWindow<TEditor>(typeof(TEditor).ToString());
			
			if (customNodeDataList != null)
			{
				graphEditorWindow.SetCustomNodeList(customNodeDataList);

				graphEditorWindow.titleContent = new GUIContent(customNodeDataList.GetType().ToString());
			}
			graphEditorWindow.Show();
		}

		public void SetCustomNodeList(CustomNodeDataList customNodeDataList)
		{
			_customNode = customNodeDataList;
			if (_customGraphView != null)
			{
				_customGraphView.LoadData(_customNode);
			}
				
			if (_objectField != null)
			{
				_objectField.value = _customNode;
			}
		}
		
		public virtual void OnEnable()
		{
			_objectField=new ObjectField
			{
				objectType = typeof(CustomNodeDataList)
			};
			_objectField.RegisterCallback<ChangeEvent<Object>>(x =>
			{ 
				SetCustomNodeList((CustomNodeDataList) x.newValue);
			});
			
			rootVisualElement.Add(_objectField);
			
			var graphView = new TGraphView
			{
				style =
				{
					flexGrow = 1
				}
			};
			_customGraphView = graphView;
			rootVisualElement.Add(graphView);
		}
	}
}