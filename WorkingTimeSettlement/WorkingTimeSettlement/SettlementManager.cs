using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimeSettlement
{
    public class SettlementManager
    {
        private List<WorkingDay> DaysWithOverhours
        {
            get => _allDays.Where(d=>d.Balance > TimeSpan.Zero).OrderBy(d => d.Balance).ToList();
        }
        private List<WorkingDay> DaysToSettle
        {
            get => _allDays.Where(d => d.Balance < TimeSpan.Zero && d.Balance > new TimeSpan(-8,0,0)).OrderBy(d => d.Balance).ToList();
        }
        private List<WorkingDay> DaysOutOfWork
        {
            get => _allDays.Where(d => TimeSpan.Equals(d.Balance,new TimeSpan(-8,0,0))).ToList();
        }
        private List<WorkingDay> SettledDays
        {
            get => _allDays.Where(d => TimeSpan.Equals(d.Balance, new TimeSpan(0, 0, 0))).ToList();
        }
        private List<WorkingDay> _allDays ;

        public List<String> ResultText { get; private set; } = new List<string>();

        public SettlementManager(List<WorkingDay> days)
        {
            if (days != null)
            {
                _allDays = new List<WorkingDay>(days);
                _allDays = _allDays.OrderBy(d => Math.Abs(d.Balance.TotalMinutes)).ToList();
            }
            else
            {
                _allDays = new List<WorkingDay>();
            }
        }

        public void ExecuteSettlement()
        {
            if (_allDays == null || SettledDays.Count == _allDays.Count)
            {
                ResultText.Add("Wszytskie znalezione dni są poprawnie rozliczone.");
                return;
            }
            ResultText.Add(Logs.GetSubChapter("Wyjścia prywate"));
            foreach (var dayToSettle in DaysToSettle)
            {
                if (DaysWithOverhours.Count() == 0)
                {
                    break;
                }
                var dayWithOverHours = DaysWithOverhours.FirstOrDefault(d => d.Balance.TotalMinutes >= Math.Abs(dayToSettle.Balance.TotalMinutes));
                if (dayWithOverHours == null)
                {
                    settleSingleDayFromManyDays(dayToSettle);
                }
                else
                {
                    ResultText.Add(settle(dayWithOverHours, dayToSettle));
                }
            }
            var toRecieve = new TimeSpan(0, Convert.ToInt32(DaysWithOverhours.Sum(d => d.Balance.TotalMinutes)), 0);
            var toWorkOut = new TimeSpan(0, Convert.ToInt32(DaysToSettle.Sum(d => d.Balance.TotalMinutes)),0);

            ResultText.Add(Logs.GetSubChapter("Dni bez karty / urlop:"));
            ResultText.Add(String.Join(Environment.NewLine, DaysOutOfWork.Select(d => d.Date).ToList()));

            ResultText.Add(Logs.GetSubChapter($"Nadgodziny (łącznie {Math.Truncate(toRecieve.TotalHours)}h {toRecieve.TotalMinutes % 60}m):"));
            ResultText.Add(String.Join(Environment.NewLine, DaysWithOverhours.Select(d => d.GetDescriptionOfWorkingTime()).ToList()));

            ResultText.Add(Logs.GetSubChapter($"Do odrobienia: (łącznie {Math.Truncate(toWorkOut.TotalHours)}h {toWorkOut.TotalMinutes%60}m):"));
            ResultText.Add(String.Join(Environment.NewLine, DaysToSettle.Select(d => d.GetDescriptionOfWorkingTime()).ToList()));
        }

        private void settleSingleDayFromManyDays(WorkingDay dayToSettle)
        {
            var overDaysCnt = DaysWithOverhours.Count;
            if (overDaysCnt == 0)
            {
                return;
            }
            string textResult;
            var daysWithOverhours = new List<WorkingDay>();
            for (int i = 0; i < overDaysCnt; i++)
            {
                daysWithOverhours.Add(DaysWithOverhours[overDaysCnt-1-i]);
                if (daysWithOverhours.Select(d=>d.Balance).Sum(b=>b.TotalMinutes) > Math.Abs(dayToSettle.Balance.TotalMinutes))
                {
                    break;
                }
            }
            textResult = settle(daysWithOverhours[0], dayToSettle);
            for (int i = 1; i < daysWithOverhours.Count; i++)
            {
                textResult += settle(daysWithOverhours[i], dayToSettle, false);
            }
            ResultText.Add(textResult);
        }

        private string settle(WorkingDay overHourDay, WorkingDay dayToSettle, bool isDateOfOverHourDayInResult=true)
        {
            if (overHourDay.Balance.TotalMinutes<0 || dayToSettle.Balance.TotalMinutes>0)
            {
                throw new ArgumentException();
            }
            string result="";
            var diff = Math.Min(Math.Abs(dayToSettle.Balance.TotalMinutes), overHourDay.Balance.TotalMinutes);
            var diffTs = new TimeSpan(0, Convert.ToInt32(diff), 0);
            if (isDateOfOverHourDayInResult)
            {
                result =$"Wyjście prywatne {dayToSettle.Date} od {dayToSettle.Exit} do {dayToSettle.Exit - dayToSettle.Balance} " +
                    $"zostanie odpracowane w dniach:" + Environment.NewLine;
            }
            result +=$" {overHourDay.Date} od {overHourDay.Exit - diffTs} do {overHourDay.Exit}" + Environment.NewLine;
            overHourDay.AddBalanse(-diffTs);
            dayToSettle.AddBalanse(diffTs);

            return result;
        }
    }
}

