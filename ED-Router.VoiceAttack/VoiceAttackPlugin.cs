using ED_Router.UI.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ED_Router.VoiceAttack
{
	public class VoiceAttackPlugin
	{
		public static string VA_DisplayName()
		{
			return "VoiceAttack ED-Router Plugin";  //a name to distinguish my plugin from others
		}

		public static Guid VA_Id()
		{
			return new Guid("{ACC53782-C0B9-40EC-8916-4C57786BF914}");   //note this is a new guid for this plugin
		}

		public static string VA_DisplayInfo()
		{
			return "VoiceAttack ED-Router Plugin\r\n\r\nRoute your neutron highway.\r\n\r\n2018 chriszero";  //this is just extended info that you might want to give to the user.  note that you should format this up properly.
		}

		public static void VA_Init1(dynamic vaProxy)
		{
		}

		public static void VA_Exit1(dynamic vaProxy)
		{
			//no need to do anything on exit with this implementation
		}

		public static void VA_StopCommand()
		{
			//no need to monitor this
		}

		public static void VA_Invoke1(dynamic vaProxy)
		{
			string context = vaProxy.Context;

			switch (context)
			{
				case "next_waypoint":
					var next = EdRouter.Instance.NextWaypoint();
					vaProxy.SetText("next_waypoint", next.System);
					break;
				case "prev_waypoint":
					var prev = EdRouter.Instance.PreviousWaypoint();
					vaProxy.SetText("prev_waypoint", prev.System);
					break;
				case "open_gui":
					InvokeConfiguration();
					break;
				case "calculate_route":
					EdRouter.Instance.CalculateRoute();
					break;
				default:
					break;
			}
		}

		private static void InvokeConfiguration()
		{
			Thread thread = new Thread(() =>
			{
					MainWindow window = new MainWindow();
					window.ShowDialog();

			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
		}

		private static void WaypointToClipboard()
		{
			Clipboard.SetText(EdRouter.Instance.CurrentWaypoint.System);
		}
	}
}
