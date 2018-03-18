using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using libspanch;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace ED_Router.UI.Desktop.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}

			CalculateCommand = new RelayCommand(CalculateMethod);
			NextWaypointCommand = new RelayCommand(NextWaypointMethod);
			PrevWaypointCommand = new RelayCommand(PrevWaypointMethod);
			CopyToClipboardCommand = new RelayCommand(WaypointToClipboard);

			Router = EdRouter.Instance;
			MyRoute = new ObservableCollection<SystemJump>();
			From = new ObservableCollection<string>();
			To = new ObservableCollection<string>();
		}

		public ICommand CalculateCommand { get; private set; }
		public ICommand NextWaypointCommand { get; private set; }
		public ICommand PrevWaypointCommand { get; private set; }
		public ICommand CopyToClipboardCommand { get; private set; }


		public EdRouter Router { get; private set; }
		public ObservableCollection<SystemJump> MyRoute { get; set; }
		public ObservableCollection<string> From { get; private set; }
		public ObservableCollection<string> To { get; private set; }

		private string _fromSearch;

		public string FromSearch
		{
			get { return _fromSearch; }
			set
			{
				_fromSearch = value;
				if (_fromSearch.Length > 2)
				{
					var suggestionSystems = Router.GetSystems(value);

					// Add new Systems
					foreach (var sys in suggestionSystems)
					{
						if (From.Contains(sys) == false)
							From.Add(sys);
					}

					// Remove systems which not in suggestion list
					foreach (var sys in From.ToList())
					{
						if (suggestionSystems.Contains(sys) == false)
							From.Remove(sys);
					}
				}
			}
		}

		private string _toSearch;

		public string ToSearch
		{
			get { return _toSearch; }
			set
			{
				_toSearch = value;
				if (_toSearch.Length > 2)
				{
					var suggestionSystems = Router.GetSystems(value);

					// Add new Systems
					foreach (var sys in suggestionSystems)
					{
						if (To.Contains(sys) == false)
							To.Add(sys);
					}

					// Remove systems which not in suggestion list
					foreach (var sys in To.ToList())
					{
						if (suggestionSystems.Contains(sys) == false)
							To.Remove(sys);
					}
				}
			}
		}


		public void CalculateMethod()
		{
			Router.Start = FromSearch;
			Router.Destination = ToSearch;
			try
			{
				Router.CalculateRoute();
				MyRoute.Clear();
				foreach (var jump in Router.Route.SystemJumps)
				{
					MyRoute.Add(jump);
				}
			}
			catch (System.Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public void NextWaypointMethod()
		{
			Router.NextWaypoint();
			WaypointToClipboard();
			RaisePropertyChanged("Router");
		}

		public void PrevWaypointMethod()
		{
			Router.PreviousWaypoint();
			WaypointToClipboard();
			RaisePropertyChanged("Router");
		}

		public void WaypointToClipboard()
		{
			Clipboard.SetText(Router.CurrentWaypoint.System);
		}
	}
}