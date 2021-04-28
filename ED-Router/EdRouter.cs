using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ED_Router.Events;
using ED_Router.Services;
using libspanch;
using Newtonsoft.Json.Linq;

namespace ED_Router
{
	public class EdRouter : INotifyPropertyChanged, IDisposable
    {
        private bool _isBusy = false;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
			}
        }

        public IDispatcherAccessor Dispatcher { get; protected set; }
		public IVoiceAttackAccessor VoiceAttackAccessor { get; protected set; }
        public static EdRouter Instance { get; } = new EdRouter();

		private static bool _initialization;
        public static void Init(IDispatcherAccessor dispatcherAccessor, IVoiceAttackAccessor voiceAttackAccessor)
        {
            if (_initialization) return;
            _initialization = true;
            Instance.Dispatcher = dispatcherAccessor;
            Instance.VoiceAttackAccessor = voiceAttackAccessor;
        }

		private static string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ed-router\\settings.json");
		private Task _backgroundTask;
        private EdRouter()
		{
			settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ed-router\\settings.json");
			_api = new SpanchApi();
			_jMon = new JournalMonitor();
			_jMon.NewLocation += _jMon_NewLocation;

            _backgroundTask = Task.Factory.StartNew(_jMon.Start);

			_range = 20;
			_efficiency = 60;

			LoadSettings();
		}

		private void _jMon_NewLocation(string obj)
		{
            CurrentSystem = obj;

            if (CurrentWaypoint == null || NeutronPlotterRoute == null || NeutronPlotterRoute.SystemJumps.Count == 0 || !EnableAutoWaypoint || !string.Equals(CurrentWaypoint?.System, obj, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (string.Equals(NeutronPlotterRoute?.DestinationSystem, obj))
            {
				EnableAutoWaypoint = false;
                VoiceAttackAccessor.SendEvent(Final_Waypoint.Create());
                return;
            }

            var (nextSystem, current) = NextWaypoint();

            VoiceAttackAccessor.SendEvent(Next_Waypoint.Create(nextSystem, _currentWaypoint,RouteTraveledPercent,true, true));
        }

		private SpanchApi _api;
		private int _currentWaypoint;
		private JournalMonitor _jMon;
		private string _destination;
		private double _range;
		private int _efficiency;
		private NeutronPlotterRoute _neutronPlotterRoute;
		private NeutronPlotterSystem _currentWaypoint1;

        public int CurrentWaypointIndex => _currentWaypoint;

        public double RouteTraveledPercent => Math.Round(((_currentWaypoint * 1d) / NeutronPlotterRoute.SystemJumps.Count)*100, 2);

        public event PropertyChangedEventHandler PropertyChanged;

		public NeutronPlotterRoute NeutronPlotterRoute
		{
			get => _neutronPlotterRoute;
			set
			{
				_neutronPlotterRoute = value;
				OnPropertyChanged();
			}
		}

        private bool _enableAutoWaypoint;
        public bool EnableAutoWaypoint
        {
            get { return _enableAutoWaypoint; }
            set
            {
                if (_enableAutoWaypoint == value) return;

                _enableAutoWaypoint = value;

                OnPropertyChanged();
            }
        }

        private string _currentSystem;
        public string CurrentSystem
        {
            get => _currentSystem;
            set
            {
                if (string.Equals(_currentSystem, value))
                {
                    return;
                }

                _currentSystem = value;
                OnPropertyChanged();

                IsCurrentStarKnown = !string.IsNullOrEmpty(ValidateWithSpanch(_currentSystem));
                SaveSettings();
            }
        }

        private bool _isCurrentStarKnown = true;
        public bool IsCurrentStarKnown
        {
            get => _isCurrentStarKnown;
            set
            {
                if (_isCurrentStarKnown == value)
                {
                    return;
                }
                _isCurrentStarKnown = value;
                OnPropertyChanged();
            }
        }

        private bool _isDestinationStarKnown = true;
        public bool IsDestinationStarKnown
        {
            get => _isDestinationStarKnown;
            set
            {
                if (_isDestinationStarKnown == value)
                {
                    return;
                }
                _isDestinationStarKnown = value;
                OnPropertyChanged();
            }
        }

        private bool _isStartStarKnown = true;
        public bool IsStartStarKnown
        {
            get => _isStartStarKnown;
            set
            {
                if (_isStartStarKnown == value)
                {
                    return;
                }
                _isStartStarKnown = value;
                OnPropertyChanged();
            }
        }

        public Task SetStartAsync(string system)
        {
            return Task.Run(async () =>
            {
                if (_start?.Equals(system, StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return;
                }

                _start = ValidateWithSpanch(system);
                IsStartStarKnown = !string.IsNullOrEmpty(_start);
                await SaveSettingsAsync();
            });
        }

        public Task SetDestinationAsync(string system)
        {
            return Task.Run(async () =>
            {
                if (_destination?.Equals(system, StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return;
                }

                _destination = ValidateWithSpanch(system);
                IsDestinationStarKnown = !string.IsNullOrEmpty(_destination);
                await SaveSettingsAsync();
            });
        }

        private string _start;
		public string Start
		{
			get => _start;
            set
			{
                if (_start == value)
                {
                    return;
				}

                _start = ValidateWithSpanch(value);

                IsStartStarKnown = !string.IsNullOrEmpty(_start);
				
				OnPropertyChanged();
				SaveSettings();
			}
		}

        private string _spanshUri;
        public string SpanshUri
        {
            get => _spanshUri;
            set
            {
                if (_spanshUri == value)
                {
                    return;
                }

                _spanshUri = value;
                OnPropertyChanged();
            }
        }

        private string ValidateWithSpanch(string system)
        {
            if (string.IsNullOrEmpty(system))
            {
                return system;
            }
            var list = _api.GetSystems(system);

            return list.Find(spanchSystem => string.Equals(spanchSystem, system, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
        }

		public string Destination
		{
			get { return _destination; }
			set
			{
				if (_destination == value)
                {
                    return;
                }

                _destination = ValidateWithSpanch(value);

                IsDestinationStarKnown = !string.IsNullOrEmpty(_destination);

                SaveSettings();
				OnPropertyChanged();
			}
		}

		public double Range
		{
			get => _range;
			set
			{
                if (Math.Abs(_range - value) < double.Epsilon)
                {
                    return;
                }

				_range = value;
				SaveSettings();
				OnPropertyChanged();
			}
		}

		public int Efficiency
		{
			get => _efficiency;
			set
			{
				if (_efficiency == value)
					return;

				_efficiency = value;
				SaveSettings();
				OnPropertyChanged();
			}
		}

        public async Task CalculateRouteAsync()
        {
            if (Start?.Length == 0)
            {
                Start = CurrentSystem;
            }

            if (Start?.Length > 0 && Destination?.Length > 0)
            {
                try
                {
                    IsBusy = true;
                    var route = await Task.Run(() => _api.PlotRouteAsync(Start, Destination, Range, Efficiency));
                    HandleRouteResponse(route);
                }
                finally
                {
                    IsBusy = false;
                }
            }
            else
            {
                throw new Exception("Start or Destination is empty.");
            }
        }

		public void CalculateRoute()
		{
            if (Start?.Length == 0)
            {
                Start = CurrentSystem;
            }

            if (Start?.Length > 0 && Destination?.Length > 0)
			{
                try
                {
                    IsBusy = true;
                    var route = _api.PlotRoute(Start, Destination, Range, Efficiency);
                    HandleRouteResponse(route);
                }
                finally
                {
                    IsBusy = false;
				}
			}
			else
            {
                throw new Exception("Start or Destination is empty.");
            }
		}

        private void HandleRouteResponse(NeutronPlotterRoute neutronPlotterRoute)
        {
            if (neutronPlotterRoute.TotalJumps <= 0)
            {
                return;
            }

            NeutronPlotterRoute = neutronPlotterRoute;
            _currentWaypoint = 0;
            CurrentWaypoint = NeutronPlotterRoute.SystemJumps.ElementAt(0);
            SpanshUri = neutronPlotterRoute.Uri;
            VoiceAttackAccessor.SendEvent(Calculate_Route.Create(neutronPlotterRoute));
        }

        public (NeutronPlotterSystem next, int? id) NextWaypoint()
		{
            if (NeutronPlotterRoute == null)
            {
                return (null, null);
            }
			if (NeutronPlotterRoute.TotalJumps > 0 && _currentWaypoint + 1 < NeutronPlotterRoute.SystemJumps.Count)
			{
				var next = NeutronPlotterRoute.SystemJumps[++_currentWaypoint];
				CurrentWaypoint = next;
			}
			return (CurrentWaypoint, _currentWaypoint);
		}

		public (NeutronPlotterSystem previous, int? id) PreviousWaypoint()
		{
            if (NeutronPlotterRoute == null)
            {
                return (null, null);
            }
			if (NeutronPlotterRoute.TotalJumps > 0 && _currentWaypoint - 1 >= 0)
			{
				var next = NeutronPlotterRoute.SystemJumps[--_currentWaypoint];
				CurrentWaypoint = next;
			}
			return (CurrentWaypoint, _currentWaypoint);
		}

		public NeutronPlotterSystem CurrentWaypoint
		{
			get => _currentWaypoint1;
			private set
			{
				if (_currentWaypoint1 == value)
					return;

				_currentWaypoint1 = value;
				OnPropertyChanged();
			}
		}

		public List<string> GetSystems(string value)
		{
			return _api.GetSystems(value);
		}

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);
        private bool _isLoading;
        public async Task SaveSettingsAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));

                using (var file = File.CreateText(settingsFile))
                {
                    var o = JObject.FromObject(new
                    {
                        settings = new
                        {
                            destination = Destination,
                            start = Start,
                            efficiency = Efficiency,
                            range = Range,
                            current = CurrentSystem,
                        }
                    });

                    file.Write(o.ToString());
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

		public async void SaveSettings()
		{
            if (_isLoading)
            {
                return;
            }
            try
            {
                await Task.Run(SaveSettingsAsync);
            }
            catch (Exception)
            {
            }
		}

		public async void LoadSettings()
		{
            try
            {
                _isLoading = true;
                await Task.Run(() =>
                {
                    if (File.Exists(settingsFile))
                    {
                        dynamic o1 = JObject.Parse(File.ReadAllText(settingsFile));

                        Start = o1.settings.start;
                        Destination = o1.settings.destination;
                        Range = o1.settings.range;
                        Efficiency = o1.settings.efficiency;
                        CurrentSystem = o1.settings.current;
                    }
                });
            }
            catch 
            {
                //TODO: log the exception somewhere.
            }
            finally
            {
                _isLoading = false;
            }

		}

		protected void OnPropertyChanged([CallerMemberName]string name = "")
        {
            if (string.IsNullOrEmpty(name)) return;
            Dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
		}

        public void Dispose()
        {
            (Dispatcher as IDisposable)?.Dispose();
            (VoiceAttackAccessor as IDisposable)?.Dispose();
            _jMon.NewLocation -= _jMon_NewLocation;
            _jMon.Stop();

			_backgroundTask = null;
        }
    }
}
