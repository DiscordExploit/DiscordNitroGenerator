using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Colorful;
using Leaf.xNet;

namespace NitroChecker
{
	internal class CheckerHelper
	{
		public static List<string> proxies = new List<string>();
		public static List<string> accounts = new List<string>();

		public static int totalCodes;
		public static int bad = 0;
		public static int hits = 0;
		public static int err = 0;
		public static int check = 0;
		public static int accIndex = 0;
		public static string proxyType = "";
		public static int proxyIndex = 0;
		public static int proxyTotal = 0;
		public static int stop = 0;
		public static int CPM = 0;
		public static int CPM_aux = 0;
		public static int threads;

		public static void LoadCodes()
		{
			using (FileStream fileStream = File.Open("codes.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (BufferedStream bufferedStream = new BufferedStream(fileStream))
					using (StreamReader streamReader = new StreamReader(bufferedStream))
						while (streamReader.ReadLine() != null)
						{
							totalCodes++;
						}
		}

		public static void LoadProxies(string fileName)
		{
			using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (BufferedStream bufferedStream = new BufferedStream(fileStream))
					using (StreamReader streamReader = new StreamReader(bufferedStream))
						while (streamReader.ReadLine() != null)
						{
							proxyTotal++;
						}
		}

		public static void UpdateTitle()
		{
			while (true)
			{
				CPM = CPM_aux;
				CPM_aux = 0;

				Colorful.Console.Title = string.Format("[Nitro Generator & Checker] | Proxy: " + proxyType + " | Checked: {0}/{1} | Hits: {2} | Bad: {3} | CPM: ", new object[]
				{
					check, totalCodes, hits, bad, err
				}) + (CPM * 60).ToString() + " | Crée par Stanley#0001";

				Thread.Sleep(1000);
			}
		}

		public static void ParseHttpResponse(string httpResponse, string currentCode)
        {
			if (httpResponse.Contains("Unknown Gift Code"))
			{
				CPM_aux++; check++; bad++;
				Colorful.Console.WriteLine("[BAD] " + currentCode, Color.DarkRed);
			}
			else if (httpResponse.Contains(", \"name\": \"Nitro\", \"summary\":"))
			{
				CPM_aux++; check++; hits++;
				Colorful.Console.WriteLine("[GOOD] " + currentCode, Color.DarkGreen);
				CheckerHelper.SaveData(currentCode);
			}
			/*else if (httpResponse.Contains("You are being rate limited") ||
				httpResponse.Contains("You have tried to access a web page which is in violation of your internet usage policy"))
			{
				CheckerHelper.accounts.Add(currentCode);
			}*/
			else
			{
				CheckerHelper.accounts.Add(currentCode);
			}
		}
		public static void Check()
		{
			while (true)
			{
				if (CheckerHelper.proxyIndex > CheckerHelper.proxies.Count() - 2)
					CheckerHelper.proxyIndex = 0;

				try
				{
					Interlocked.Increment(ref CheckerHelper.proxyIndex);
					using (HttpRequest httpRequest = new HttpRequest())
					{
						if (CheckerHelper.accIndex >= CheckerHelper.accounts.Count())
						{
							CheckerHelper.stop++;
							break;
						}

						string currentCode =
							CheckerHelper.accounts[CheckerHelper.accIndex];

						Interlocked.Increment(ref CheckerHelper.accIndex);

						try
						{
							switch (proxyType)
                            {
								case "HTTP":
									httpRequest.Proxy = HttpProxyClient.Parse(CheckerHelper.proxies[CheckerHelper.proxyIndex]);
									break;
								case "SOCKS4":
									httpRequest.Proxy = Socks4ProxyClient.Parse(CheckerHelper.proxies[CheckerHelper.proxyIndex]);
									break;
								case "SOCKS5":
									httpRequest.Proxy = Socks5ProxyClient.Parse(CheckerHelper.proxies[CheckerHelper.proxyIndex]);
									break;
								default: // NO
									httpRequest.Proxy = null;
									break;
							}

							if (httpRequest != null) httpRequest.Proxy.ConnectTimeout = 5000;

							httpRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
							httpRequest.KeepAlive = true;
							httpRequest.IgnoreProtocolErrors = true;
							httpRequest.ConnectTimeout = 5000;
							httpRequest.UseCookies = true;
							httpRequest.AddHeader("Authorization", "mfa.TrYd-1Giyv9PUgpiv09tE9zoUfmYZIHjbvRx9K4bxl3TsaQ4h3Mjrj6NV0ro9ImI2q1fdXr104EC79H-NS0r");
							httpRequest.AddHeader("X-Fingerprint", "622832459796709396.no-ggtFhW5yweBngUZhaXThqlKk");

							string httpResponse = httpRequest.Get(
								string.Format("https://discordapp.com/api/v6/entitlements/gift-codes/{0}?with_application=true&with_subscription_plan=true",
									new string[] { CheckerHelper.accounts[CheckerHelper.accIndex] }
								),
								null
							).ToString();

							ParseHttpResponse(httpResponse, currentCode);
						}
						catch (Exception)
						{
							accounts.Add(currentCode);
						}
					}
				}
				catch
				{
					Interlocked.Increment(ref err);
				}
			}
		}

		public static void SaveData(string code)
		{
			try
			{
				using (StreamWriter streamWriter = File.AppendText("hits.txt"))
				{
					streamWriter.WriteLine("--------------------| Nitro Gift Code |----------------------");
					streamWriter.WriteLine("- Link: http://discord.gift/" + code);
					streamWriter.WriteLine("-------------------------------------------------------------");
					streamWriter.WriteLine();
				}
			}
			catch
			{
			}
		}


	}
}
