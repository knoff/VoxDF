using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Utils;
using csDelaunay;
namespace World{
	public class World : Singleton<World> {
		public float size=512;
		public int numPoints = 1;
		public int seed{
			get { return UnityEngine.Random.seed; }
			set { if(UnityEngine.Random.seed!=value) UnityEngine.Random.seed = value; }
		}
		//public Dictionary<Vector2,Biome> biomes = new Dictionary<Vector2, Biome>();
		//public List<string> biomesAv = new List<string>();

		public List<Vector2> points = new List<Vector2>();
		public List<Center> centers = new List<Center>();
		public List<Corner> corners = new List<Corner>();
		public List<Edge> edges = new List<Edge>();

		public delegate bool IslandShape(Vector2 point);
		public delegate List<Vector2> PointSelector(int numPoints);

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
			World.Instance.seed = seed;
			IslandShape f = delegate (Vector2 point) {

				return true;
			};
			PointSelector ps = delegate(int numPoints) {
				int i;
				Vector2 p;
				Voronoi voronoi;
				List<Vector2> region = new List<Vector2> ();
				return region; // temp return
			};
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
		public void Go(int first, int last){
			List<Stage> stages = new List<Stage> ();
			Debug.Log ("Go");
			stages.Add (new Stage ("Place points...", delegate() {
				Reset();
				//pointSelector(numPoints);
			}));
			stages [0].action();
		}
		protected struct Stage{
			public string message;
			public Action action;
			public Stage(string m, Action a){
				message = m;
				action = a;
			}
		}
		/*void OnDrawGizmoz(){
			VoronoiDiagram.Instance.ShowGizmos();
		}*/
		void OnDrawGizmosSelected(){
			//VoronoiDiagram.Instance.ShowGizmos();
		}
	}

	public class IsShape{
		static public float islandFactor = 1.07f;	//1.0 means no small islands; 2.0 leads to a lot

	}
}