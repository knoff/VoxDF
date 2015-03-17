using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Utils;
namespace World{
	public class World : Singleton<World> {
		public float size=512;
		public int numPoints = 1;
		public int seed{
			get { return Random.seed; }
			set { Random.seed = value; }
		}
		//public Dictionary<Vector2,Biome> biomes = new Dictionary<Vector2, Biome>();
		//public List<string> biomesAv = new List<string>();

		public List<Vector2> points = new List<Vector2>();
		public List<Center> centers = new List<Center>();
		public List<Corner> corners = new List<Corner>();
		public List<Edge> edges = new List<Edge>();

		public void Map(float size){
			this.size = size;
			numPoints = 1;
			Reset();
			/*Debug.Log ("!!!");
			Dictionary<Vector2,Biome> biomes = new Dictionary<Vector2, Biome>();
			VoronoiDiagram.Instance.Generate();
			*/	
		}
		public void NewIsland(string pointType, int numPoints_, int seed, int variant){

		}
		void Reset(){
			Center p;
			Corner q;
			Edge edge;

			points.Clear();
			foreach(Edge _edge in edges){
				_edge.d0 = _edge.d1 = null;
				_edge.v0 = _edge.v1 = null;
			}
			edges.Clear();
			foreach(Corner _q in corners){
				_q.adjacent.Clear();
				_q.touches.Clear();
				_q.protrudes.Clear();
				_q.downslope = null;
				_q.watershed = null;
			}
			corners.Clear();
		}
		/*void OnDrawGizmoz(){
			VoronoiDiagram.Instance.ShowGizmos();
		}*/
		void OnDrawGizmosSelected(){
			//VoronoiDiagram.Instance.ShowGizmos();
		}
	}

	public class IslandShape{
		static public float islandFactor = 1.07f;	//1.0 means no small islands; 2.0 leads to a lot

	}
}