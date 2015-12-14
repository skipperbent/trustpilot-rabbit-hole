using System;
using System.Threading;

namespace thetruth
{
	class Program
	{
		private static DateTime _startTime;

		static void Main(string[] args)
		{
			_startTime = DateTime.Now;
			Console.ForegroundColor = ConsoleColor.DarkGreen;

			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			var anagram = new Anagram("poultry outwits ants", "4624d200580677270a54ccff86b9610e")
			{
				DictionaryFile = AppDomain.CurrentDomain.BaseDirectory + @"\wordlist"
			};

			anagram.Changed += AnagramOnChanged;

			// Try first assuming that the words always have the same length (it's fast so worth the try), otherwise try all matching words.

			var result = anagram.GetMagicWord(true) ?? anagram.GetMagicWord();

			Console.Clear();

			if (result != null)
			{
				Console.WriteLine(String.Format("{0}: MAGIC WORD FOUND!\n\nPhrase: {1}\nMD5: {2}", DateTime.Now - _startTime, result.Phrase, result.Md5));
			}
			else
			{
				Console.WriteLine(String.Format("{0}: No match found :(", DateTime.Now - _startTime));
			}

			Console.ReadLine();
		}

		private static void AnagramOnChanged(object sender, PhraseEventArg phraseEventArg)
		{
			Console.WriteLine(String.Format("{0}: {1}", DateTime.Now - _startTime, phraseEventArg.Phrase));
		}
	}
}