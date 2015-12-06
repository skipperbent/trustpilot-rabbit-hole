# trustpilot-rabbit-hole test
My take on creating the algorithm for finding the magic word in TrustPilot's rabbit hole job-test. This project finds the right combination for the "magic word" using a very simple algorithm.

## The mission

Find the correct word for the MD5 key - using only a wordlist and a anagram phrase.

http://followthewhiterabbit.trustpilot.com/

## Screenshot

![Screenshot](https://github.com/skipperbent/trustpilot-rabbit-hole/blob/master/screenshot.gif?raw=true)

**Enough wierd talk, let's try to break it down into steps...**

- We know that the anagram is ```poultry outwits ants``` and has the following MD5 key ```4624d200580677270a54ccff86b9610e```.
This allows us to creating a combination of every word provided in the dictionary, but for now - let's skip that, as theres way too many combinations.

- Let's filter the word-list so it only shows the words containing the characters from ```poultry outwits ants```. So ```p```, ```o```, ```u```, ```l``` should be present within the word in the dictionary... you get the point.

- Okay, now we've got a list of around ```790``` words. This is still way to many, as it can contain combinations of ```790 * 790 * 790```, which is way more than 1 million combinations.

- By doing this we've already increase the chance dramaticly for finding the correct word, but it there's still too many combinations. So let's filter even more. There's a very small chance that the "magic word" has the same number of letters for each words, so let's take that chance and filter even further. It will increase the speed dramatically, so it's worth the try.

- By looping through these words and ensuring that only characters appear the number of times that they do in the anagram-phrase, and that the total phrase-word is not greather- or less than the anagram length, we can actually filter even further on the list above.

- That's pretty much it!

## The MIT License (MIT)

Copyright (c) 2015 Simon Sessingø

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
