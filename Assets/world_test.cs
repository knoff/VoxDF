using UnityEngine;
using System.Collections;
using World;
using Utils;

public class world_test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Utils.AssetsManager.Instance.LoadPrefab ("Blocks/Core");
		Utils.AssetsManager.Instance.LoadPrefab ("World/World");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
