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
			foreach(string dataFile in Directory.GetFiles(dataFolder+"Blocks/", "*.data", SearchOption.AllDirectories)){
				string[] lines = File.ReadAllLines (dataFile);
				string name = dataFile.Substring (dataFile.LastIndexOf("\\")+1, dataFile.LastIndexOf(".")-(dataFile.LastIndexOf("\\")+1));
				prefabs.Add (name, new CustomPrefab(name, lines));
				Debug.Log("Prefab "+name+" loaded");
			}
		}
		
		// Update is called once per frame
		void Update () {
			if(Input.GetKeyDown(KeyCode.Space)){
				prefabs["Blocks/Core"].Instantiate();
			}
		}
	}
}
