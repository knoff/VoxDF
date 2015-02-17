using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Utils{
	public class EnitityLoader : MonoBehaviour {

		string dataFolder = @"Data\";
		public Dictionary<string, CustomPrefab> prefabs = new Dictionary<string, CustomPrefab>();


		// Use this for initialization
		void Start () {
			LoadPrefabFolder(dataFolder+@"Blocks/");
		}
		
		// Update is called once per frame
		void Update () {
			if(Input.GetKeyDown(KeyCode.Space)){
				InstantiatePrefab("Blocks/Core");
			}
		}

		public void LoadPrefabFolder(string dataFolder){
			foreach(string dataFile in Directory.GetFiles(dataFolder, "*.yaml", SearchOption.AllDirectories)){
				StreamReader sr = new StreamReader(dataFile);
				string fileContent = sr.ReadToEnd();
				string name = dataFile.Substring (dataFile.LastIndexOf("\\")+1, dataFile.LastIndexOf(".")-(dataFile.LastIndexOf("\\")+1));
				CustomPrefab prefab = new CustomPrefab(name, fileContent);
				if(prefab.PrepAndVerify()){
					prefabs.Add (name, prefab);
					Debug.Log("Prefab "+name+" loaded");
				}
			}
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
