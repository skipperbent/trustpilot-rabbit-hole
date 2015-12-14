using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace thetruth
{
	public delegate void ChangedEventHandler(object sender, PhraseEventArg e);

	public class Anagram
	{

		// TODO: make more generic by making user input the md5 + anagram text + refactoring getMagicWord method .

		public event ChangedEventHandler Changed;

		public string Phrase { get; set; }
		public string PhraseMd5 { get; set; }
		public string DictionaryFile { get; set; }

		public Anagram(string phrase, string phraseMd5)
		{
			this.Phrase = phrase;
			this.PhraseMd5 = phraseMd5;
		}

		public string GenerateMd5(string phrase)
		{
			return System.BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF7.GetBytes(phrase))).Replace("-", String.Empty).ToLower();
		}

		protected string GeneratePhrase(params string[] words)
		{
			return String.Join(" ", words);
		}

		// Helper to speed up processing time by ensuring none duplicate characters are used which are not in the original phrase.
		protected bool ValidatePhrase(string phrase)
		{
			var anagramChars = Phrase.Replace(" ", String.Empty).ToCharArray().ToList();
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

		public AnagramResultPhrase GetMagicWord()
		{
			return GetMagicWord(false);
		}

		protected virtual void OnChanged(PhraseEventArg e)
		{
			if (Changed != null)
			{
				Changed(this, e);
			}
		}

		public AnagramResultPhrase GetMagicWord(bool fast)
		{
			string[] words = File.ReadAllLines(DictionaryFile);
			var anagramWords = Phrase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			var anagramChars = new String(Phrase.Replace(" ", String.Empty).Distinct().ToArray());

			int? word1MaxLength = null;
			int? word2MaxLength = null;
			int? word3MaxLength = null;

			if (fast)
			{
				word1MaxLength = anagramWords[0].Length;
				word2MaxLength = anagramWords[1].Length;
				word3MaxLength = anagramWords[2].Length;
			}

			var matches = (from word in words from anagramWord in anagramWords where anagramWord.Length == word.Length let charactersMatches = word.ToCharArray().Count(c => anagramChars.Contains(c)) where charactersMatches == word.Length select word).Distinct().ToArray();

			// Based on the benchmark here, we create a for loop with arrays to improve performance
			// http://codebetter.com/patricksmacchia/2008/11/19/an-easy-and-efficient-way-to-improve-net-code-performances/

			for (int i1 = 0; i1 < matches.Length; i1++)
			{
				var word1 = matches[i1];

				// Skip if the first word doesnt match the length of the anagrams first word
				if (word1MaxLength != null && word1.Length < word1MaxLength)
				{
					continue;
				}

				for (int i2 = 0; i2 < matches.Length; i2++)
				{
					var word2 = matches[i2];

					var phrase = GeneratePhrase(word1, word2);

					// Skip if the word is longer than the anagram or if it matches the anagrams second word
					if (word2MaxLength != null && word2.Length < word2MaxLength || word1 == word2 || phrase.Length > Phrase.Length || !ValidatePhrase(phrase))
					{
						continue;
					}

					for (int i3 = 0; i3 < matches.Length; i3++)
					{
						var word3 = matches[i3];

						phrase = GeneratePhrase(word1, word2, word3);

						// Skip if the word is longer than the anagram or if it matches the anagrams third word
						// ValidatePhrase ensures that only characters in the same number as the in the anagram.
						if (word3MaxLength != null && word3.Length < word3MaxLength || word2 == word3 || word3 == word1 || word3.Length != anagramWords[2].Length || !ValidatePhrase(phrase))
						{
							continue;
						}

						var md5 = GenerateMd5(phrase);

						this.OnChanged(new PhraseEventArg()
						{
							Phrase = phrase,
							Md5 = md5,
						});

						if (md5 == PhraseMd5)
						{
							return new AnagramResultPhrase()
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
