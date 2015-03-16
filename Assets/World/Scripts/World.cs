using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
namespace World{
	public class World : Singleton<World> {
		public Dictionary<Vector2,Biome> biomes = new Dictionary<Vector2, Biome>();
		public List<string> biomesAv = new List<string>();
		public bool Generate(){
			Dictionary<Vector2,Biome> biomes = new Dictionary<Vector2, Biome>();
			return false;
		}
	}
}