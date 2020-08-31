using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimeSettlement
{
    public static class WorkingDayFactory
    {
        public static List<WorkingDay> GetWorkingDays(List<WorkingDayRecord> workingData)
        {
            var result = new List<WorkingDay>();
            foreach (var workingDayRecord in workingData)
            {
                var workingDay = GetWorkingDay(workingDayRecord);
                result.Add(workingDay);
            }
            return result;
        }

        public static WorkingDay GetWorkingDay(WorkingDayRecord workingData)
        {
            var result = new WorkingDay();
            
            var entrance = cellStringToTimeSpan(workingData.Entrance);
            var exit = cellStringToTimeSpan(workingData.Exit);
            var balanse = cellStringToTimeSpan(workingData.Balance);
            if (balanse == TimeSpan.Zero)
                balanse = cellStringToTimeSpan(workingData.ToRecieve);

            result.Entrance = entrance;
            result.Exit = exit;
            result.Balance = balanse;
            result.Date = workingData.Date;
            return result;
        }
        private static bool IsIncorrectTimeFormat(string cellValue)
        {
            if (cellValue == String.Empty || String.IsNullOrWhiteSpace(cellValue))
                return true;
            else if (cellValue.Length < 4 || cellValue.Length > 5)
                return true;
            else if (String.Equals(cellValue[cellValue.Length - 3], ':') != true)
                return true;

            return false;
        }
        private static TimeSpan cellStringToTimeSpan(string entrace)
        {
            //Correct formats: '0:15' or '-0:15' or '10:15'
            if (IsIncorrectTimeFormat(entrace))
            {
                entrace = tryToChangeTextFormat(entrace);
                    if (IsIncorrectTimeFormat(entrace))
                        return TimeSpan.Zero;
            }
            var stringLenght = entrace.Length;
            string hourRecord;
            if (stringLenght == 4)
                hourRecord = entrace.Substring(0, 1);
            else if (entrace[0] == '-')
                hourRecord = entrace.Substring(1, 1);
            else
                hourRecord = entrace.Substring(stringLenght-5, 2);

            int hours = Convert.ToInt32(hourRecord);
            var minuteRecord = entrace.Substring(entrace.Length-2,2);
            int minutes = Convert.ToInt32(minuteRecord);

            var result = new TimeSpan(hours, minutes, 0);

            if (entrace[0] == '-')
                result = TimeSpan.Zero - result;
            return result;
        }

        private static string tryToChangeTextFormat(string text)
        {
            //Correct formats: '0:15' or '-0:15' or '10:15'
            if (text == null)
                return null;

            string result=text;
            if (decimal.TryParse(text, out var x))
            {
                decimal minutes = Math.Round(x * 60 * 24,0);
                var Ts = new TimeSpan(0, Convert.ToInt32(minutes), 0);
                result = Ts.ToString(@"h\:mm");
                if (minutes<0)
                {
                    result = "-" + result;
                }
            }
            else if (text.Length >7)
            {
                result = text.Substring(text.Length - 5 - 3, 5);
            }

            return result;
        }

        private static TimeSpan convertExit()
        {
            return new TimeSpan();

        }
    }
}
