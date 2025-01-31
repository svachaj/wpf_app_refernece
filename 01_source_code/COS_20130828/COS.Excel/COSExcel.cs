using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COS.Excel
{
    public class COSExcel : IDisposable
    {
        public COSExcel()
        {
            ExcelApp = new Microsoft.Office.Interop.Excel.Application();

            WorkBook = ExcelApp.Workbooks.Add(misValue);
        }

        public COSExcel(string fileName)
        {
            ExcelApp = new Microsoft.Office.Interop.Excel.Application();

            WorkBook = ExcelApp.Workbooks.Open(fileName, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);

        }

        public COSExcel(byte[] fileData, string excelFileName)
        {
            ExcelApp = new Microsoft.Office.Interop.Excel.Application();

            using (FileStream fs = new FileStream(excelFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                fs.Write(fileData, 0, fileData.Length);
                fs.Close();
            }

            WorkBook = ExcelApp.Workbooks.Open(excelFileName, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);

        }

        object misValue = System.Reflection.Missing.Value;

        public void LoadExcelFile(string fileName)
        {
            WorkBook = ExcelApp.Workbooks.Open(fileName, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);

        }

        public object GetData(int workSheet, int rowIndex, int columnIndex)
        {
            object result = null;

            if (WorkBook != null)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet ws = WorkBook.Worksheets[workSheet];

                    result = ws.Cells[rowIndex, columnIndex].Value;
                }
                catch
                {
                    result = null;
                }
            }

            return result;
        }

        public string GetDataText(int workSheet, int rowIndex, int columnIndex)
        {
            string result = null;

            if (WorkBook != null)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet ws = WorkBook.Worksheets[workSheet];

                    result = ws.Cells[rowIndex, columnIndex].Text;
                }
                catch
                {
                    result = null;
                }
            }

            return result;
        }

        public void SetData(int workSheet, int rowIndex, int columnIndex, object value)
        {
            if (WorkBook != null)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet ws = WorkBook.Worksheets[workSheet];

                    ws.Cells[rowIndex, columnIndex].Value = value;
                }
                catch
                {
                    //nic se nenastaví...
                }
            }
        }

        public void SetData(int workSheet, int rowIndex, int columnIndex, object value, System.Drawing.Color color)
        {
            if (WorkBook != null)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet ws = WorkBook.Worksheets[workSheet];

                    ws.Cells[rowIndex, columnIndex].Value = value;
                    ws.Cells[rowIndex, columnIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
                }
                catch
                {
                    //nic se nenastaví...
                }
            }
        }

        public void SetData(int workSheet, int rowIndex, int columnIndex, object value, Tuple<int, int> rowColCopyAll)
        {
            if (WorkBook != null)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet ws = WorkBook.Worksheets[workSheet];

                    if (rowColCopyAll != null)
                    {
                        ws.Cells[rowColCopyAll.Item1, rowColCopyAll.Item2].Copy(Type.Missing);
                        ws.Cells[rowIndex, columnIndex].PasteSpecial(Microsoft.Office.Interop.Excel.XlPasteType.xlPasteAll, Microsoft.Office.Interop.Excel.XlPasteSpecialOperation.xlPasteSpecialOperationNone, false, false);
                    }
                    ws.Cells[rowIndex, columnIndex].Value = value;
                }
                catch
                {
                    //nic se nenastaví...
                }
            }
        }


        public void HighliteData(int workSheet, int rowIndex)
        {
            if (WorkBook != null)
            {
                try
                {
                    Microsoft.Office.Interop.Excel.Worksheet ws = WorkBook.Worksheets[workSheet];

                    ws.Rows[rowIndex].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

                }
                catch
                {
                    //nic se nenastaví...
                }
            }
        }


        public void Save(string filename)
        {
            try
            {
                WorkBook.SaveAs(filename, misValue, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            }
            catch { }
        }

        public Microsoft.Office.Interop.Excel.Application ExcelApp { set; get; }

        public Microsoft.Office.Interop.Excel.Workbook WorkBook { set; get; }

        public void Dispose()
        {
            if (WorkBook != null)
                WorkBook.Close(false, misValue, misValue);

            if (ExcelApp != null)
                ExcelApp = null;

            GC.Collect();
        }
    }
}
