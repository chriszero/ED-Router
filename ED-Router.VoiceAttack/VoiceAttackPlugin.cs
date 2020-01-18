using ED_Router.UI.Desktop;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ED_Router.Events;
using ED_Router.Extensions;
using ED_Router.Services;
using ED_Router.UI.Desktop.Services;
using ED_Router.VoiceAttack.Extensions;
using ED_Router.VoiceAttack.Services;

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
			return "VoiceAttack ED-Router Plugin\r\n\r\nRoute your neutron highway.\r\n\r\nCreated by chriszero (2018).\r\n\r\nMaintained by dominiquesavoie (2019)";  //this is just extended info that you might want to give to the user.  note that you should format this up properly.
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
                    dispatcher.UnhandledException += DispatcherOnUnhandledException;

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

            var voiceAttackAccessor = new VoiceAttackAccessor();

			voiceAttackAccessor.EventSent += (sender, args) =>
            {
                HandleEvents(args, ref vaProxy);
            };

            voiceAttackAccessor.LogSent += (sender, args) => { WriteToLog(ref vaProxy, args.Message, args.Color); };

            voiceAttackAccessor.VariableSent += (sender, args) => { SetVariable(ref vaProxy, args.ValueType, args.VariableName, args.Value); };

			EdRouter.Init(new DispatcherAccessor(window.Dispatcher), voiceAttackAccessor);

            App.IsFromVA = true;

            vaProxy.WriteToLog($"{VA_DisplayName()} ready!", MessageColor.Green.ToLogColor());
        }

        private static void SetVariable(ref dynamic vaProxy, Type variableType, string variableName, object value)
        {
            if(value == null) return;

            if (variableType == typeof(int))
            {
                vaProxy.SetInt($"EDRouter {variableName}", (int)value);
            }
            else if(variableType == typeof(decimal))
            {
                vaProxy.SetDec($"EDRouter {variableName}", (decimal)value);
            }
            else
            {
                vaProxy.SetText($"EDRouter {variableName}", value.ToString());
            }
        }

        private static void WriteToLog(ref dynamic vaProxy, string message, MessageColor color = MessageColor.Blue)
        {
            vaProxy.WriteToLog($"EdRouter: {message}", color.ToLogColor());
        }

        private static void HandleEvents(RouterEventArgs @event, ref dynamic vaProxy)
        {
            var vaCommandName = $"((EDRouter {@event.EventName.ToLowerInvariant()}))";
            if (@event.EmitEvent && vaProxy.CommandExists(vaCommandName))
            {
                vaProxy.ExecuteCommand(vaCommandName);
            }

            foreach (var voiceAttackVariable in @event.EventArgs)
            {
                SetVariable(ref vaProxy, voiceAttackVariable.Type, voiceAttackVariable.VariableName, voiceAttackVariable.VariableValue);
            }

            if (@event is IWaypointEvent waypointEvent && waypointEvent.CopyToClipboard)
            {
                WaypointToClipboard();
            }
        }


        private static void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            EdRouter.Instance.VoiceAttackAccessor.LogMessage($"{VA_DisplayName()}: unhandled error, {e.Exception.Message}.", MessageColor.Red);
        }

        public static void VA_Exit1(dynamic vaProxy)
		{
            App.IsFromVA = false;
            if (window?.Dispatcher != null)
            {
				window.Dispatcher.UnhandledException -= DispatcherOnUnhandledException;
				window.Dispatcher.BeginInvoke((Action)window.Close);
            }
			EdRouter.Instance.Dispose();
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
                            foreach (var variable in next.SystemJumpToVoiceAttackVariables())
                            {
                                SetVariable(ref vaProxy, variable.Type, variable.VariableName, variable.VariableValue);
                            }
						}
						break;
					case "prev_waypoint":
						var prev = EdRouter.Instance.PreviousWaypoint();
                        if (prev != null)
                        {
                            foreach (var variable in prev.SystemJumpToVoiceAttackVariables())
                            {
                                SetVariable(ref vaProxy, variable.Type, variable.VariableName, variable.VariableValue);
                            }
                        }
						break;
					case "open_gui":
						InvokeConfiguration();
						break;
					case "calculate_route":
						EdRouter.Instance.CalculateRoute();
                        break;
					case "toggle_automate_next_waypoint" :
                        EdRouter.Instance.EnableAutoWaypoint = !EdRouter.Instance.EnableAutoWaypoint;
                        break;
                    case "automate_next_waypoint_on":
                        EdRouter.Instance.EnableAutoWaypoint = true;
                        break;
                    case "automate_next_waypoint_off":
                        EdRouter.Instance.EnableAutoWaypoint = false;
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
