using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace thetruth
{
	class Program
	{
		private const string AnagramText = "poultry outwits ants";
		private const string PhraseMd5 = "4624d200580677270a54ccff86b9610e";
		private const string DictionaryFile = "wordlist";
		private static DateTime _startTime;

		static void Main(string[] args)
		{
			_startTime = DateTime.Now;
			Console.SetWindowPosition(0, 0);
			Console.ForegroundColor = ConsoleColor.DarkGreen;

			Thread.CurrentThread.Priority = ThreadPriority.Highest;

			var result = GetMagicWord();

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

		protected static string GenerateMd5(string phrase)
		{
			return System.BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF7.GetBytes(phrase))).Replace("-", String.Empty).ToLower();
		}

		protected static string GeneratePhrase(params string[] words)
		{
			return String.Join(" ", words);
		}

		// Helper to speed up processing time by ensuring none duplicate characters are used which are not in the original phrase.
		protected static bool ValidatePhrase(string phrase)
		{
			var anagramChars = AnagramText.Replace(" ", String.Empty).ToCharArray().ToList();
			var phraseChars = new String(phrase.Replace(" ", String.Empty).ToArray());

			var matches = false;

			foreach (var phraseChar in phraseChars)
			{
				var exists = anagramChars.FirstOrDefault(c => c == phraseChar);

				if (exists != 0)
				{
					anagramChars.Remove(exists);
					matches = true;
				}
				else
				{
					matches = false;
					break;
				}
			}

			return (matches);
		}

		protected static ResultPhrase GetMagicWord()
		{
			string[] words = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + DictionaryFile);
			var anagramWords = AnagramText.Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var anagramChars = new String(AnagramText.Replace(" ", String.Empty).Distinct().ToArray());

			var matches = (from word in words from anagramWord in anagramWords where anagramWord.Length >= word.Length let charactersMatches = word.ToCharArray().Count(c => anagramChars.Contains(c)) where charactersMatches == word.Length select word).Distinct().ToArray();

			// Based on the benchmark here, we create a for loop with arrays to improve performance
			// http://codebetter.com/patricksmacchia/2008/11/19/an-easy-and-efficient-way-to-improve-net-code-performances/

			for (int i1 = 0; i1 < matches.Length; i1++)
			{
				var word1 = matches[i1];

				// Skip if the first word doesnt match the length of the anagrams first word
				if (word1.Length > anagramWords[0].Length)
				{
					continue;
				}

				for (int i2 = 0; i2 < matches.Length; i2++)
				{
					var word2 = matches[i2];

					var phrase = GeneratePhrase(word1, word2);

					// Skip if the word is longer than the anagram or if it matches the anagrams second word
					if (word1 == word2 || phrase.Length > AnagramText.Length || word2.Length != anagramWords[1].Length || !ValidatePhrase(phrase))
					{
						continue;
					}

					for (int i3 = 0; i3 < matches.Length; i3++)
					{
						var word3 = matches[i3];

						phrase = GeneratePhrase(word1, word2, word3);

						// Skip if the word is longer than the anagram or if it matches the anagrams third word
						// ValidatePhrase ensures that only characters in the same number as the in the anagram.
						if (word2 == word3 || word3 == word1 || word3.Length != anagramWords[2].Length || phrase.Length != AnagramText.Length || !ValidatePhrase(phrase))
						{
							continue;
						}

						Console.WriteLine(String.Format("{0}: {1}", DateTime.Now - _startTime, phrase) );

						var md5 = GenerateMd5(phrase);

						if (md5 == PhraseMd5)
						{
							return new ResultPhrase()
							{
								Date = DateTime.Now,
								Md5 = md5,
								Phrase = phrase
							};
						}
					}
				}
			}
			return null;
		}
	}
}