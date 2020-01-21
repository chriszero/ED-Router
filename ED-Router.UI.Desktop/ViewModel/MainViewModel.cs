using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ED_Router.Events;
using ED_Router.Extensions;
using ED_Router.Services;

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
		}

		private void Router_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
            if (e.PropertyName == nameof(Router.Start) && !(Router.Start ?? string.Empty).Equals(_fromSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                FromSearch = Router.Start;
            }
            else if (e.PropertyName == nameof(Router.Destination) && !(Router.Destination ?? string.Empty).Equals(_toSearch, StringComparison.InvariantCultureIgnoreCase))
            {
                ToSearch = Router.Destination;
            }

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
                if (_fromSearch == value)
                {
                    return;
                }
                _fromSearch = value;
                Router.SetStartAsync(value).FireAndForget();
				RaisePropertyChanged();
                if (_fromSearch.Length > 2)
				{
					var suggestionSystems = Router.GetSystems(value);

                    foreach (var sys in suggestionSystems)
					{
                        if (From.Contains(sys) == false)
                        {
                            From.Add(sys);
						}
					}

					// Remove systems which not in suggestion list
					foreach (var sys in From.ToList())
					{
                        if (suggestionSystems.Contains(sys) == false)
                        {
                            From.Remove(sys);
						}
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
                if (_toSearch == value)
                {
                    return;
                }
                _toSearch = value;
                Router.SetDestinationAsync(value).FireAndForget();
				RaisePropertyChanged();
				if (_toSearch.Length > 2)
				{
					var suggestionSystems = Router.GetSystems(value);

					// Add new Systems
					foreach (var sys in suggestionSystems)
					{
						if (To.Contains(sys) == false)
						{
                            To.Add(sys);
						}
							
					}

					// Remove systems which not in suggestion list
					foreach (var sys in To.ToList())
					{
                        if (suggestionSystems.Contains(sys) == false)
                        {
                            To.Remove(sys);
						}
					}
				}
                
			}
		}
		
		public async void CalculateMethod()
		{
            try
			{
                //Router.Start = FromSearch;
                //Router.Destination = ToSearch;
				await Router.CalculateRouteAsync();
            }
			catch (Exception e)
			{
                if (App.IsFromVA)
                {
                    EdRouter.Instance.VoiceAttackAccessor.LogMessage($"EDRouter Error: {e.Message}", MessageColor.Red);
				}
                else
                {
					MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		public void NextWaypointMethod()
		{
            try
            {
                var nextSystem = Router.NextWaypoint();

                Router.VoiceAttackAccessor.SendEvent(Next_Waypoint.Create(nextSystem, false, false));
				WaypointToClipboard();
                RaisePropertyChanged(() => Router);
            }
            catch (Exception e)
            {
                if (App.IsFromVA)
                {
					EdRouter.Instance.VoiceAttackAccessor.LogMessage($"EDRouter Error: {e.Message}", MessageColor.Red);
                }
                else
                {
                    MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
			}
		}

		public void PrevWaypointMethod()
		{
            try
            {
                var prevSystem = Router.PreviousWaypoint();

                Router.VoiceAttackAccessor.SendEvent(Previous_Waypoint.Create(prevSystem, false, false));
				WaypointToClipboard();
                RaisePropertyChanged(() => Router);
            }
            catch (Exception e)
            {
				if (App.IsFromVA)
                {
                    EdRouter.Instance.VoiceAttackAccessor.LogMessage($"EDRouter Error: {e.Message}", MessageColor.Red);
				}
                else
                {
                    MessageBox.Show(e.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
			}
		}

		public void WaypointToClipboard()
		{
            if (Router.CurrentWaypoint == null)
            {
                return;
            }
			Clipboard.SetText(Router.CurrentWaypoint.System);
		}
	}
}