using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*
 * Timesheet database record unit
 */
namespace TimesheetServerless
{
	public class WorkWeek : IComparable<WorkWeek>
	{
		public WorkWeek()
		{
			PunchIn = new string[7];
			PunchOut = new string[7];
			LunchIn = new string[7];
			LunchOut = new string[7];
			Reason = new string[7];
			Assoc = new string[7];
			Admin = new string[7];
		}

		//Properties
		public int TableID { get; set; }                 //Primary Key
		public string EmployeeID { get; set; }                          //Repeated for each employee
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public string[] PunchIn { get; set; }           //[Days].......
		public string[] PunchOut { get; set; }          //[Days].......
		public string[] LunchIn { get; set; }           //[Days].......
		public string[] LunchOut { get; set; }          //[Days].......
		public string[] Reason { get; set; }
		public string[] Assoc { get; set; }
		public string[] Admin { get; set; }

		//MM/dd/yyyy
		public string Week { get; set; }                

		//Save employee when added to employee list. Ignore cells fields in TimeSheet form, for now.
		public static readonly string zeroDummy = "0000 0000 0000 0000 0000 0000 0000";
		public static readonly string ellipsisDummy = "... ... ... ... ... ... ...";
		public static readonly string dashDummy = "-- -- -- -- -- -- --";


		/*
		 * Creating multidimensional array for worker data 
		 * Enumerating days of type sbyte (16 bits)
		 */
		public enum Records : sbyte
		{
			PunchIn,			
			LunchIn,
			LunchOut,
			PunchOut,
			Reason,
			Assoc,
			Admin
		};

		public enum Days : sbyte
		{
			Mon,
			Tue,
			Wed,
			Thu,
			Fri,
			Sat,
			Sun
		};

		public static readonly Dictionary<string, Days> daysDict = new Dictionary<string, Days>()
		{
			{Days.Mon.ToString(), Days.Mon},
			{Days.Tue.ToString(), Days.Tue},
			{Days.Wed.ToString(), Days.Wed},
			{Days.Thu.ToString(), Days.Thu},
			{Days.Fri.ToString(), Days.Fri},
			{Days.Sat.ToString(), Days.Sat},
			{Days.Sun.ToString(), Days.Sun}
		};

		public static readonly Dictionary<string, Records> recsDict = new Dictionary<string, Records>()
		{
			{Records.PunchIn.ToString(), Records.PunchIn},			
			{Records.LunchIn.ToString(), Records.LunchIn},
			{Records.LunchOut.ToString(), Records.LunchOut},
			{Records.PunchOut.ToString(), Records.PunchOut},
			{Records.Reason.ToString(), Records.Reason},
			{Records.Assoc.ToString(), Records.Assoc},
			{Records.Admin.ToString(), Records.Admin}			
		};

		//Implement compareTo
		public int CompareTo(WorkWeek other)
		{
			//DateTime myDate = Convert.ToDateTime(Week);
			//DateTime otherDate = Convert.ToDateTime(other.Week);
			DateTime lastMyDate = LastBeginningOfWeek(Week);
			DateTime otherDate = LastBeginningOfWeek(other.Week);

			return lastMyDate.CompareTo(otherDate);                     //Lower to Higher			
		}


		//Convert starting date to weekspan (all strings)
		public static string DateToWeekSpan(string dateStr)
		{
			//Parse value
			DateTime dateVal = new DateTime();
			if (UtilDotNET.ValidateDate(dateStr))
			{
				dateVal = UtilDotNET.StringToDate(dateStr);
			}
			else
				return "ERROR: Please fix starting date from database table";		

			//Last Monday
			int deltaToLastMonday = DayOfWeek.Monday - dateVal.DayOfWeek;
			DateTime lastMonday = dateVal.AddDays(deltaToLastMonday).Date;

			//This sunday
			int deltaToThisSunday = DayOfWeek.Sunday - dateVal.DayOfWeek;
			if (deltaToThisSunday <= (int)dateVal.DayOfWeek)
				deltaToThisSunday += 7;
			DateTime thisSunday = dateVal.AddDays(deltaToThisSunday).Date;

			string weekspan = String.Format("{0}-{1}", lastMonday.ToString("MM/dd/yy"), thisSunday.ToString("MM/dd/yy"));
			return weekspan;
		}

		//INPUT: week span string
		public static string ConvertWeekSpanToBeginningOfWeek(string weekSpan)
		{
			return weekSpan.Substring(0, 8);
		}


		/*	====================================================
		 *	GetLastMondayFromDate
		 *	> Error-catch any possible undesirable change in table values
		 *	====================================================
		 */
		public static DateTime LastBeginningOfWeek(string dateStr)
		{
			DateTime date = UtilDotNET.StringToDate(dateStr);
			//Last Monday
			if (date != DateTime.MinValue)
			{
				int deltaToLastMonday = DayOfWeek.Monday - date.DayOfWeek;
				if(deltaToLastMonday > 0)
				{
					deltaToLastMonday -= 7;
				}
				DateTime lastMonday = date.AddDays(deltaToLastMonday).Date;
				return lastMonday;
			}
			else
				return date;
		}

		

		public static WorkWeek GetCurrentWorkWeek(List<WorkWeek> workWeekList)
		{
			WorkWeek currentWorkWeek = new WorkWeek();
			DateTime today = DateTime.Now;
			DateTime lastMonday = LastBeginningOfWeek(today.ToString());

			for (int i = 0; i < workWeekList.Count; i++)
			{
				DateTime loopDate = LastBeginningOfWeek(workWeekList[i].Week);
				currentWorkWeek	= workWeekList[i];
				if (loopDate == lastMonday)
					break;	
			}
			return currentWorkWeek;
		}


		/*
		 * INPUT: Reference multidim array
		 * OUTPUT: void (reference) indexes with dim equal to workweek records, and days size.
		 * USAGE: eg myIndexes[Days.Mon][Records.PunchIn] == ParseCell(e)
		 */
		public static void GetIndexes(ref string[][] myIndexes)
		{
			myIndexes = new string[Enum.GetValues(typeof(Records)).Length][];
			var days = Enum.GetValues(typeof(Days));
			for (int i = 0; i < myIndexes.Count(); i++)
			{
				myIndexes[i] = new string[days.Length];                       //Ini arrays of arrays (jagged)
			}
		}



	}

}
