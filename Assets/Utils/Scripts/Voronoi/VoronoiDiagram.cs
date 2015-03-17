using UnityEngine;
using System.Collections.Generic;

using csDelaunay;
namespace Utils{
	public class VoronoiDiagram : Singleton<VoronoiDiagram> {

		public int poligonNumber = 500;
		public Vector2 size = new Vector2(512f,512f);
		public int lloydIterations = 20;

		private Dictionary<Vector2f,Site> sites;
		private List<Edge> edges;

		public void Generate(){
			// Create sites (Centers of polygons)
			List<Vector2f> points = CreateRandomPoints();

			// Create the bounds of the voronoi diagram
			Rectf bounds = new Rectf(0,0,size.x, size.y);

			// Create diagram
			Voronoi voronoi = new Voronoi(points, bounds, lloydIterations);

			sites = voronoi.SitesIndexedByLocation;
			edges = voronoi.Edges;

		}

		private List<Vector2f> CreateRandomPoints(){
			List<Vector2f> points = new List<Vector2f>();
			for (int i=0; i<poligonNumber;i++){
				points.Add(new Vector2f(Random.Range(0,size.x), Random.Range(0, size.y)));
			}
			return points;
		}
		public void ShowGizmos(){
			//Debug.Log("Draw");
			Gizmos.color = Color.red;
			foreach(KeyValuePair<Vector2f,Site> kv in sites){
				Gizmos.DrawSphere(new Vector3(kv.Key.x,0,kv.Key.y),1);
			}
			Gizmos.color = Color.green;
			foreach(Edge edge in edges){
				if(edge.ClippedEnds==null) continue;
				Gizmos.DrawLine(new Vector3(edge.ClippedEnds[LR.LEFT].x,0,edge.ClippedEnds[LR.LEFT].y),new Vector3(edge.ClippedEnds[LR.RIGHT].x,0,edge.ClippedEnds[LR.RIGHT].y));
			}
		}
	}
}