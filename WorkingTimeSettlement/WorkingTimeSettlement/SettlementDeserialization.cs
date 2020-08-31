using Npoi.Mapper;
using NPOI;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingTimeSettlement
{
    public class SettlementDeserialization
    {
        public string SettlementPath { get;set;}
        public int worksheetIndex {get;set;} = 0;
        public List<WorkingDayRecord> WorkingDays{ get; set; }
        public bool IsSerialized { get; private set; } = false;
        public List<WorkingDayRecord> DaysToSettle{
            get
            {
                var result = WorkingDays.Where(d => String.IsNullOrWhiteSpace(d.ToRecieve) == false || String.IsNullOrWhiteSpace(d.Balance) == false).ToList();
                return result;
            }
        }

        public SettlementDeserialization(string path)
        {
            if (Path.IsPathRooted(path))
                SettlementPath = path;
            else
                throw new Exception("Settlement Path is incorrect. You need to assign correct Settlement Path in SettlementDeserialization object during initialization");
        }

        internal void ExecuteDeserialization()
        {
            WorkingDays = new List<WorkingDayRecord>();

            IWorkbook workbook;
            try
            {
                using (FileStream file = new FileStream(SettlementPath, FileMode.Open, FileAccess.Read))
                {
                    workbook = WorkbookFactory.Create(file);
                }
            }
            catch (System.IO.IOException)
            {

                return;
            }
            
            var sheet = workbook.GetSheetAt(worksheetIndex);

            RemoveMergedCells(sheet);

            var importer = new Mapper(workbook);
            var items = importer.Take<WorkingDayRecord>(worksheetIndex);
            foreach (var item in items)
            {
                var currentRow = item.Value;
                if (string.IsNullOrEmpty(currentRow.Date))
                    continue;
                WorkingDays.Add(currentRow);
            }

            IsSerialized = true;
        }

        private static void RemoveMergedCells(ISheet sheet)
        {
            var countMergedRegions = sheet.NumMergedRegions;
            if (countMergedRegions == 0)
                return;

            for (int i = 0; i < 2; i++)
            {
                sheet.RemoveRow(sheet.GetRow(0));
                sheet.ShiftRows(0 + 1, sheet.LastRowNum, -1);
            }
        }
    }
}
