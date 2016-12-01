using System;
using UIKit;
using CoreGraphics;
using DM.MovieApi;
using DM.MovieApi.MovieDb.Movies;
using DM.MovieApi.ApiResponse;
using System.Collections.Generic;
using MovieDownload;
using System.Threading;
using System.IO;

namespace MovieSearch.iOS
{
	public class MovieController : UIViewController
	{
		private Movies _movies;
		UIActivityIndicatorView activitySpinner;

		public MovieController(Movies movies)
		{
			// Initialize spinner
			MovieDbFactory.RegisterSettings(new MovieDbSettings());
			this._movies = movies;
			this.TabBarItem = new UITabBarItem(UITabBarSystemItem.Search, 0);
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
		}

		private const int HorizontalMargin = 20;

		private const int StartY = 80;

		private const int StepY = 50;

		private int _yCoord;

		public override void ViewWillAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			activitySpinner.StopAnimating();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.Title = "Movie search";
			this.View.BackgroundColor = UIColor.White;

			this._yCoord = StartY;

			var prompt = this.CreatePrompt();

			var movieField = this.CreateMovieField();

			var getMoviesButton = this.CreateButton("Get movies");

			getMoviesButton.TouchUpInside += async (sender, args) =>
			{
				// API call to get searched movies 
				var movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
				ApiSearchResponse<MovieInfo> responseMovieInfos = await movieApi.SearchByTitleAsync(movieField.Text == null ? "" : movieField.Text);

				movieField.ResignFirstResponder();
				getMoviesButton.Enabled = false;

				activitySpinner.Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50);
				activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
				this.View.AddSubview(activitySpinner);
				activitySpinner.StartAnimating();

				await _movies.GetMovies(responseMovieInfos);

				// set imgpath
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

				this.NavigationController.PushViewController(new MovieListController(_movies.MoviesList), true);

				getMoviesButton.Enabled = true;
				//activitySpinner.StopAnimating();
				movieField.Text = null;
			};

			this.View.AddSubview(prompt);
			this.View.AddSubview(movieField);
			this.View.AddSubview(getMoviesButton);
		}

		private UIButton CreateButton(string title)
		{ 
			var button = UIButton.FromType(UIButtonType.RoundedRect);
			button.Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50);
			button.SetTitle(title, UIControlState.Normal);
			this._yCoord += StepY;
			return button;
		}

		private UILabel CreatePrompt()
		{
			var prompt = new UILabel()
			{
				Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width, 50),
				Text = "Enter words in movie title: "
			};
			this._yCoord += StepY;

			return prompt;
		}

		private UITextField CreateMovieField()
		{
			var movieField = new UITextField()
			{
				Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50),
				BorderStyle = UITextBorderStyle.RoundedRect,
				Placeholder = "Movie title"
			};
			this._yCoord += StepY;

			return movieField;
		}
	}
}
