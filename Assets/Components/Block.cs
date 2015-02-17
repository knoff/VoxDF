using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
public class Block : CustomComponentBase {
	public int weight;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void SetData(Dictionary<string,string> properties){
		Debug.Log ("Component Block setting data!");
		weight = int.Parse(properties["weight"]);
	}
}
