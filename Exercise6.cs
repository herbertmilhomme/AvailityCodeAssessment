using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AvailityCodeAssessment
{
	/// <summary>
	/// Availity receives enrollment files from various benefits management and enrollment solutions(I.e.HR platforms, payroll platforms).
	/// Most of these files are typically in EDI format. However, there are some files in CSV format.
	/// For the files in CSV format, write a program that will read the content of the file and separate enrollees by insurance company in its own file.
	/// Additionally, sort the contents of each file by last and first name (ascending).
	/// Lastly, if there are duplicate User Ids for the same Insurance Company, then only the record with the highest version should be included.
	/// </summary>
	[TestClass]
	public class Exercise6
	{
		private readonly string outputPath = Directory.GetCurrentDirectory() + "\\output"; //nesting files to reduce conflicts

		[TestMethod]
		public void TestMethod1()
		{
			//string? failed = null;
			//Assert.IsTrue(failed == null,failed); //error will return the string that failed
			Assert.IsTrue(StartHere(""));
		}

		/// <summary>
		/// Takes import file, and process it for database migration
		/// </summary>
		/// <param name="filepath">Master File</param>
		/// <returns></returns>
		private bool StartHere(string filepath)
		{
			if (string.IsNullOrEmpty(filepath)) return false;
			if (!System.IO.File.Exists(filepath)) return false;

			//Read the contents of master import file
			using (StreamReader sr = new StreamReader(filepath))
			{
				string? currentLine;
				int i = 0;
				// currentLine will be null when the StreamReader reaches the end of file
				while ((currentLine = sr.ReadLine()) != null)
				{
					if (i == 0)
						continue; //first line is usually the headers in csv file
					string[] fields = currentLine.Split(','); //CSV means files are separated by Commas

					string company = string.Empty;
					//Company name may have commas in their spelling?...
					for (int n = 4; n < fields.Length; n++)
					{
						company += fields[n];
					}

					//I'll assume that the headers are presented in th order listed in the instructions, as most EDIs are consistent with their column IDs
					EDI edi = new EDI()
					{
						UserId = fields[0].Trim(),
						FirstName = fields[1].Trim(),
						LastName = fields[2].Trim(),
						Version = int.Parse(fields[3].Trim()),
						InsuranceCompany = company.Replace('"',' ').Trim() //remove string identifier if there happened to be any because of commas
					};
					Append(string.Format("{0}\\{1}",outputPath, edi.InsuranceCompany), edi); //files are named after company
					i++;
				}
			}

			string[] fileNames = System.IO.Directory.GetFiles(
				outputPath,
				"*.csv",
				SearchOption.TopDirectoryOnly);
			//after separating the records into different files...
			foreach(string file in fileNames)
			{
				//this entire loop could've been done as async, but the rest of the code is synchronous and not enough time to work out logic and bug-test
				if(!Sort(string.Format("{0}\\{1}", outputPath, file)))
					return false;
			}
			return true;
		}

		private bool Append(string filepath, EDI record)
		{
			if (string.IsNullOrEmpty(filepath)) return false;
			if (!System.IO.File.Exists(filepath))
			{
				File.Create(filepath).Close();
				//File.AppendAllLines(filepath, new string[] { "table headers here with comma", //comment out here to remove else on line 96
				//	string.Format("{0}, {1}, {2}, {3}, {4}", record.UserId, record.FirstName, record.LastName, record.Version, record.InsuranceCompany) });
				File.AppendAllText(filepath, "table headers here with comma" + Environment.NewLine); //Since csv needs header, i just added some
			}
			//else
				File.AppendAllText(filepath, string.Format("{0}, {1}, {2}, {3}, {4}", record.UserId, record.FirstName, record.LastName, record.Version, record.InsuranceCompany) + Environment.NewLine);

			return true;
		}

		/// <summary>
		/// Locates existing file and sorts through values;
		/// If file does not exist it'll just create one.
		/// </summary>
		/// <param name="filepath"></param>
		/// <returns></returns>
		private bool Sort(string filepath)
		{
			if (string.IsNullOrEmpty(filepath)) return false;
			List<string> text = new List<string>();
			text.Add("table headers here with comma"); //Since csv needs header, i just added some first
			List<EDI> records = new List<EDI>();

			if (System.IO.File.Exists(filepath))
			{
				using (StreamReader sr = new StreamReader(filepath))
				{
					string? currentLine;
					int i = 0;
					// currentLine will be null when the StreamReader reaches the end of file
					while ((currentLine = sr.ReadLine()) != null)
					{
						if (i == 0)
							continue; //first line is usually the headers in csv file
						string[] fields = currentLine.Split(','); //CSV means files are separated by Commas

						string company = string.Empty;
						//Company name may have commas in their spelling?...
						for (int n = 4; n < fields.Length; n++)
						{
							company += fields[n];
						}

						//I'll assume that the headers are presented in th order listed in the instructions, as most EDIs are consistent with column IDs
						records.Add(new EDI()
						{
							UserId = fields[0].Trim(),
							FirstName = fields[1].Trim(),
							LastName = fields[2].Trim(),
							Version = int.Parse(fields[3].Trim()),
							InsuranceCompany = company.Replace('"', ' ').Trim() //remove string identifier if there happened to be any because of commas
						});
						i++;
					}
				}
				File.Delete(filepath);
			}
			File.Create(filepath).Close();

			records
				.OrderBy(x => x.LastName)
				.ThenBy(y => y.FirstName)
				//.ThenByDescending(z => z.Version) //moved to line 159
				//.GroupBy(p => new { p.InsuranceCompany, p.LastName, p.FirstName }) //oops, duplicate check was for user id... not names
				.GroupBy(p => new { p.InsuranceCompany, p.UserId }) //will make query for distinct
				.Select(grp => {
					EDI record = grp
						.OrderByDescending(z => z.Version)
						.First();
					text.Add(string.Format("{0}, {1}, {2}, {3}, {4}", record.UserId, record.FirstName, record.LastName, record.Version, record.InsuranceCompany));
					return record;
				}); //n => n.Version));
			File.AppendAllLines(filepath, text);
			return true;
		}
	}

	public class EDI
	{
		public string? UserId { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public int Version { get; set; }
		public string? InsuranceCompany { get; set; }
	}
}