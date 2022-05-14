using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AvailityCodeAssessment
{
	/// <summary>
	/// Coding exercise (using C#): You are tasked to write a checker that validates the parentheses of a LISP code.
	/// Write a program which takes in a string as an input and returns true if all the parentheses in the string are properly closed and nested.
	/// </summary>
	[TestClass]
	public class Exercise5
	{
		[TestMethod]
		public void TestMethod1()
		{
			string? failed = null;
			string[] list = new string[]
			{
				"list of strings to test",
				"list of strings to test)",
				"((developer or engineer) or (frontend or backend)) and dba"
			};
			foreach(string str in list)
			{
				if(!validateString(str))
					failed = str;
			}

			Assert.IsTrue(failed == null,failed); //error will return the string that failed
		}

		bool validateString(string input)
		{
			if (input != null)
			{
				//bool open = false;
				int i = 0; //didn't want to make for-loop for character index
				int d = 0; //represents depth of nesting or index of parenthesis
				Dictionary<int,bool> pairs = new Dictionary<int,bool>();
				foreach(char c in input)
				{
					if (c == '(')
					{
						pairs.Add(d, true); //add pairs to list, to keep track of how many are opened
						d++;
					}
					if(c == ')')
					{
						if (!pairs.ContainsKey(i)) //if close exist before one is created, catch error and return false
							//|| !pairs[i]) //if an extra close bracket is lingering in string, catch bad pair and return false
							return false;
						pairs[i] = false; //close the pairs from the bottom, rather than match by their id
						i++;
						//d--;
					}
					//i++;
				}
				if (pairs.ContainsValue(true)) //|| d != 0
					return false;
			}
			return true;
		}

		/// <summary>
		/// Recursive loop to validate if open brackets are closed
		/// </summary>
		/// <param name="input"></param>
		/// <returns>returns true if string has open-close bracket</returns>
		/// I didn't properly debug this one, so not sure if it's a valid attempt, but this could be considered test one
		bool isClosed(string input)
		{
			if (input != null)
			{
				bool closed = false;
				int i = 0; //didn't want to make for-loop
				foreach(char c in input)
				{
					i++; //begin at next char in array
					if(c == '(')
					{
						if(closed) //&& !isClosed(input)
							isClosed(input.Substring(i));
						//else
						//	open = true;
						return false; //if EOF and still open, it's not closed
					}
					if(c == ')')
					{
						if (closed)
							closed = false;
						return true;
					}
				}
			}
			return false;
		}
	}
}