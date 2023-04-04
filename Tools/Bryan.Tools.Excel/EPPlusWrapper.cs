using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OfficeOpenXml;

namespace Helpers
{
    /// <summary>
    /// 单元格从1,1开始
    /// </summary>
    public class EPPlusWrapper : IDisposable
    {
        private ExcelPackage _excelPackage;

        public ExcelPackage ExcelPackage { get => _excelPackage; private set => _excelPackage = value; }
        public EPPlusWrapper(string filePath)
        {
            _excelPackage = new ExcelPackage(new FileInfo(filePath));
        }

        public ExcelWorksheet GetExcelWorksheet(string name)
        {
            return _excelPackage.Workbook.Worksheets[name];
        }

        public void Dispose()
        {
            _excelPackage?.Dispose();
        }
    }
}
