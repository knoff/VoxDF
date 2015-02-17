using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Utils{
	public class CustomPrefab {
		string name;
		string[] dataLines;
		int dataPointer = 0;
		Dictionary <string, string> properties = new Dictionary<string, string>();

		public CustomPrefab(string name, string[] scriptLines){
			this.name = name;
			this.dataLines = scriptLines;
			while(dataPointer < dataLines.Length){
				if(dataLines[dataPointer].Length < 1||dataLines[dataPointer].Trim()[0].Equals(";")){
					dataPointer++;
					continue;
				}
				string key = dataLines[dataPointer].Substring(0,dataLines[dataPointer].IndexOf("=")-1).Trim().ToLower();
				string value = dataLines[dataPointer].Substring(dataLines[dataPointer].IndexOf("=") + 1).Trim();
				this.properties[key] = value;
				Debug.Log(key+" = "+value);
				dataPointer++;
			}
			dataPointer = 0;
		}

		public GameObject Instantiate(){
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.name = name;
			CustomComponentBase c = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(go, "Assets/Utils/Scripts/CustomPrefabs/CustomPrefab.cs (32,29)", properties["component"]) as CustomComponentBase;
			if(c!=null){
				c.SetData(properties);
			}else{
				Debug.Log("Error adding "+properties["component"]+"! Ensure the name is typed correctly");
			}
			return go;
		}
	}
}