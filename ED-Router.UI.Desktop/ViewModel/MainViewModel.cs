using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Windows.Input;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ED_Router.Events;
using ED_Router.Extensions;
using ED_Router.Services;
using ED_Router.UI.Desktop.Helper;
using Microsoft.Win32;

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
            ImportCSVCommand = new RelayCommand(ImportCSV);
            OpenOnSpansh = new RelayCommand(OpenOnSpanshWebsite);

			Router = EdRouter.Instance;
			Router.PropertyChanged += Router_PropertyChanged;
			From = new ObservableCollection<string>();
			To = new ObservableCollection<string>();
		}

        private void OpenOnSpanshWebsite()
        {
            if (string.IsNullOrEmpty(Router.SpanshUri))
            {
                MessageBox.Show("Unable to open the website");
                return;
            }

            ProcessHelper.ExecuteProcessUnElevated(Router.SpanshUri,"");
        }

        private async void ImportCSV()
        {
            var fileDialog = new OpenFileDialog()
            {
                DefaultExt = ".csv",
                Filter = "Comma separated value (*.csv)|*.csv",
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                
            };

            var result = fileDialog.ShowDialog();

            if ((result ?? false) == false)
            {
                return;
            }

            try
            {
                var flightPlan = await Task.Run(async () =>
                {
                    Task.Yield();
                    return await Router.CsvManager
                        .ImportCsv(fileDialog.FileName)
                        .ConfigureAwait(continueOnCapturedContext: false);
                });

                Router.SetNewFlightPlan(flightPlan);
            }
            catch(Exception e)
            {
                MessageBox.Show("A problem occurred while trying to read the Csv file.", "An error occurred", MessageBoxButton.OK);
            }
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
            else if(e.PropertyName == nameof(Router.FlightPlan))
            {
                RaisePropertyChanged(nameof(ShowRefuel));
            }

            RaisePropertyChanged();
		}

		public ICommand CalculateCommand { get; private set; }
		public ICommand NextWaypointCommand { get; private set; }
		public ICommand PrevWaypointCommand { get; private set; }
		public ICommand CopyToClipboardCommand { get; private set; }

        public ICommand ImportCSVCommand { get; }

		public ICommand OpenOnSpansh { get; private set; }


		public EdRouter Router { get; private set; }
        
        public Visibility ShowRefuel => (Router?.FlightPlan?.RefuelDataAvailable ?? false)
            ? Visibility.Visible
            : Visibility.Collapsed;

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
                var (nextSystem, _) = Router.NextWaypoint();

                Router.VoiceAttackAccessor.SendEvent(Next_Waypoint.Create(nextSystem, Router.CurrentWaypointIndex, Router.RouteTraveledPercent,false, false));
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
                var (prevSystem, _) = Router.PreviousWaypoint();

                Router.VoiceAttackAccessor.SendEvent(Previous_Waypoint.Create(prevSystem, Router.CurrentWaypointIndex, Router.RouteTraveledPercent, false, false));
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
			Clipboard.SetText(Router.CurrentWaypoint.Name);
		}
	}
}