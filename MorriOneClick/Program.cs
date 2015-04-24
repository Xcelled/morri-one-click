using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MorriOneClick
{
	class Program
	{
		private static readonly string MboxTitle = "MorriOneClick";

		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			if (!File.Exists("morrighan.exe"))
			{
				MessageBox.Show("Cannot locate 'morrighan.exe'!\n\nPlease make sure MorriOneClick is in the same folder as Morrighan.",
					MboxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

				return;
			}

			Trace.TraceInformation("Retrieving NA args...");

			var naArgs = GetNaArgs();

			Trace.TraceInformation("Starting with args: " + naArgs);

			Trace.TraceInformation("Starting Morrighan...");

			var startInfo = new ProcessStartInfo("morrighan.exe", naArgs);

			Process.Start(startInfo);
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			MessageBox.Show("MorriOneClick has encountered an unexpected error:\n\n" + (e.ExceptionObject as Exception).Message, MboxTitle,
				MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		public static string GetNaArgs()
		{
			var ret = "code:1622 logport:11000";
			using (var wc = new System.Net.WebClient())
			{
				var opi = wc.DownloadString("http://mabipatchinfo.nexon.net/patch/patch.txt");
				using (var r = new StringReader(opi))
				{
					while (r.Peek() != -1)
					{
						var line = r.ReadLine();
						if (string.IsNullOrWhiteSpace(line) || !line.Contains('='))
							continue;
						var parts = line.Split(new[] { '=' }, 2);

						if (parts[0] == "login")
							ret += " logip:" + parts[1];
						else if (parts[0] == "main_version")
							ret += " ver:" + parts[1];
						else if (parts[0] == "arg")
							ret += " " + parts[1];
					}
				}
			}

			return ret;
		}
	}
}
