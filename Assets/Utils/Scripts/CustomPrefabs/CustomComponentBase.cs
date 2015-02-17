using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Utils{
	public abstract class CustomComponentBase : MonoBehaviour {
		public abstract void SetData(Dictionary<string,string> properties);
	}
}