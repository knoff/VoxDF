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
	public class Center
	{
		public int index;
		public Vector2 point;		// location
		public bool water;			// lake or ocean
		public bool ocean;			// ocean
		public bool coast;			// land polygon touching an ocean
		public bool border;			// at the edge of map
		public string biome;		// biome type
		public float elevation;		// 0.0 - 1.0
		public float moisture;		// 0.0 - 1.0

		public List<Center> neighbors = new List<Center>();
		public List<Edge> borders = new List<Edge>();
		public List<Corner> corners = new List<Corner>();

		public Center ()
		{

		}
	}
}

