using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimeSettlement
{
   public class WorkingDay
    {
        public string Date { get; set;}
        public TimeSpan Entrance { get; set; }
        public TimeSpan Exit { get; set; }
        public TimeSpan Balance { get; set; }

        internal void AddBalanse(TimeSpan diffTs)
        {
            Balance += diffTs;
            Exit += diffTs;
        }

        public string GetDescriptionOfWorkingTime()
        {
            if (Balance==null)
            {
                throw new ArgumentNullException("Balance");
            }
            string result = $"{Date}:   ";
            string format = @"hh\:mm";
            if (Balance == TimeSpan.Zero)
                result += "Prawidłowy czas pracy.";
            else if (Balance == new TimeSpan(-8, 0, 0))
            {
                result += "Brak karty / urlop";
            }
            else if (Balance > TimeSpan.Zero)
            {
                var exitShouldBe = Exit - Balance;
                result += $"Nadgodziny od {exitShouldBe.ToString($"{format}")} do {Exit.ToString($"{format}")}";
            }
            else
            {
                //Balanse will be negative. "- Balanse" in fact will add time.
                var exitShouldBe = Exit - Balance;
                result += $"Wyjście prywatne od {Exit.ToString($"{format}")} do {exitShouldBe.ToString($"{format}")}";
            }

            return result;

        }
    }
}
