using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Reflection;
using YamlDotNet.RepresentationModel;
using Assimp;
using Assimp.Configs;

namespace Utils{
	public class CustomPrefab {
		public string name;
		private string data;
		public Type type;
		private List<string> tags = new List<string>();
		private List<string> components = new List<string>();

		private GameObject gameObjectCopy;
		private YamlMappingNode mapping;
		public object instance;
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
		static private Dictionary<string,Type> Components = new Dictionary<string,Type>{
			{"MeshFilter",GetType("UnityEngine.MeshFilter")},
			{"Mesh",GetType("UnityEngine.MeshFilter")},
			{"MeshRenderer",GetType("UnityEngine.MeshRenderer")},
			{"Renderer",GetType("UnityEngine.MeshRenderer")},
			{"Collider",GetType("UnityEngine.BoxCollider")},
			{"BoxCollider",GetType("UnityEngine.BoxCollider")},
			{"SphereCollider",GetType("UnityEngine.SphereCollider")}
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
			GameObject go = GameObject.CreatePrimitive(UnityEngine.PrimitiveType.Cube);
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
				UnityEngine.Object instance = (UnityEngine.Object) Construct(type, name);
				if(Application.isEditor){
					//UnityEngine.Object.DestroyImmediate(instance);
				}else{
					//UnityEngine.Object.Destroy(instance);
				}
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
			List<string> keys = new List<string>();
			switch(type.FullName){

			case "UnityEngine.GameObject":
				//Debug.Log("GameObject Init");
				keys.Clear();
				foreach (var entry in mapping.Children){
					keys.Add(entry.Key.ToString());
				}
				GameObject go = new GameObject(name);
				Type comp;
				UnityEngine.Component component;
				if(keys.Exists(x=>x=="components")){
					List<string> components = new List<string>();
					List<string> componentParams = new List<string>();
					var items = (YamlSequenceNode)mapping.Children[new YamlScalarNode("components")];
					foreach (YamlMappingNode item in items){
						comp = null;
						componentParams.Clear();
						foreach(var par in item.Children){
							componentParams.Add(par.Key.ToString());
						}
						if(!componentParams.Exists(x=>x=="component")){
							continue;
						}
						//comp = GetType("UnityEngine."+item.Children[new YamlScalarNode("component")].ToString());
						if(!Components.TryGetValue(item.Children[new YamlScalarNode("component")].ToString(),out comp)){
							continue;
						}
						//if(comp==null){
						//	continue;
						//}
						component = go.AddComponent(comp);
						Type t = component.GetType();
						/*System.Reflection.FieldInfo[] fieldInfo = t.GetFields();
						foreach (System.Reflection.FieldInfo info in fieldInfo)
							Debug.Log("Field:" +info.Name);
						
						System.Reflection.PropertyInfo[] propertyInfo = t.GetProperties();
						foreach (System.Reflection.PropertyInfo info in propertyInfo)
							Debug.Log("Prop:"+info.Name);
	*/
						foreach(string param in componentParams){
							if(param!="component"){
								string value = item.Children[new YamlScalarNode(param)].ToString();
								string tp = "string";
								if(value.IndexOf(' ')>=0){
									tp = value.Substring(0,value.IndexOf(' '));
									value = value.Substring(value.IndexOf(' ')+1);
								}
								System.Reflection.PropertyInfo propertyInfo = t.GetProperty(param);
								if (propertyInfo!=null)
									Debug.Log("Prop:"+propertyInfo.PropertyType);
								//FieldInfo info = component.GetType().GetField("mesh");
								//Debug.Log (component.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Length);
								//Debug.Log (component.GetType().GetProperty(param).GetType().ToString());
								//Debug.Log(tp+"|"+value);
								try{
									//component.GetType().InvokeMember(param,BindingFlags.Instance | BindFlags.Public | BindFlags.SetProperty | Type.DefaultBinder, component, 
								}catch{

								}
							}
						}
					}
				}
				return go;
				break;
			case "UnityEngine.Material":
				//Debug.Log("Material Init");
				keys.Clear();
				foreach (var entry in mapping.Children){
					keys.Add(entry.Key.ToString());
				}
				if(!keys.Exists(x=>x=="shader")){
					throw new Exception("No shader for material");
				}
				YamlScalarNode value = (YamlScalarNode) mapping.Children[new YamlScalarNode("shader")];
				UnityEngine.Material instance = new UnityEngine.Material(Shader.Find(value.Value));
				instance.name = name;
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
		private UnityEngine.Texture GetYamlTexture(string name){
			return new UnityEngine.Texture();
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

		public dynamic GetInstance(){
			switch(type.FullName){
			case "UnityEngine.Material":
				if(instance==null){
					instance=Construct(type,name);
					//AssetDatabase.CreateAsset(material,("Assets/Resources/"+name.ToString()+".mat"));
				}
				return instance;
				break;
			case "UnityEngine.Texture":
				if(instance==null)
					instance=Construct(type,name);
				return instance;
				break;
			default:
				break;
			}
			return null;
		}
	}
}