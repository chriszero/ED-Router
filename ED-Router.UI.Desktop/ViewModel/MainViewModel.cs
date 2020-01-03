using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;

namespace ED_Router.UI.Desktop.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            CalculateCommand = new RelayCommand(CalculateMethod);
			NextWaypointCommand = new RelayCommand(NextWaypointMethod);
			PrevWaypointCommand = new RelayCommand(PrevWaypointMethod);
			CopyToClipboardCommand = new RelayCommand(WaypointToClipboard);

			Router = EdRouter.Instance;
			Router.PropertyChanged += Router_PropertyChanged;
			From = new ObservableCollection<string>();
			To = new ObservableCollection<string>();
			_fromSearch = Router.Start;
			_toSearch = Router.Destination;
		}

		private void Router_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			RaisePropertyChanged();
		}

		public ICommand CalculateCommand { get; private set; }
		public ICommand NextWaypointCommand { get; private set; }
		public ICommand PrevWaypointCommand { get; private set; }
		public ICommand CopyToClipboardCommand { get; private set; }


		public EdRouter Router { get; private set; }
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

					// pass the Name to the underlying router
					if (suggestionSystems.Contains(value))
					{
						Router.Destination = value;
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

					// pass the Name to the underlying router
					if (suggestionSystems.Contains(value))
					{
						Router.Destination = value;
					}
				}
			}
		}
		
		public async void CalculateMethod()
		{
            try
			{
                Router.Start = FromSearch;
                Router.Destination = ToSearch;
				await Router.CalculateRouteAsync();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public void NextWaypointMethod()
		{
            try
            {
                Router.NextWaypoint();
                WaypointToClipboard();
                RaisePropertyChanged(() => Router);
            }
            catch (Exception e)
            {
				MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public void PrevWaypointMethod()
		{
            try
            {
                Router.PreviousWaypoint();
                WaypointToClipboard();
                RaisePropertyChanged(() => Router);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public void WaypointToClipboard()
		{
			Clipboard.SetText(Router.CurrentWaypoint.System);
		}
	}
}