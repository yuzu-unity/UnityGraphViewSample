using System.Collections;
using System.Collections.Generic;
using CustomGraph;
using UnityEngine;

namespace CustomGraph.Sample
{

	public class SampleCustomNodeData : CustomNodeData
	{
		public override int InCount { get; } = 1;
		public override int OutCount { get; } = 3;
		
	}
}