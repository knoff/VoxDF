//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.18444
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace World
{
	public class Corner
	{
		public int index;
		public Vector2 point;		// location
		public bool ocean;			// ocean
		public bool water;			// lake or ocean
		public bool coast;			// touches ocean and land polygons
		public bool border;			// at the edge of map
		public float elevation;		// 0.0 - 1.0
		public float moisture;		// 0.0 - 1.0

		public List<Center> touches = new List<Center>();
		public List<Edge> protrudes = new List<Edge>();
		public List<Corner> adjacent = new List<Corner>();

		public int river;			// 0 if no river, or volume of water in river
		public Corner downslope;	// pointer to adjacent corner most downhill
		public Corner watershed;	// pointer to coastal corner, or null
		public int watershed_size;
		public Corner ()
		{
		}
	}
}
