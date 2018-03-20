using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libspanch;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ED_Router
{
	public class EdRouter : INotifyPropertyChanged
	{

		private static readonly EdRouter _instance = new EdRouter();
		private static string settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ed-router\\settings.json");
		public static EdRouter Instance
		{
			get
			{
				return _instance;
			}
		}

		private EdRouter()
		{
			settingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ed-router\\settings.json");
			_api = new SpanchApi();
			_jMon = new JournalMonitor();
			_jMon.NewLocation += _jMon_NewLocation;

			var backgroundTask = Task.Factory.StartNew(() =>
			{
				_jMon.Start();
				//do something with the result
			});

			_range = 20;
			_efficiency = 60;

			LoadSettings();
		}

		private EdRouter(bool justLoadSettings)
		{

		}

		private void _jMon_NewLocation(string obj)
		{
			Start = obj;
		}

		private SpanchApi _api;
		private int _currentWaypoint;
		private JournalMonitor _jMon;

		public Route Route { get; set; }


		private string _start;
		public string Start
		{
			get { return _start; }
			set
			{
				var list = _api.GetSystems(value);
				if (list.Contains(value))
					_start = value;
				else
					_start = string.Empty;

				PropertyChanged(this, new PropertyChangedEventArgs("Start"));
				SaveSettings();
			}
		}

		private string _destination;
		private double _range;
		private int _efficiency;

		public event PropertyChangedEventHandler PropertyChanged;

		public string Destination
		{
			get { return _destination; }
			set
			{
				var list = _api.GetSystems(value);
				if (list.Contains(value))
					_destination = value;
				else
					_destination = string.Empty;

				SaveSettings();
			}
		}

		public double Range
		{
			get => _range;
			set
			{
				_range = value;
				SaveSettings();
			}
		}

		public int Efficiency
		{
			get => _efficiency;
			set
			{
				_efficiency = value;
				SaveSettings();
			}
		}

		public void CalculateRoute()
		{
			if (Start?.Length > 0 && Destination?.Length > 0)
			{
				var route = _api.PlotRoute(Start, Destination, Range, Efficiency);
				if (route.TotalJumps > 0)
				{
					Route = route;
					_currentWaypoint = 0;
					CurrentWaypoint = Route.SystemJumps.ElementAt(0);
				}
			}
			else
				throw new Exception("Start or Destination is empty.");
		}

		public string GetCurrentSystem()
		{
			return "";
		}

		public SystemJump NextWaypoint()
		{
			if (Route.TotalJumps > 0 && _currentWaypoint + 1 < Route.SystemJumps.Count)
			{
				var next = Route.SystemJumps[++_currentWaypoint];
				CurrentWaypoint = next;
			}
			return CurrentWaypoint;
		}

		public SystemJump PreviousWaypoint()
		{
			if (Route.TotalJumps > 0 && _currentWaypoint - 1 >= 0)
			{
				var next = Route.SystemJumps[--_currentWaypoint];
				CurrentWaypoint = next;
			}
			return CurrentWaypoint;
		}

		public SystemJump CurrentWaypoint { get; private set; }

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
	}
}
