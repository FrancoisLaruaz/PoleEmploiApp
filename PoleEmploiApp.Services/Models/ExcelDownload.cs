using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace PoleEmploiApp.Services.Models
{
    public class ExcelDownload
    {
        private Dictionary<string, List<string>> columnList = new Dictionary<string, List<string>>();
        private Dictionary<string, int> rowCounts = new Dictionary<string, int>();
        private string currentSheetName;

        public ExcelDownload()
        {
            this.Package = new ExcelPackage();
        }

        public bool AddWorksheet(string name)
        {
            try
            {
                this.Worksheet = Package.Workbook.Worksheets.Add(name);
                currentSheetName = name;
                columnList.Add(name, new List<string>());
                RowCount = 1;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool WritePlainText(string cell, string text, bool bold = false)
        {
            try
            {
                this.Worksheet.Cells[cell].Value = text;
                this.Worksheet.Cells[cell].Style.Font.Bold = bold;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SwitchSheetTo(string name)
        {
            currentSheetName = name;
            this.Worksheet = Package.Workbook.Worksheets[name];
            return true;
        }

        /// <summary>
        /// This function ONLY works with ONE spreadsheet, do not create TWO
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetValue(string fieldName, object value, int firstRowCount = 1, bool IsNumeric = false)
        {

                var _columns = columnList[currentSheetName];
                bool columnExists = _columns.Contains(fieldName.Trim());
                int columnNumber;
                if (columnExists)
                {
                    columnNumber = _columns.IndexOf(fieldName) + 1;
                }
                else
                {
                    _columns.Add(fieldName.Trim());
                    columnNumber = _columns.Count;
                    Worksheet.Cells[Number2String(columnNumber, true) + firstRowCount.ToString()].Style.Font.Bold = true;
                    Worksheet.Cells[Number2String(columnNumber, true) + firstRowCount.ToString()].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#31afb4");
                    Worksheet.Cells[Number2String(columnNumber, true) + firstRowCount.ToString()].Style.Fill.BackgroundColor.SetColor(color);
                    Worksheet.Cells[Number2String(columnNumber, true) + firstRowCount.ToString()].Style.Font.Color.SetColor(System.Drawing.Color.White);
                    Worksheet.Cells[Number2String(columnNumber, true) + firstRowCount.ToString()].Value = fieldName.Trim();
                }

                Worksheet.Cells[Number2String(columnNumber, true) + RowCount.ToString()].Value = value==null?"": value.ToString();
                if (IsNumeric)
                {
                    //   Worksheet.Cells[Number2String(columnNumber, true) + RowCount.ToString()].Style.Numberformat.Format = "0.00";
                }

            return true;
        }

        public bool SetHyperlink(string fieldName, string value)
        {

                var _columns = columnList[currentSheetName];
                bool columnExists = _columns.Contains(fieldName.Trim());
                int columnNumber;
                if (columnExists)
                {
                    columnNumber = _columns.IndexOf(fieldName) + 1;
                }
                else
                {
                    _columns.Add(fieldName.Trim());
                    columnNumber = _columns.Count;
                    Worksheet.Cells[Number2String(columnNumber, true) + "1"].Style.Font.Bold = true;
                    Worksheet.Cells[Number2String(columnNumber, true) + "1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    Worksheet.Cells[Number2String(columnNumber, true) + "1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                    Worksheet.Cells[Number2String(columnNumber, true) + "1"].Style.Font.Color.SetColor(System.Drawing.Color.Black);
                    Worksheet.Cells[Number2String(columnNumber, true) + "1"].Value = fieldName.Trim();
                }

                Worksheet.Cells[Number2String(columnNumber, true) + RowCount.ToString()].Formula = value;
                Worksheet.Cells[Number2String(columnNumber, true) + RowCount.ToString()].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                Worksheet.Cells[Number2String(columnNumber, true) + RowCount.ToString()].Style.Font.UnderLine = true;

            return true;
        }

        public int RowCount
        {
            get
            {
                return rowCounts[currentSheetName];
            }
            set
            {
                rowCounts[currentSheetName] = value;
            }
        }

        public int ColumnCount { get { return columnList[currentSheetName].Count; } }

        public ExcelPackage Package { get; set; }
        public ExcelWorksheet Worksheet { get; set; }

        private static String Number2String(int number, bool isCaps)
        {

                string ColumnBase = "";
                if (number >= 79)
                {
                    ColumnBase = "C";
                    number = number - 78;
                }
                else if (number >= 53)
                {
                    ColumnBase = "B";
                    number = number - 52;
                }
                else if (number >= 27)
                {
                    ColumnBase = "A";
                    number = number - 26;
                }

                Char c = (Char)((isCaps ? 65 : 97) + (number - 1));

                return ColumnBase + c.ToString();
  
        }

        public bool AutoFitColumns(string startCell = "A1")
        {

                if (ColumnCount > 0 && RowCount > 1)
                {
                    string range = startCell + ":" + Number2String(ColumnCount, true) + (RowCount).ToString();
                    var modelTable = Worksheet.Cells[range];
                    modelTable.AutoFitColumns();

                    modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                return true;

            return false;
        }

        public bool AutoFilter()
        {

            if (ColumnCount > 0)
            {
                Worksheet.Cells["A1:" + Number2String(ColumnCount, true) + "1"].AutoFilter = true;
                return true;
            }

            return false;
        }

        public byte[] FileBytes
        {
            get
            {
   
                    byte[] result = null;
                    using (var fileStream = new MemoryStream())
                    {
                        this.Package.SaveAs(fileStream);
                        fileStream.Position = 0;
                        result = fileStream.ToArray();
                    }
                    return result;

            }
        }
    }
}
