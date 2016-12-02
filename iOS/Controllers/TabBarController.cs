using System;
using UIKit;

namespace MovieSearch.iOS
{
	public class TabBarController : UITabBarController
	{
		public TabBarController()
		{
			Delegate = new TabBarDelegate();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.TabBar.BackgroundColor = UIColor.LightGray;
			this.TabBar.TintColor = UIColor.LightGray;
			this.SelectedIndex = 0;
		}
	}
}
