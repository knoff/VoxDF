using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using World;
using Utils;

public class world_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Utils.AssetsManager.Instance.LoadPrefab ("Blocks/Core");
		Utils.AssetsManager.Instance.LoadPrefabFolder("World");
		//Utils.AssetsManager.Instance.LoadPrefabFolder("World/Biomes");
		List<string> names = Utils.AssetsManager.Instance.GetPrefabsNames(@"*/Biomes/*");
		foreach(string name in names){
			Debug.Log(name);
		}
		List<CustomPrefab> biomes = Utils.AssetsManager.Instance.GetByType("World.Biome");
		foreach(CustomPrefab biome in biomes){
			Debug.Log(biome.name);
		}
		World.World.Instance.Map(512);
		World.World.Instance.Go (0,2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
