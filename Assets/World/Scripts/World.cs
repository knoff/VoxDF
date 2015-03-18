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

		public List<Vector2f> points = new List<Vector2f>();
		public List<Center> centers = new List<Center>();
		public List<Corner> corners = new List<Corner>();
		public List<Edge> edges = new List<Edge>();

		protected int bumps;
		protected float startAngle;
		protected float dipAngle;
		protected float dipWidth;
		public float islandFactor = 1.07f;
		public float LakeThreshold = 0.3f;
		public bool IslandShape(Vector2f point){

			float angle = Mathf.Atan2(point.y,point.x);
			float length = 0.5f * (Mathf.Max(Mathf.Abs(point.x), Mathf.Abs(point.y))+point.magnitude);
			float r1 = 0.5f+0.4f*Mathf.Sin (startAngle+bumps*angle+Mathf.Cos((bumps+3)*angle));
			float r2 = 0.7f+0.2f*Mathf.Sin (startAngle+bumps*angle+Mathf.Cos((bumps+2)*angle));
			if(Mathf.Abs(angle-dipAngle)<dipWidth
			   || Mathf.Abs(angle-dipAngle+2*Mathf.PI)<dipWidth
			   || Mathf.Abs(angle-dipAngle-2*Mathf.PI)<dipWidth){
				r1 = r2 = 0.2f;
			}
			return (length<r1||(length>r1*islandFactor&&length<r2));
			//return true;
		}
		public List<Vector2f> PointSelector(int numPoints) {
			int LloydRelaxations = 2;
			int i;
			//Vector2f p;
			Voronoi voronoi;
			List<Vector2f> region = new List<Vector2f> ();
			List<Vector2f> points = new List<Vector2f>();
			for(i=0; i< numPoints; i++){
				points.Add(new Vector2f(UnityEngine.Random.Range(10,size-10),
				                        UnityEngine.Random.Range(10,size-10))
				           );
			}
			voronoi = new Voronoi(points, new Rectf(0,0,size, size),LloydRelaxations);
			points = voronoi.SiteCoords();
			voronoi.Dispose();
			return points;
		}

		public void Map(float size){
			this.size = size;
			numPoints = Mathf.RoundToInt((size/16)*(size/16));
			NewIsland(numPoints,85882);
			Reset();
			/*Debug.Log ("!!!");
			Dictionary<Vector2,Biome> biomes = new Dictionary<Vector2, Biome>();
			VoronoiDiagram.Instance.Generate();
			*/	
		}
		public void NewIsland(int numPoints_, int seed){
			World.Instance.seed = seed;
			World.Instance.numPoints = numPoints_;
			bumps = (int) UnityEngine.Random.Range(1,6);
			startAngle = UnityEngine.Random.Range(0,2*Mathf.PI);
			dipAngle = UnityEngine.Random.Range(0, 2*Mathf.PI);
			dipWidth = UnityEngine.Random.Range(0.2f,0.7f);
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
			stages.Add (new Stage ("Place points ("+numPoints+")...", delegate() {
				Reset();
				points = World.Instance.PointSelector(numPoints);
			}));
			stages.Add (new Stage("Build graph...",delegate(){
				Voronoi voronoi = new Voronoi(points, new Rectf(0,0,size, size));
				BuildGraph(points, voronoi);
				ImproveCorners();
				voronoi.Dispose();
				voronoi = null;
				points.Clear();
				points = null;
			}));
			stages.Add(new Stage("Assign elevations...",delegate(){
				AssignCornerElevations();

				AssignOceanCoastAndLand();

				RedistributeElevations(LandCorners(corners));
			}));

			for(int i=first; i<=last; i++){
				Debug.Log (stages[i].message);
				stages[i].action();
			}
		}

		public void ImproveCorners(){
			Dictionary<int,Vector2f> newCorners = new Dictionary<int,Vector2f>(/*corners.Count*/);
			foreach(Corner _q in corners){
				if(_q.border){
					newCorners[_q.index] = _q.point;
				}else{
					Vector2f point = new Vector2f(0,0);
					foreach(Center r in _q.touches){
						point.x+=r.point.x;
						point.y+=r.point.y;
					}
					point.x/=_q.touches.Count;
					point.y/=_q.touches.Count;
					newCorners[_q.index] = point;
				}
			}

			for(int i = 0; i<corners.Count; i++){
				corners[i].point = newCorners[i];
			}

			foreach(Edge edge in edges){
				if(edge.v0!=null && edge.v1!=null){
					edge.midpoint = Vector2f.Lerp(edge.v0.point,edge.v1.point,0.5f);
				}
			}
		}

		public void BuildGraph(List<Vector2f> points, Voronoi voronoi){
			Center p;
			//Corner q;
			//Vector2f point;
			Vector2f other;
			List<csDelaunay.Edge> libedges = voronoi.Edges;
			Dictionary<Vector2f,Center> centerLookup = new Dictionary<Vector2f, Center>();

			// Build Center objects for each of the points, and a lookup map
			// to find those Center objects again as we build the graph
			foreach(Vector2f _point in points){
				p = new Center();
				p.index = centers.Count;
				p.point = _point;
				p.neighbors = new List<Center>();
				p.borders = new List<Edge>();
				p.corners = new List<Corner>();
				centers.Add(p);
				centerLookup[_point] = p;
			}
			Dictionary<int,List<Corner>> _cornerMap = new Dictionary<int,List<Corner>>();
			Func<Vector2f,Corner> makeCorner = delegate (Vector2f point){
				if(point == null || point==new Vector2f()) return null;
				List<Corner> value = new List<Corner>();
				for(int _bucket=Convert.ToInt32(point.x)-1; _bucket<=Convert.ToInt32(point.x)+1; _bucket++){
					if(!_cornerMap.TryGetValue(_bucket,out value)) continue;
					foreach(Corner q in _cornerMap[_bucket]){
						float dx = point.x - q.point.x;
						float dy = point.y - q.point.y;
						if(dx*dx+dy*dy<1e-6){
							return q;
						}
					}
				}
				int bucket = Convert.ToInt32(point.x);
				if(!_cornerMap.TryGetValue(bucket,out value)) _cornerMap[bucket]=new List<Corner>();
				Corner _q = new Corner();
				_q.index = corners.Count;
				_q.point = point;
				_q.border = (point.x==0||point.x==size||point.y==0||point.y==size);
				_q.touches = new List<Center>();
				_q.protrudes = new List<Edge>();
				_q.adjacent = new List<Corner>();
				corners.Add (_q);
				_cornerMap[bucket].Add(_q);
				return _q;
			};
			Action<List<Corner>,Corner> AddToCornerList = delegate (List<Corner> v,Corner x) {
				if(x!=null && v.IndexOf(x)<0){
					v.Add(x);
				}
			};
			Action<List<Center>,Center> AddToCenterList = delegate (List<Center> v,Center x) {
				if(x!=null && v.IndexOf(x)<0){
					v.Add(x);
				}
			};

			foreach(csDelaunay.Edge libedge in libedges){
				LineSegment dedge = libedge.DelaunayLine();
				LineSegment vedge = libedge.VoronoiLine();

				Edge edge = new Edge();
				edge.index = edges.Count;
				edge.river = 0;
				edges.Add(edge);
				edge.midpoint = (vedge.p0!=null && vedge.p1!=null) ? Vector2f.Lerp(vedge.p0, vedge.p1, 0.5f):new Vector2f();

				edge.v0 = makeCorner(vedge.p0);
				edge.v1 = makeCorner(vedge.p1);
				edge.d0 = centerLookup[dedge.p0];
				edge.d1 = centerLookup[dedge.p1];

				if(edge.d0!=null){edge.d0.borders.Add(edge);}
				if(edge.d1!=null){edge.d1.borders.Add(edge);}
				if(edge.v0!=null){edge.v0.protrudes.Add(edge);}
				if(edge.v1!=null){edge.v1.protrudes.Add(edge);}

				if(edge.d0!=null && edge.d1!= null){
					AddToCenterList(edge.d0.neighbors, edge.d0);
					AddToCenterList(edge.d1.neighbors, edge.d1);
				}

				if(edge.v0 != null&& edge.v1 != null){
					AddToCornerList(edge.v0.adjacent, edge.v1);
					AddToCornerList(edge.v1.adjacent, edge.v0);
				}

				if(edge.d0 != null){
					AddToCornerList(edge.d0.corners, edge.v0);
					AddToCornerList(edge.d0.corners, edge.v1);
				}

				if(edge.d1 != null){
					AddToCornerList(edge.d1.corners, edge.v0);
					AddToCornerList(edge.d1.corners, edge.v1);
				}

				if(edge.v0!=null){
					AddToCenterList(edge.v0.touches, edge.d0);
					AddToCenterList(edge.v0.touches, edge.d1);
				}
				if(edge.v1!=null){
					AddToCenterList(edge.v1.touches, edge.d0);
					AddToCenterList(edge.v1.touches, edge.d1);
				}
			}
		}
			//foreach(Center _p in centers){
			//	voronoi.Region(p.point);
			//}

		public void AssignCornerElevations(){
			List<Corner> queue = new List<Corner>();
			foreach(Corner _q in corners){
				_q.water = !IslandShape(new Vector2f(2*(_q.point.x/size-0.5f),2*(_q.point.y/size-0.5f)));
				if(_q.border){
					_q.elevation = 0.0f;
					queue.Add(_q);
				}else{
					_q.elevation = 100;
				}
			}
			Corner q;
			while(queue.Count>0){
				q = queue[queue.Count-1];
				queue.RemoveAt(queue.Count-1);
				foreach(Corner s in q.adjacent){
					float newElevation = 0.01f+q.elevation;
					if(!q.water&&!s.water){
						newElevation+=1;
					}
					if(newElevation<s.elevation){
						s.elevation = newElevation;
						queue.Add(s);
					}
				}
			}
		}
		public void AssignOceanCoastAndLand(){
			List<Center> queue = new List<Center>();
			foreach(Center _p in centers){
				int numWater = 0;
				foreach(Corner _q in _p.corners){
					if(_q.border){
						_p.border = true;
						_p.ocean = true;
						_q.water = true;
						queue.Add(_p);
					}
					if(_q.water){
						numWater+=1;
					}
				}
				_p.water = (_p.ocean||numWater>=_p.corners.Count*LakeThreshold);
			}
			while(queue.Count>0){
				Center p = queue[queue.Count-1];
				queue.RemoveAt(queue.Count-1);
				foreach(Center r in p.neighbors){
					if(r.water && !r.ocean){
						r.ocean = true;
						queue.Add(r);
					}
				}
			}
			int numOcean = 0;
			int numLand = 0;
			foreach(Center p in centers){
				numOcean = 0;
				numLand = 0;
				foreach(Center _r in p.neighbors){
					numOcean+=_r.ocean?1:0;
					numLand+=_r.water?0:1;
				}
				p.coast = (numOcean>0)&&(numLand>0);
			}

			foreach(Corner q in corners){
				numOcean = 0;
				numLand = 0;
				foreach(Center pp in q.touches){
					numOcean+= pp.ocean?1:0;
					numLand+= pp.water?0:1;
				}
				q.ocean = (numOcean==q.touches.Count);
				q.coast = (numOcean>0)&&(numLand>0);
				q.water = q.border || ((numLand!=q.touches.Count)&&!q.coast);
			}
		}
		private struct Stage{
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
			foreach(Center kv in centers){
				if(kv.water)
					Gizmos.color = Color.blue;
				else
					Gizmos.color = Color.red;
				Gizmos.DrawSphere(new Vector3(kv.point.x,0,kv.point.y),1);
			}
			/*foreach(Edge edge in edges){
				//if(edge.ClippedEnds==null) continue;
				//Gizmos.color = Color.green;
				//Gizmos.DrawLine(new Vector3(edge.d0.point.x,0,edge.d0.point.y),new Vector3(edge.d1.point.x,0,edge.d1.point.y));
				if(edge.v0!=null&&edge.v1!=null){
				  Gizmos.color = Color.blue;
				  Gizmos.DrawLine(new Vector3(edge.v0.point.x,0,edge.v0.point.y),new Vector3(edge.v1.point.x,0,edge.v1.point.y));
				}
			}*/
		}
	}
}