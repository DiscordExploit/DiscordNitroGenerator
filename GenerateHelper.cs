using System;
using System.IO;
using System.Linq;

namespace NitroChecker
{
	internal class GenerateHelper
	{
		private static Random randomInstance = new Random();

		private static char getRandomFromStr(string inputStr) => inputStr[randomInstance.Next(inputStr.Length)];
		private static string GenerateRandomCode()
		{
			string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567891234567891234567891234567891234567891234567";
			return new string(Enumerable.Repeat(charset, 16).Select(new Func<string, char>(getRandomFromStr)).ToArray<char>());
		}

		public static void WriteRandomCodes(int amount)
		{
			StreamWriter streamWriter = new StreamWriter("codes.txt");
			try
			{
				for (int i = 1; i < amount; i++)
					streamWriter.WriteLine(GenerateHelper.GenerateRandomCode());
			}
			finally
			{
				((IDisposable)streamWriter).Dispose();
			}
		}
	}
}
