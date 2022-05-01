using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomGraph
{
    public abstract class CustomNodeData : ScriptableObject
    {
	    
	    //GUID
        public string Guid;

        //PortIn Count
        public abstract int InCount { get; }
        
        //PortOut Count
        public abstract int OutCount { get; }
        
#if UNITY_EDITOR

        public Vector2 Rect;
#endif

	    public virtual CustomNodeData Clone() {
		    return Instantiate(this);
	    }
	   
    }
}
