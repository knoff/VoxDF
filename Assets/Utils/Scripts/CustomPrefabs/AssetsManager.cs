using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System;
using Assimp;
using Assimp.Configs;

namespace Utils{
	public class AssetsManager : Singleton<AssetsManager> {

		string dataFolder = @"Data/";
		private Dictionary<string, CustomPrefab> prefabs = new Dictionary<string, CustomPrefab>();


		// Use this for initialization
		void Start () {
			//LoadPrefabFolder(dataFolder+@"Materials/");
			//LoadPrefabFolder(dataFolder+@"Blocks/");
		}
		
		// Update is called once per frame
		// void Update () {
		// }


		/**
		 * Loads all prefabs from folder;
		 */
		public List<string> LoadPrefabFolder(string dataFolder){
			List<string> prefabs = new List<string>();
			foreach(string dataFile in Directory.GetFiles(this.dataFolder+dataFolder+@"/", "*.yaml", SearchOption.AllDirectories)){
				string prefabName = dataFile.Substring (dataFile.LastIndexOf(this.dataFolder)+this.dataFolder.Length, dataFile.LastIndexOf(".")-(dataFile.LastIndexOf(this.dataFolder)+this.dataFolder.Length));
				prefabName = prefabName.Replace('\\','/');
				if(LoadPrefab(prefabName)){
					prefabs.Add (prefabName);
				}
			}
			return prefabs;
		}


		/**
		 * Loads prefab with provided name
		 * @param <string> prefanName: Name of loaded prefab
		 */
		public bool LoadPrefab(string prefabName){
			return LoadPrefab(prefabName,false);
		}
		/**
		 * Loads prefab with provided name
		 * @param <string> prefanName: Name of loaded prefab
		 * @param <bool> force: if true, reloads previously loaded prefab
		 */
		public bool LoadPrefab(string prefabName, bool force){
			if(!prefabs.ContainsKey(prefabName)||force){
				StreamReader sr = new StreamReader(dataFolder+prefabName+".yaml");
				string fileContent = sr.ReadToEnd();
				sr.Close();
				CustomPrefab prefab = new CustomPrefab(prefabName, fileContent);
				if(prefab.PrepAndVerify()){
					prefabs.Add (prefabName, prefab);
					return true;
					//Debug.Log("Prefab "+prefabName+" loaded");
				}
			}
			return false;
		}
		public CustomPrefab GetPrefab(string prefabName){
			CustomPrefab prefab;
			prefabs.TryGetValue(prefabName, out prefab);
			return prefab;
		}
		public List<string> GetPrefabsNames(string search){
			/*return new List<CustomPrefab>(from p in prefabs
			                                       where Regex.Matches(search, p.Key)
			                                       select p.Value).All();
			*/
			search = search.Replace("*",@"[\w\s\-]+");
			return (from result in prefabs
				where Regex.Match (result.Key,search).Success
				select result.Key).ToList<string>();
		}
		public List<CustomPrefab> GetPrefabs(string search){
			search = search.Replace("*",@"[\w\s\-]+");
			return (from result in prefabs
			        where Regex.Match (result.Key,search).Success
			        select result.Value).ToList<CustomPrefab>();
		}
		public List<CustomPrefab> GetByType(string type){
			return (from result in prefabs
			        where (result.Value.type.ToString()==type)
			        select result.Value).ToList<CustomPrefab>();
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
