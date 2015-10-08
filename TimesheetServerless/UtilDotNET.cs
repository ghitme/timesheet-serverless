using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TimesheetServerless
{
	public static class UtilDotNET
	{
		public static readonly string phonePattern1 = @"\d{3}-\d{3}-\d{4}";				//xxx-xxx-xxxx
		public static readonly string emailPattern = @"^\w+\@\w+.com$";

		/*
		 *	Pattern Matching section
		 */
		public static bool IsMatch(string pattern, string testedString)
		{
			Regex rx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

			testedString = testedString.ToLower();					//In case users input capitals

			MatchCollection matches = rx.Matches(testedString);
			int numOfMatches = matches.Count;

			if (numOfMatches == 1)
				return true;
			else
				return false;
		}


		//Cell containing military text format
		public static bool ValidateMilitaryTextFormat(string timeStr)
		{
			DateTime res;
			if (DateTime.TryParseExact(timeStr, "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
				return true;
			return false;
		}
		

		//Check if string is a date and returns a bool
		public static bool ValidateDate(string dateStr)
		{
			DateTime date;
			if (DateTime.TryParse(dateStr, out date))
				return true;
			else
				return false;	
		}



		/*	=====================================================
		 *	StringToDate
		 *	INPUT: string representing date
		 *	=====================================================	
		 */
		public static DateTime StringToDate(string dateStr)
		{
			DateTime date;
			if(DateTime.TryParse(dateStr, out date))
				return date;
			else
				return DateTime.MinValue;			
		}
	}
}
