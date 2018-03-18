using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libspanch;

namespace ED_Router
{
    public class EdRouter : INotifyPropertyChanged
    {

		private static readonly EdRouter _instance = new EdRouter();
		public static EdRouter Instance
		{
			get
			{
				return _instance;
			}
		}

        private EdRouter()
        {
            _api = new SpanchApi();
			_jMon = new JournalMonitor();
			_jMon.NewLocation += _jMon_NewLocation;

			var backgroundTask = Task.Factory.StartNew(() =>
			{
				_jMon.Start();
				//do something with the result
			});

            Efficiency = 60;
			Range = 20;
        }

		private void _jMon_NewLocation(string obj)
		{
			Start = obj;
		}

		private SpanchApi _api;
        private int _currentWaypoint;
		private JournalMonitor _jMon;

		public Route Route { get; set; }
		//public string CurrentLocation { get; private set; }


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
                    _destination = string.Empty;

				PropertyChanged(this, new PropertyChangedEventArgs("Start"));
            }
        }

        private string _destination;

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
            }
        }

        public int Range { get; set; }
        public int Efficiency { get; set; }

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
            if (Route.TotalJumps > 0 && _currentWaypoint+ 1 < Route.SystemJumps.Count)
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

    }
}
