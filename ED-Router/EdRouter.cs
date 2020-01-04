using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
			Start = obj;

			if (CurrentLocationDebug)
            {
				VoiceAttackAccessor.LogMessage($"EDRouter: current system {obj}", MessageColor.Yellow);
            }

            if (!EnableAutoWaypoint || !string.Equals(CurrentWaypoint.System, obj))
            {
                return;
            }

            var nextSystem = NextWaypoint();

            VoiceAttackAccessor.SendEvent($"Next Waypoint Set");

            if (nextSystem.DistanceLeft > 0)
            {
                return;
            }

            EnableAutoWaypoint = false;
            VoiceAttackAccessor.SendEvent($"Final Waypoint Set");
        }

		private SpanchApi _api;
		private int _currentWaypoint;
		private JournalMonitor _jMon;
		private string _destination;
		private double _range;
		private int _efficiency;
		private Route _route;
		private SystemJump _currentWaypoint1;

		public event PropertyChangedEventHandler PropertyChanged;

		public Route Route
		{
			get => _route;
			set
			{
				_route = value;
				OnPropertyChanged();
			}
		}

        public bool CurrentLocationDebug { get; set; } = false;

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


		private string _start;
		public string Start
		{
			get { return _start; }
			set
			{
				if (_start == value)
					return;

				var list = _api.GetSystems(value);
				if (list.Contains(value))
					_start = value;
				else
					_start = string.Empty;

				OnPropertyChanged();
				SaveSettings();
			}
		}

		public string Destination
		{
			get { return _destination; }
			set
			{
				if (_destination == value)
					return;

				var list = _api.GetSystems(value);
				if (list.Contains(value))
					_destination = value;
				else
					_destination = string.Empty;

				SaveSettings();
				OnPropertyChanged();
			}
		}

		public double Range
		{
			get => _range;
			set
			{
				if (_range == value)
					return;

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
            if (Start?.Length > 0 && Destination?.Length > 0)
            {
                try
                {
                    IsBusy = true;
                    var route = await Task.Run(() => _api.PlotRouteAsync(Start, Destination, Range, Efficiency));
                    if (route.TotalJumps > 0)
                    {
                        Route = route;
                        _currentWaypoint = 0;
                        CurrentWaypoint = Route.SystemJumps.ElementAt(0);
                    }
				}
                finally
                {
                    IsBusy = false;
					VoiceAttackAccessor.SendEvent("Calculate Route");
                }
            }
            else
            {
                throw new Exception("Start or Destination is empty.");
            }
        }

		public void CalculateRoute()
		{
            if (Start?.Length > 0 && Destination?.Length > 0)
			{
                try
                {
                    IsBusy = true;
                    var route = _api.PlotRoute(Start, Destination, Range, Efficiency);
                    if (route.TotalJumps > 0)
                    {
                        Route = route;
                        _currentWaypoint = 0;
                        CurrentWaypoint = Route.SystemJumps.ElementAt(0);
                    }
                }
                finally
                {
                    IsBusy = false;
                    VoiceAttackAccessor.SendEvent("CalculateRoute");
				}
			}
			else
            {
                throw new Exception("Start or Destination is empty.");
            }
		}

		public SystemJump NextWaypoint()
		{
            if (Route == null)
            {
                return null;
            }
			if (Route.TotalJumps > 0 && _currentWaypoint + 1 < Route.SystemJumps.Count)
			{
				var next = Route.SystemJumps[++_currentWaypoint];
				CurrentWaypoint = next;
			}
			return CurrentWaypoint;
		}

		public SystemJump PreviousWaypoint()
		{
            if (Route == null)
            {
                return null;
            }
			if (Route.TotalJumps > 0 && _currentWaypoint - 1 >= 0)
			{
				var next = Route.SystemJumps[--_currentWaypoint];
				CurrentWaypoint = next;
			}
			return CurrentWaypoint;
		}

		public SystemJump CurrentWaypoint
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

		public void SaveSettings()
		{
			Directory.CreateDirectory(Path.GetDirectoryName(settingsFile));

			using (StreamWriter file = File.CreateText(settingsFile))
			{
				JObject o = JObject.FromObject(new
				{
					settings = new
					{
						destination = Destination,
						start = Start,
						efficiency = Efficiency,
						range = Range
					}
				});

				file.Write(o.ToString());
			}
		}

		public void LoadSettings()
		{
			try
			{
				if (File.Exists(settingsFile))
				{
					dynamic o1 = JObject.Parse(File.ReadAllText(settingsFile));

					_start = o1.settings.start;
					_destination = o1.settings.destination;
					_range = o1.settings.range;
					_efficiency = o1.settings.efficiency;
				}

			}
			catch (Exception) { }

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
			_backgroundTask?.Dispose();
            _jMon.NewLocation -= _jMon_NewLocation;
			_jMon.Stop();
        }
    }
}
