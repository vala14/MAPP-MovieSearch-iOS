using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using DM.MovieApi;
using DM.MovieApi.ApiResponse;
using DM.MovieApi.MovieDb.Movies;
using MovieDownload;
using UIKit;

namespace MovieSearch.iOS
{
	public class TopRatedMovieController : UITableViewController
	{
		private List<Movie> _topMoviesList;
		private Movies _movies;
		UIActivityIndicatorView activitySpinner;
		private bool _reload;

		public void setReload(bool reload)
		{
			this._reload = reload;
		}

		private const int HorizontalMargin = 20;
		private const int StartY = 0;
		private const int StepY = 50;
		private int _yCoord;

		public TopRatedMovieController(Movies movies)
		{
			this._reload = true;
			MovieDbFactory.RegisterSettings(new MovieDbSettings());
			this._movies = movies;
			_topMoviesList = new List<Movie>();
			this.TabBarItem = new UITabBarItem(UITabBarSystemItem.Favorites, 0);
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
		}

		public async override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (this._reload)
			{
				// Reset the list
				_topMoviesList = new List<Movie>();
				this.TableView.Source = new MovieListSource(this._topMoviesList, OnSelectedMovie);
				TableView.ReloadData();

				// Set spinner
				SetSpinner();

				// Get movies
				await GetTopRatedMovies();
				this._reload = false;

				// Stop spinner
				activitySpinner.StopAnimating();

				TableView.ReloadData();
				// Set source
				this.TableView.Source = new MovieListSource(this._topMoviesList, OnSelectedMovie);
			}
		}

		public override void ViewDidLoad()
		{
			this._yCoord = StartY;
			this.Title = "Top rated movies";

			//// Set spinner
			//SetSpinner();

			//// Get movies
			//await GetTopRatedMovies();

			//// Stop spinner
			//activitySpinner.StopAnimating();

			//TableView.ReloadData();
			//// Set source
			//this.TableView.Source = new MovieListSource(this._topMoviesList, OnSelectedMovie);
		}

		private void SetSpinner()
		{
			this.activitySpinner.Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50);
			this.activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
			this.View.AddSubview(activitySpinner);
			activitySpinner.StartAnimating();
		}

		private void OnSelectedMovie(int row)
		{
			this.NavigationController.PushViewController(new MovieInfoController(this._topMoviesList[row]), false);
		}

		private async Task GetTopRatedMovies()
		{
			var movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
			ApiSearchResponse<MovieInfo> responseMovieInfos = await movieApi.GetTopRatedAsync();

			await _movies.GetMovies(responseMovieInfos);
			_topMoviesList = _movies.MoviesList;

			// Set image path
			StorageClient client = new StorageClient();
			ImageDownloader downloader = new ImageDownloader(client);
			foreach (Movie m in _movies.MoviesList)
			{
				if (m.ImagePath != null)
				{
					string localPath = downloader.LocalPathForFilename(m.ImagePath);

					// if localPath does not exist then download image
					if (!File.Exists(localPath))
					{
						await downloader.DownloadImage(m.ImagePath, localPath, new CancellationToken());
					}

					m.setImagePath(localPath);
				}
				else
				{
					m.setImagePath("");
				}
			}
		}
	}
}
