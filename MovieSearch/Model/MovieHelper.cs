using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DM.MovieApi;
using DM.MovieApi.ApiResponse;
using DM.MovieApi.MovieDb.Movies;

namespace MovieSearch
{
	public class MovieHelper
	{
		private List<Movie> _movies;

		public MovieHelper()
		{
			MovieDbFactory.RegisterSettings(new MovieDbSettings());
			this._movies = new List<Movie>();
		}

		public async Task GetMovies(ApiSearchResponse<MovieInfo> responseMovieInfos)
		{
			await this.LoadMovies(responseMovieInfos);
		}

		public List<Movie> MoviesList => this._movies;

		public void AddMovie(string name, string yearReleased, string overview, string imagePath, string actors, string genres, int runningTime)
		{
			var movie = new Movie()
			{
				Name = name,
				YearReleased = yearReleased,
				Overview = overview,
				ImagePath = imagePath,
				Actors = actors,
				Genres = genres,
				RunningTime = runningTime
			};

			this._movies.Add(movie);
		}

		private async Task LoadMovies(ApiSearchResponse<MovieInfo> responseMovieInfos)
		{
			this._movies = new List<Movie>();
			var movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;

			int runtime = 0;
			if (responseMovieInfos != null && responseMovieInfos.Results != null && responseMovieInfos.Results.Count != 0)
			{
				foreach (MovieInfo info in responseMovieInfos.Results)
				{
					// Get movie id
					int movieId = info.Id;

					// Get Movie object - to get runtime
					ApiQueryResponse<DM.MovieApi.MovieDb.Movies.Movie> responseMovies = await movieApi.FindByIdAsync(movieId);
					DM.MovieApi.MovieDb.Movies.Movie movie = responseMovies.Item;
					if (movie != null)
					{
						runtime = movie.Runtime;
					}

					// Get MovieCredit object - to get list of actors
					ApiQueryResponse<MovieCredit> responseCredits = await movieApi.GetCreditsAsync(movieId);
					MovieCredit credit = responseCredits.Item;

					string actors = "";
					// Take first 3 actors
					if (credit != null && credit.CastMembers != null && credit.CastMembers.Count != 0)
					{
						int actorsListSize = credit.CastMembers.Count > 3 ? 3 : credit.CastMembers.Count;
						for (int i = 0; i < actorsListSize; i++)
						{
							actors += credit.CastMembers[i].Name;
							if (i != actorsListSize - 1)
							{
								actors += ", ";
							}
						}
					}

					// Put genre to list
					string genres = "";
					for (int i = 0; i < info.Genres.Count; i++)
					{
						genres += info.Genres[i].Name;
						if (i != info.Genres.Count - 1)
						{
							genres += ", ";
						}
					}

					string title = info.Title == null ? "" : info.Title;
					string year = info.ReleaseDate.Year.ToString() == null ? "" : info.ReleaseDate.Year.ToString();
					string overview = info.Overview == null ? "" : info.Overview;
					string posterPath = info.PosterPath;
					this.AddMovie(title, year, overview, posterPath, actors, genres, runtime);
				}
			}
		}
	}
}
