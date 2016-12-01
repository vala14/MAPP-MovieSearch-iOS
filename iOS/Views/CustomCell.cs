using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MovieDownload;
using UIKit;

namespace MovieSearch.iOS
{
	public class CustomCell : UITableViewCell
	{
		private UILabel _nameLabel, _actorsLabel;

		private UIImageView _imageView;

		public CustomCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
		{
			this._imageView = new UIImageView();
			this._nameLabel = new UILabel()
			{
				Font = UIFont.FromName("AppleSDGothicNeo-Bold", 20f),
				TextColor = UIColor.FromRGB(45, 45, 45),
			};

			this._actorsLabel = new UILabel()
			{
				Font = UIFont.FromName("AppleSDGothicNeo-UltraLight", 12f),
				TextColor = UIColor.FromRGB(117, 117, 117),
			};		

			this.ContentView.AddSubviews(new UIView[] { this._imageView, this._nameLabel, this._actorsLabel });
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this._imageView.Frame = new CGRect(5, 5, 25, 34);
			this._nameLabel.Frame = new CGRect(25 + 10, 5, this.ContentView.Bounds.Width - 40, 25);
			this._actorsLabel.Frame = new CGRect(25 + 10, 22, this.ContentView.Bounds.Width - 40, 25);
		}

		public void UpdateCell(string name, string year, string imageName, string actors)
		{
			this._imageView.Image = UIImage.FromFile(imageName);
			this._nameLabel.Text = name + " (" + year + ")";
			this._actorsLabel.Text = actors;

			this.Accessory = UITableViewCellAccessory.DisclosureIndicator;
		}
	}
}
