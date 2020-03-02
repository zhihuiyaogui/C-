using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace GenerSoft.OpenTSDB.Client
{
    public class testclass
    {
        public static string opentsdburl = "http://172.22.15.132:10042";
        private string fileName = null; //文件名
        public static IWorkbook workbook = null;
        public static FileStream fs = null;
        private bool disposed;

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public static void ExcelToDataTable(bool isFirstRowColumn)
        {
            ISheet sheet = null;
            string fileName = @"C:\Users\libo_lc\Desktop\111.xlsx";
            OpentsdbClient client = new OpentsdbClient(opentsdburl);
            try
            {
                Dictionary<string, string> tagMap = new Dictionary<string, string>();
                tagMap.Add("yhgateway", "yhgateway1");
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                {
                    workbook = new HSSFWorkbook(fs);
                }
                
                sheet = workbook.GetSheetAt(0);//尝试获取第一个sheet
                IDataFormat dataformat = workbook.CreateDataFormat();
                ICellStyle style = workbook.CreateCellStyle();
                style.DataFormat = dataformat.GetFormat("yyyyMMdd HH:mm:ss");
                if (sheet != null)
                {
                    IRow row;// = sheet.GetRow(0);            //新建当前工作表行数据  
                    IRow row1 = sheet.GetRow(0);//当前工作表第一行数据
                    for (int i = 1; i < sheet.LastRowNum; i++)  //对工作表每一行  
                    {
                        row = sheet.GetRow(i);   //row读入第i行数据
                        if (row != null)
                        {
                            for (int j = 1; j < row.LastCellNum; j++)  //对工作表每一列  
                            {
                                string cellValue = row.GetCell(j).ToString(); //获取i行j列数据
                                row.GetCell(0).CellStyle = style;
                                var datatime = Convert.ToDateTime(row.GetCell(0).DateCellValue).ToString("yyyyMMdd HH:mm:ss");//获取第一列时间数据
                                Console.WriteLine(cellValue);
                                client.putData(row1.GetCell(j).ToString(), DateTimeUtil.parse(datatime, "yyyyMMdd HH:mm:ss"), cellValue, tagMap);
                            }
                            //列名、第一列、数据
                        }
                    }
                    Console.ReadLine();
                    fs.Close();
                    workbook.Close();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
        }


        public static void TestExcelRead(string file)
        {
            try
            {
                ExcelToDataTable(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }

    class ExcelHelper
    {
        static void Main(string[] args)
        {
            testclass.ExcelToDataTable(true);
        }
        static void PrintData(DataTable data)
        {
            if (data == null) return;
            for (int i = 0; i < data.Rows.Count; ++i)
            {
                for (int j = 0; j < data.Columns.Count; ++j)
                    Console.Write("{0} ", data.Rows[i][j]);
                Console.Write("\n");
            }
        }
    }
}
