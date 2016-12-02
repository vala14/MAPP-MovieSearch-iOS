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
		private MovieHelper _movieHelper;
		UIActivityIndicatorView activitySpinner;

		public MovieController(MovieHelper movieHelper)
		{
			// Initialize spinner
			MovieDbFactory.RegisterSettings(new MovieDbSettings());
			this._movieHelper = movieHelper;
			this.TabBarItem = new UITabBarItem(UITabBarSystemItem.Search, 0);
			activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
		}

		private const int HorizontalMargin = 20;
		private const int StartY = 160;
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
			this.View.BackgroundColor = UIColor.FromRGB(245,244,244);

			this._yCoord = StartY;
			var prompt = this.CreatePrompt();
			var movieField = this.CreateMovieField();
			var getMoviesButton = this.CreateButton("Get movies");

			getMoviesButton.TouchUpInside += async (sender, args) =>
			{
				movieField.ResignFirstResponder();
				getMoviesButton.Enabled = false;

				activitySpinner.Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50);
				activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
				this.View.AddSubview(activitySpinner);
				activitySpinner.StartAnimating();

				// API call to get searched movies 
				var movieApi = MovieDbFactory.Create<IApiMovieRequest>().Value;
				ApiSearchResponse<MovieInfo> responseMovieInfos = await movieApi.SearchByTitleAsync(movieField.Text == null ? "" : movieField.Text);

				await _movieHelper.GetMovies(responseMovieInfos);

				// set imgpath
				StorageClient client = new StorageClient();
				ImageDownloader downloader = new ImageDownloader(client);
				foreach (Movie m in _movieHelper.MoviesList)
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

				this.NavigationController.PushViewController(new MovieListController(_movieHelper.MoviesList), true);

				getMoviesButton.Enabled = true;
				movieField.Text = null;
			};

			this.View.AddSubview(prompt);
			this.View.AddSubview(movieField);
			this.View.AddSubview(getMoviesButton);
		}

		private UIButton CreateButton(string title)
		{
			this._yCoord += 20;
			var button = UIButton.FromType(UIButtonType.RoundedRect);
			button.Layer.CornerRadius = 25;
			button.Layer.BorderWidth = 0.1f;
			button.Layer.BorderColor = UIColor.DarkGray.CGColor;
			button.Layer.BackgroundColor = UIColor.LightGray.CGColor;

			button.Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50);
			button.SetTitle(title, UIControlState.Normal);
			button.SetTitleColor(UIColor.White, UIControlState.Normal);
			this._yCoord += StepY;
			return button;
		}

		private UILabel CreatePrompt()
		{
			var prompt = new UILabel()
			{
				Font = UIFont.FromName("AppleSDGothicNeo-UltraLight", 18f),
				Frame = new CGRect(0, this._yCoord, this.View.Bounds.Width, 50),
				Text = "Enter words in movie title: ",
				TextAlignment = UITextAlignment.Center
			};
			this._yCoord += StepY;
			return prompt;
		}

		private UITextField CreateMovieField()
		{
			var movieField = new UITextField()
			{
				Frame = new CGRect(HorizontalMargin, this._yCoord, this.View.Bounds.Width - 2 * HorizontalMargin, 50),
				BorderStyle = UITextBorderStyle.None,
				Placeholder = "Movie title",
				TextAlignment = UITextAlignment.Center,
			};
			this._yCoord += StepY;

			movieField.Layer.CornerRadius = 25;
			movieField.Layer.BorderWidth = 0.1f;
			movieField.Layer.BorderColor = UIColor.DarkGray.CGColor;

			return movieField;
		}
	}
}
