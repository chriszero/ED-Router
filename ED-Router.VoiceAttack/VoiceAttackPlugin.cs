using ED_Router.UI.Desktop;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ED_Router.UI.Desktop.Services;

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
            var configThread = new Thread(Dispatcher.Run);
            configThread.SetApartmentState(ApartmentState.STA);
            configThread.Start();

            for (var i = 0; i < 10; i++)
            {
                var dispatcher = Dispatcher.FromThread(configThread);

                if (dispatcher != null)
                {
                    dispatcher.Invoke(() =>
                    {
                        window = new MainWindow();
                    });

                    break;
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }

            if (window == null)
            {
                vaProxy.WriteToLog($"{VA_DisplayName()} did not correctly load.", "red");
                return;
            }

            EdRouter.Dispatcher = new DispatcherAccessor(window.Dispatcher);

            App.IsFromVA = true;

            vaProxy.WriteToLog($"{VA_DisplayName()} ready!", "green");
        }

		public static void VA_Exit1(dynamic vaProxy)
		{
            App.IsFromVA = false;
            window?.Dispatcher?.BeginInvoke((Action) window.Close);
            (EdRouter.Dispatcher as IDisposable)?.Dispose();
            window?.Dispatcher?.InvokeShutdown();
            window = null;
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
                        if (next != null)
                        {
                            vaProxy.SetText("jumps", EdRouter.Instance.CurrentWaypoint.Jumps.ToString());
                            vaProxy.SetText("next_waypoint", next.System);
						}
						break;
					case "prev_waypoint":
						var prev = EdRouter.Instance.PreviousWaypoint();
                        if (prev != null)
                        {
                            vaProxy.SetText("jumps", EdRouter.Instance.CurrentWaypoint.Jumps.ToString());
                            vaProxy.SetText("prev_waypoint", prev.System);
                        }
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
            window?.Dispatcher?.Invoke(() => window.Show());
        }

		private static void WaypointToClipboard()
		{
			var copyThread = new Thread(() =>
			{
                try
                {
                    if (EdRouter.Instance.CurrentWaypoint == null)
                    {
                        return;
                    }

                    Clipboard.SetText(EdRouter.Instance.CurrentWaypoint.System);
                }
                catch
                {
                    
                }

			});
			copyThread.SetApartmentState(ApartmentState.STA);
			copyThread.Start();
		}
	}
}
