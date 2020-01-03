using Newtonsoft.Json.Linq;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

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
			string location = "";
			try
			{
				Match match = JsonRegex.Match(line);
				if (match.Success)
				{
					JObject o = JObject.Parse(line);
					/*dynamic ev = JObject.Parse(line);
					string loca = ev.StarSystem;*/

					string eventType = (string)o["event"];

					switch (eventType)
					{
						case "StartJump":
						case "FSDJump":
						case "Location":
							location = (string)o["StarSystem"];
							
							break;
					}

				}
			}
			catch (Exception)
			{
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
			else
			{
				throw new ExternalException("Failed to find the saved games directory.", result);
			}
		}

		internal class NativeMethods
		{
			[DllImport("Shell32.dll")]
			internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);
		}

	}
}