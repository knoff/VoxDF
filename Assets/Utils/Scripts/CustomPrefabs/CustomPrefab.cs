using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using YamlDotNet.RepresentationModel;

namespace Utils{
	public class CustomPrefab {
		public string name;
		private string data;
		public Type type;
		private List<string> tags = new List<string>();
		private List<string> components = new List<string>();

		private GameObject gameObjectCopy;
		private YamlMappingNode mapping;

		string[] dataLines;
		int dataPointer = 0;
		static private Dictionary<string,Color> Colors = new Dictionary<string, Color> {
			{"black",Color.black},
			{"blue",Color.blue},
			{"clear",Color.clear},
			{"cyan",Color.cyan},
			{"gray",Color.gray},
			{"green",Color.green},
			{"grey",Color.grey},
			{"magenta",Color.magenta},
			{"red",Color.red},
			{"white",Color.white},
			{"yellow",Color.yellow}
		};

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
			//CustomComponentBase c = UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent(go, "Assets/Utils/Scripts/CustomPrefabs/CustomPrefab.cs (32,29)", properties["component"]) as CustomComponentBase;
			CustomComponentBase c = go.AddComponent(Type.GetType(properties["component"])) as CustomComponentBase;
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

		/*private bool Parse(){
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
		}*/
		//public bool Parse(ref GameObject go){
		public bool Parse(){
			bool retVal = true;
			// Get Yaml data
			YamlStream yaml = new YamlStream();
			StringReader input = new StringReader(data);
			yaml.Load(input);
			mapping =
				(YamlMappingNode)yaml.Documents[0].RootNode;
			// Try to get Prefab Type. It can be Exists Class As examle: UnityEngine.GameObject, UnityEngine.Material...
			try{
				YamlScalarNode prefType = (YamlScalarNode) mapping.Children[new YamlScalarNode("type")];
				//type =Type.GetType("UnityEngine.GameObject");
				type = GetType(prefType.Value.ToString());
				var instance = Construct(type, name);
				//Debug.Log(instance);
			}catch(Exception e){
				Debug.Log (e.Message);
				return false;
			}
			/*
			if(typeof(GameObject)==type){
				Debug.Log("GO");
				YamlSequenceNode components = (YamlSequenceNode) mapping.Children[new YamlScalarNode("components")];
				foreach (YamlMappingNode component in components){
					Debug.Log (component);
				}
			}*/
			/*string componentName = mapping.Children[new YamlScalarNode("component")].ToString();

			foreach (var entry in mapping.Children)
			{
				if(((YamlScalarNode)entry.Key).Value!="component"){
					//Debug.Log((YamlScalarNode)entry.Value);
					Debug.Log(((YamlScalarNode)entry.Key).Value);
				}

			}*/
			return retVal;
		}
		public static Type GetType(string typeName){
			var type = Type.GetType(typeName);
			if(type!=null)
				return type;
			var assemblyName = typeName.Substring(0, typeName.IndexOf('.'));
			var assembly = Assembly.Load(assemblyName);
			if(assembly == null)
				return null;

			return assembly.GetType(typeName);
		}
		private dynamic Construct(Type type, string name){
			//instance = Activator.CreateInstance(type);
			switch(type.FullName){
			case "UnityEngine.GameObject":
				Debug.Log("GameObject Init");
				return null;
				break;
			case "UnityEngine.Material":
				Debug.Log("Material Init");
				List<string> keys = new List<string>();
				foreach (var entry in mapping.Children){
					keys.Add(entry.Key.ToString());
				}
				if(!keys.Exists(x=>x=="shader")){
					throw new Exception("No shader for material");
				}
				YamlScalarNode value = (YamlScalarNode) mapping.Children[new YamlScalarNode("shader")];
				Material instance = new Material(Shader.Find(value.Value));
				/*foreach(string key in keys){
					if(key=="type"||key=="shader")
						continue;
					switch(key){
					case "Color":
						value = (YamlScalarNode) mapping.Children[new YamlScalarNode("color")];
						instance.color = getYamlColor(value.Value);
						break;
					case "MainTex":
						break;
					case "MainTex":
						break;
					}
				}*/
				if(keys.Exists(x=>x=="params")){
					List<string> parameters = new List<string>();
					var items = (YamlSequenceNode)mapping.Children[new YamlScalarNode("params")];
					string iType,iName,iValue;
					foreach (YamlMappingNode item in items){
						//parameters.Add(item.Children[new YamlScalarNode("name")].ToString());
						iType= item.Children[new YamlScalarNode("type")].ToString();
						iName= item.Children[new YamlScalarNode("name")].ToString();
						iValue= item.Children[new YamlScalarNode("value")].ToString();
						switch(iType){
						case "Color" :
							instance.SetColor("_"+iName,GetYamlColor(iValue));
							break;
						case "Texture":
							instance.SetTexture("_"+iName,GetYamlTexture(iValue));
							break;
						}
					}
				}
				return instance;
				break;
			case "UnityEngine.Texture":
				Debug.Log("Texture Init");
				return null;
				break;
			}
			return null;
		}
		private Texture GetYamlTexture(string name){
			return new Texture();
		}
		private Color GetYamlColor(string color){
			string[] rgba = color.Split(' ');
			Color c = new Color();
			switch(rgba.Length){
			case 1:
				Colors.TryGetValue(color,out c);
				if(c!=new Color()){
					Debug.Log ("!");
					return c;
				}
				break;
			case 3:
				float.TryParse(rgba[0],out c.r);
				float.TryParse(rgba[1],out c.g);
				float.TryParse(rgba[2],out c.b);
				c.a = 1;
				break;
			case 4:
				float.TryParse(rgba[0],out c.r);
				float.TryParse(rgba[1],out c.g);
				float.TryParse(rgba[2],out c.b);
				float.TryParse(rgba[3],out c.a);
				break;
			default:
				break;
			}
			return c;
		}
	}
}