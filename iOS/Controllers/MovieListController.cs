using System;
using System.Collections.Generic;
using UIKit;

namespace MovieSearch.iOS
{
	public class MovieListController : UITableViewController
	{
		private List<Movie> _movieList;

		public MovieListController(List<Movie> movieList)
		{
			this._movieList = movieList;
		}

		public override void ViewDidLoad()
		{
			this.View.BackgroundColor = UIColor.White;
			this.Title = "Movie list";
			this.TableView.Source = new MovieListSource(this._movieList, OnSelectedMovie);
		}

		private void OnSelectedMovie(int row) 		{ 			this.NavigationController.PushViewController(new MovieInfoController(this._movieList[row]),false); 		}

		//is disapper - ismovedtoparent - er þá hægt að kalla á eh fall í TopRatedMovieController? 
	}
}
