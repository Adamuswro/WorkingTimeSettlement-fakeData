using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkingTimeSettlement
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            AdjustConsoleWIndowsSize();
            SettlementDeserialization deserializator;
            try
            {
                deserializator = new SettlementDeserialization(GetPathToSettlement());
            }
            catch
            {
                Console.WriteLine("File not found. Press any key to exit.");
                Console.ReadLine();
                return;
            }

            var spinner = new Spinner("Processing file...",0, 0,75);
            spinner.Start();

            #region Loop to read the file. If there is error user is asked for selection again.
            do
            {
                deserializator.ExecuteDeserialization();
                if (deserializator.IsSerialized == false)
                {
                    Console.WriteLine($"Error during file reading. " + Environment.NewLine +
                        $"Make sure if selected file '{Path.GetFileName(deserializator.SettlementPath)}' is closed."
                        + Environment.NewLine + "Push enter to try again.");
                    Console.ReadLine();
                }
            } while (deserializator.IsSerialized == false);
            #endregion

            var beforeSettlement = WorkingDayFactory.GetWorkingDays(deserializator.DaysToSettle);
            var beforeSettlementText = String.Join(Environment.NewLine, beforeSettlement.Select(s => s.GetDescriptionOfWorkingTime()).ToList());

            var daysSM = new SettlementManager(beforeSettlement);
            daysSM.ExecuteSettlement();
            //This thread sleep is used only to admire spinner feature :) 
            Thread.Sleep(2000);
            spinner.Stop();

            Console.WriteLine($"{Logs.GetChapter("DO ROZLICZENIA")}");
            Console.WriteLine(beforeSettlementText);
            Console.WriteLine($"{Logs.GetChapter("ROZLICZENIE")}");
            Console.WriteLine(String.Join("",daysSM.ResultText));
            Console.ReadLine();
        }

        private static void AdjustConsoleWIndowsSize()
        {
            Console.WindowHeight = Console.LargestWindowHeight;
        }

        static string GetPathToSettlement()
        {
            string result = null;

            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
            fd.FilterIndex = 1;
            fd.Multiselect = false;
            fd.InitialDirectory = GetInput_FilesFolderPath();

            if(fd.ShowDialog() == DialogResult.OK)
            {
                result = fd.FileName;
            }

            return result;
        }

        private static string GetInput_FilesFolderPath()
        {
            var result = Path.Combine(GetProjectDirectory(), "Input_Files");
            return result;
        }
        private static string GetProjectDirectory()
        {
            var applicationPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var result = Path.GetFullPath(Path.Combine(applicationPath, "..", ".."));
            return result;
        }

    }
}
