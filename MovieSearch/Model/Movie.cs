using System;
using System.Collections.Generic;

namespace MovieSearch
{
	public class Movie
	{
		public string Name { get; set; }

		public string YearReleased { get; set; }

		public string Overview { get; set; }

		public string ImagePath { get; set; }

		public string Actors { get; set; }

		public string Genres { get; set; }

		public int RunningTime { get; set; }

		public void setImagePath(string path)
		{
			this.ImagePath = path;
		}
	}
}
