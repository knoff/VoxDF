using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace Utils{
	public class CustomPrefab {
		public string name;
		private string data;

		private List<string> tags = new List<string>();
		private List<string> components = new List<string>();

		private GameObject gameObjectCopy;

		string[] dataLines;
		int dataPointer = 0;
		Dictionary <string, string> properties = new Dictionary<string, string>();

		public CustomPrefab(string name, string scriptLines){
			this.name = name;
			this.data = scriptLines;
			/*
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
			*/
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

		public enum SupportedUnityComponent { Mesh, Transform, Collider}

		public bool PrepAndVerify(){
			return Parse();
		}

		private bool Parse(){
			gameObjectCopy = new GameObject(name);
			tags.Clear();
			components.Clear ();

			bool retVal = Parse(ref gameObjectCopy);
			if(!retVal){
				Debug.Log("Error parsing!");
				if(Application.isEditor){
					UnityEngine.Object.DestroyImmediate(gameObjectCopy);
				}else{
					UnityEngine.Object.Destroy(gameObjectCopy);
				}
			}
			return retVal;
		}
		public bool Parse(ref GameObject go){
			bool retVal = true;
			//Lexer lex = new Lexer(data);
			YamlStream yaml = new YamlStream();
			StringReader input = new StringReader(data);
			yaml.Load(input);
			var mapping =
				(YamlMappingNode)yaml.Documents[0].RootNode;
			//yaml.Load(new TextReader());
			string componentName = mapping.Children[new YamlScalarNode("component")].ToString();

			foreach (var entry in mapping.Children)
			{
				if(((YamlScalarNode)entry.Key).Value!="component"){
					//Debug.Log((YamlScalarNode)entry.Value);
					Debug.Log(((YamlScalarNode)entry.Key).Value);
				}

			}
			return retVal;
		}

	}
}