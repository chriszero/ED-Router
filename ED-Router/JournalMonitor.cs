using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using ED_Router.Services;
using Newtonsoft.Json.Linq;

namespace ED_Router
{
	public class JournalMonitor : LogMonitor
	{
		private static Regex JsonRegex = new Regex(@"^{.*}$");

		public JournalMonitor() : base(GetSavedGamesDir(), @"^Journal.*\.[0-9\.]+\.log$")
		{ }

		public event Action<string> NewLocation;

		public override void EventCallback(string data)
		{
			string location = GetLocation(data);
			if(!string.IsNullOrEmpty(location))
			{
				//raise event
				NewLocation?.Invoke(location);
			}
		}

		public static string GetLocation(string line)
		{
			var location = "";
			try
			{
				var match = JsonRegex.Match(line);
				if (match.Success)
				{
					var o = JObject.Parse(line);

					var eventType = (string)o["event"];

					switch (eventType)
					{
						case "FSDJump":
						case "Location":
							location = (string)o["StarSystem"];

                            if (location == null)
                            {
								EdRouter.Instance.VoiceAttackAccessor.LogMessage($"EDRouter: ERROR, event missing StarSystem. {line}", MessageColor.Red);
                            }
							break;
					}
                }
			}
			catch (Exception ex)
			{
                EdRouter.Instance.VoiceAttackAccessor.LogMessage($"EDRouter: An error occurred {ex.Message}", MessageColor.Red);
			}
			return location;
		}

		private static string GetSavedGamesDir()
		{
			IntPtr path;
			int result = NativeMethods.SHGetKnownFolderPath(new Guid("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"), 0, new IntPtr(0), out path);
			if (result >= 0)
			{
				return Marshal.PtrToStringUni(path) + @"\Frontier Developments\Elite Dangerous";
			}

            throw new ExternalException("Failed to find the saved games directory.", result);
        }

		internal class NativeMethods
		{
			[DllImport("Shell32.dll")]
			internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
		}

	}
}