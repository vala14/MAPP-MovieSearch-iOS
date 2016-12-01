using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;

namespace MovieSearch.iOS
{
	public class MovieInfoController : UIViewController
	{
		private Movie _movieInfo;

		private const int StartY = 70;
		private const int StepY = 30;
		private const int HorizontalMargin = 20;

		private int _yCoord;

		private UIImageView _imageView;


		public MovieInfoController(Movie movieInfo)
		{
			this._movieInfo = movieInfo;
			this._imageView = new UIImageView();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this._yCoord = StartY;

			this.View.BackgroundColor = UIColor.White;
			this.Title = "Movie info";


			var movieNameAndYear = this.MovieInfoText(  _movieInfo.Name + " (" + _movieInfo.YearReleased + ")", new CGRect(HorizontalMargin, _yCoord + 10, this.View.Bounds.Width - 40, 50), "AppleSDGothicNeo-Bold", 20f);
			var movieInfo = this.MovieInfoText(_movieInfo.RunningTime.ToString() + " min | " + _movieInfo.Genres, new CGRect(HorizontalMargin, _yCoord + 25, this.View.Bounds.Width - 40, 15), "AppleSDGothicNeo-UltraLight", 14f);
			var movieOverview = this.MovieInfoText(_movieInfo.Overview, new CGRect(HorizontalMargin + this.View.Bounds.Width / 2 - 45 + 10, 150, this.View.Bounds.Width / 2, 200), "AppleSDGothicNeo-Regular", 14f);
			movieOverview.SizeToFit();

			this._imageView.Frame = new CGRect(HorizontalMargin, _yCoord - 10, this.View.Bounds.Width / 2 - 45, (this.View.Bounds.Width / 2 - 45) * 1.3);
			this._imageView.Image = UIImage.FromFile(_movieInfo.ImagePath);

			this.View.AddSubview(this._imageView);
			this.View.AddSubview(movieNameAndYear);
			this.View.AddSubview(movieInfo);
			this.View.AddSubview(movieOverview);
		}

		private UILabel MovieInfoText(string text, CGRect rect, string font, float fontSize)
		{
			var prompt = new UILabel()
			{
				Font = UIFont.FromName(font, fontSize),
				Frame = rect,
				Text = text,
				Lines = 0,
				LineBreakMode = UILineBreakMode.TailTruncation
			};
			this._yCoord += StepY;

			return prompt;
		}
	}
}
