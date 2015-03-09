using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using Assimp;
using Assimp.Configs;

namespace Utils{
	public class EnitityLoader : MonoBehaviour {

		string dataFolder = @"Data/";
		private Dictionary<string, CustomPrefab> prefabs = new Dictionary<string, CustomPrefab>();


		// Use this for initialization
		void Start () {
			string fileName = dataFolder+@"Models/sphere.fbx";

			AssimpContext importer = new AssimpContext();
			NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(66.0f);
			importer.SetConfig(config);
			Scene model = importer.ImportFile(fileName, PostProcessPreset.TargetRealTimeMaximumQuality);

			Debug.Log(model.Meshes.Count);
			model.Clear();
			importer.Dispose();

			/*
			LoadPrefabFolder(dataFolder+@"Materials/");
			LoadPrefabFolder(dataFolder+@"Blocks/");
			Debug.Log(GetPrefab("Materials/Core").GetInstance());
			*/
		}
		
		// Update is called once per frame
		void Update () {
			if(Input.GetKeyDown(KeyCode.Space)){
				InstantiatePrefab("Blocks/Core");
			}
		}


		/**
		 * Loads all prefabs from folder;
		 */
		public void LoadPrefabFolder(string dataFolder){
			foreach(string dataFile in Directory.GetFiles(dataFolder, "*.yaml", SearchOption.AllDirectories)){
				string prefabName = dataFile.Substring (dataFile.LastIndexOf(this.dataFolder)+this.dataFolder.Length, dataFile.LastIndexOf(".")-(dataFile.LastIndexOf(this.dataFolder)+this.dataFolder.Length));
				LoadPrefab(prefabName);
			}
		}


		/**
		 * Loads prefab with provided name
		 * @param <string> prefanName: Name of loaded prefab
		 */
		public void LoadPrefab(string prefabName){
			LoadPrefab(prefabName,false);
		}
		/**
		 * Loads prefab with provided name
		 * @param <string> prefanName: Name of loaded prefab
		 * @param <bool> force: if true, reloads previously loaded prefab
		 */
		public void LoadPrefab(string prefabName, bool force){
			if(!prefabs.ContainsKey(prefabName)||force){
				StreamReader sr = new StreamReader(dataFolder+prefabName+".yaml");
				string fileContent = sr.ReadToEnd();
				CustomPrefab prefab = new CustomPrefab(prefabName, fileContent);
				if(prefab.PrepAndVerify()){
					prefabs.Add (prefabName, prefab);
					//Debug.Log("Prefab "+prefabName+" loaded");
				}
			}
		}
		public CustomPrefab GetPrefab(string prefabName){
			CustomPrefab prefab;
			prefabs.TryGetValue(prefabName, out prefab);
			return prefab;
		}

		public GameObject InstantiatePrefab(string prefabName){
			if(prefabs.ContainsKey(prefabName)){
				return prefabs[prefabName].Instantiate();
			}else{
				Debug.Log ("Prefab "+prefabName+" not loaded");
			}
			return null;
		}
	}
}
