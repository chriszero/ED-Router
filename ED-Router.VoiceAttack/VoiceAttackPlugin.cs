using ED_Router.UI.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ED_Router.VoiceAttack
{
	public class VoiceAttackPlugin
	{
		private static MainWindow window = null;

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
			if (window != null)
			{
				try
				{
					window.Dispatcher.BeginInvoke((Action)window.Close);
                    window.Dispatcher.InvokeShutdown();
                }
				catch (Exception) { }
			}
		}

		public static void VA_StopCommand()
		{
			//no need to monitor this
		}

		public static void VA_Invoke1(dynamic vaProxy)
		{
			try
			{
				string context = vaProxy.Context;

				switch (context)
				{
					case "next_waypoint":
						var next = EdRouter.Instance.NextWaypoint();
						vaProxy.SetText("jumps", EdRouter.Instance.CurrentWaypoint.Jumps.ToString());
						vaProxy.SetText("next_waypoint", next.System);
						break;
					case "prev_waypoint":
						var prev = EdRouter.Instance.PreviousWaypoint();
						vaProxy.SetText("jumps", EdRouter.Instance.CurrentWaypoint.Jumps.ToString());
						vaProxy.SetText("prev_waypoint", prev.System);
						break;
					case "open_gui":
						InvokeConfiguration();
						break;
					case "calculate_route":
						EdRouter.Instance.CalculateRoute();
						vaProxy.SetText("total_jumps", EdRouter.Instance.Route.TotalJumps.ToString());
						break;
					default:
						break;
				}
				WaypointToClipboard();
			}
			catch (Exception ex)
			{
				vaProxy.WriteToLog("Error from ED-Router: " + ex.Message, "red");
			}
		}

		private static void InvokeConfiguration()
		{
			if (window == null)
            {
                var configThread = new Thread(() =>
                {
                    try
                    {
                        window = new MainWindow();
                        window.Show();

                        // We make a dispatcher for ED-Runner so that we do not block EDDI or VoiceAttack when we do ComputeRoute.
                        Dispatcher.Run();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show(ex.InnerException.Message);
                        }
                    }

                });
                configThread.SetApartmentState(ApartmentState.STA);
                configThread.Start();
            }
            else
            {
                window.Dispatcher.Invoke(() => window.Show());
            }
		}

		private static void WaypointToClipboard()
		{
			Thread copyThread = new Thread(() =>
			{
				try
				{
					Clipboard.SetText(EdRouter.Instance.CurrentWaypoint.System);
				}
				catch (Exception)
				{ }

			});
			copyThread.SetApartmentState(ApartmentState.STA);
			copyThread.Start();
		}
	}
}
