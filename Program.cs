using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Colorful;

namespace NitroChecker
{
	internal class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Program.Menu(args);
		}

		private static void Menu(string[] args)
		{

			Colorful.Console.WriteAscii("Stan Generator", Color.FromArgb(251, 51, 0));



			Colorful.Console.Title = (Colorful.Console.Title = "[Nitro Generator & Checker] | Make by Stanley#0001");
			Thread.Sleep(250);
			Colorful.Console.WriteLine("[1] Créer des codes nitros", Color.Orange);
			Colorful.Console.WriteLine("[2] Check des codes nitros", Color.Orange);
			Colorful.Console.WriteLine();
			Colorful.Console.Write("", Color.DarkOrange);
			string text = System.Console.ReadLine();
			string text2 = text;
			if (text2 != null)
			{
				if (!(text2 == "1"))
				{
					if (text2 == "2")
					{
						Colorful.Console.WriteLine();
						Colorful.Console.Write("Combien de ", Color.Orange);
						Colorful.Console.Write("THREADS", Color.Orange);
						Colorful.Console.Write(" voulez-vous utilisez", Color.Orange);
						Colorful.Console.Write(": ", Color.Orange);
						try
						{
							CheckerHelper.threads = int.Parse(Colorful.Console.ReadLine());
						}
						catch
						{
							CheckerHelper.threads = 100;
						}
						for (; ; )
						{
							Colorful.Console.Write("> Quel type de ", Color.Orange);
							Colorful.Console.Write("PROXY ", Color.White);
							Colorful.Console.Write("[HTTP, SOCKS4, SOCKS5, NO]", Color.DarkMagenta);
							Colorful.Console.Write(": ", Color.DarkMagenta);
							CheckerHelper.proxyType = Colorful.Console.ReadLine().ToUpper();

							string[] proxyTypeList = {
								"HTTP", "SOCKS4", "SOCKS5", "NO"
							};

							if (proxyTypeList.Contains(CheckerHelper.proxyType))
								break;

							Colorful.Console.Write("> Veuillez choisir un format valide.\n\n", Color.Red);
							Thread.Sleep(1000);
						}

						Task.Factory.StartNew(delegate ()
						{
							CheckerHelper.UpdateTitle();
						});

						Colorful.Console.WriteLine();
						CheckerHelper.accounts = new List<string>(File.ReadAllLines("codes.txt"));
						CheckerHelper.LoadCodes();
						Colorful.Console.Write("> ");
						Colorful.Console.Write(CheckerHelper.totalCodes, Color.DarkMagenta);
						Colorful.Console.WriteLine(" codes nitros ajoutés\n");

						OpenFileDialog openFileDialog = new OpenFileDialog();
						if (CheckerHelper.proxyType != "NO")
						{
							string fileName;
							do
							{
								Colorful.Console.WriteLine("Choisissez vos proxies", Color.DarkMagenta);
								Thread.Sleep(500);
								openFileDialog.Title = "Select Proxy List";
								openFileDialog.DefaultExt = "txt";
								openFileDialog.Filter = "Text files|*.txt";
								openFileDialog.RestoreDirectory = true;
								openFileDialog.ShowDialog();
								fileName = openFileDialog.FileName;
							}  while (!File.Exists(fileName));

							CheckerHelper.proxies = new List<string>(File.ReadAllLines(fileName));
							CheckerHelper.LoadProxies(fileName);
							Colorful.Console.Write("> ");
							Colorful.Console.Write(CheckerHelper.proxyTotal, Color.DarkMagenta);
							Colorful.Console.WriteLine(" proxies ajoutés\n");
						}

						for (int i = 1; i <= CheckerHelper.threads; i++)
							new Thread(new ThreadStart(CheckerHelper.Check)).Start();

						Colorful.Console.ReadLine();
						Environment.Exit(0);
					}
				}
				else
				{

					int codesAmount;

					Colorful.Console.WriteLine();
					Colorful.Console.Write("Combien de ", Color.Orange);
					Colorful.Console.Write("CODES", Color.Orange);
					Colorful.Console.Write(" voulez-vous générer", Color.Orange);
					Colorful.Console.Write(": ", Color.Orange);
					
					try
					{
						codesAmount = int.Parse(Colorful.Console.ReadLine());
					} catch
					{
						codesAmount = 25000;
					}

					Colorful.Console.WriteLine("Génération de " + codesAmount.ToString() + " codes...");

					Task.Factory.StartNew(delegate ()
					{
						GenerateHelper.WriteRandomCodes(codesAmount);
					}).Wait();

					Colorful.Console.WriteLine("Codes générés avec succès; retour au hub...", Color.Green);
					Thread.Sleep(1000);
                    System.Console.Clear();

					Program.Main(args);
					System.Console.ReadLine();
				}
			}
		}
	}
}
