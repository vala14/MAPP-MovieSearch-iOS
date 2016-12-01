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
			this._imageView.Frame = new CGRect(this.View.Bounds.Width/ 2 , _yCoord + 20 , this.View.Bounds.Width / 2 - 20 , (this.View.Bounds.Width / 2 - 20)*1.3	);
			this._imageView.Image = UIImage.FromFile(_movieInfo.ImagePath);

			var movieNameAndYear = this.MovieInfoText(  _movieInfo.Name + " (" + _movieInfo.YearReleased + ")", new CGRect(HorizontalMargin, _yCoord + 10 , this.View.Bounds.Width / 2 - 20,30 ), "AppleSDGothicNeo-Bold", 15f);
			var movieInfo = this.MovieInfoText(_movieInfo.RunningTime.ToString() + " min | " + _movieInfo.Genres , new CGRect(HorizontalMargin, _yCoord + 10, this.View.Bounds.Width - 100, 15),"AppleSDGothicNeo-UltraLight", 12f);
			var movieOverview = this.MovieInfoText(_movieInfo.Overview, new CGRect(HorizontalMargin, _yCoord , this.View.Bounds.Width / 2 - 30, 200),"AppleSDGothicNeo-Regular", 12f);

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
