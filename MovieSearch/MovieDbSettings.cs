using System;
using DM.MovieApi;

namespace MovieSearch
{
	public class MovieDbSettings : IMovieDbSettings
	{
		public string ApiUrl
		{
			get
			{
				return "http://api.themoviedb.org/3/";
			}
		}

		public string ApiKey
		{
			get
			{
				return "a6b44528de54b909b5a64a630475c8a4";
			}
		}
	}
}
