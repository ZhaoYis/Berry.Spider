using NPOI.SS.UserModel;
using System.Data;

namespace Berry.Spider.Core;

public static class OfficeHelper
{
    /// <summary>
    /// 将excel文件内容读取到DataTable数据表中
    /// </summary>
    /// <param name="fileName">文件完整路径名</param>
    /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
    /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
    /// <returns>DataTable数据表</returns>
    public static DataTable? ReadExcelToDataTable(string fileName, string sheetName = "", bool isFirstRowColumn = true)
    {
        if (!File.Exists(fileName)) return null;

        //根据指定路径读取文件
        FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        return ReadStreamToDataTable(fs, sheetName, isFirstRowColumn);
    }

    /// <summary>
    /// 将文件流读取到DataTable数据表中
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
    /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
    /// <returns>DataTable数据表</returns>
    public static DataTable? ReadStreamToDataTable(Stream fileStream, string sheetName = "", bool isFirstRowColumn = true)
    {
        DataTable? data = new DataTable();
        try
        {
            //excel工作表
            ISheet sheet;
            //根据文件流创建excel数据结构,NPOI的工厂类WorkbookFactory会自动识别excel版本，创建出不同的excel数据结构
            IWorkbook workbook = WorkbookFactory.Create(fileStream);
            //如果有指定工作表名称
            if (!string.IsNullOrEmpty(sheetName))
            {
                //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                sheet = workbook.GetSheet(sheetName) ?? workbook.GetSheetAt(0);
            }
            else
            {
                //如果没有指定的sheetName，则尝试获取第一个sheet
                sheet = workbook.GetSheetAt(0);
            }

            if (sheet != null)
            {
                //数据开始行(排除标题行)
                int startRow = 0;
                IRow firstRow = sheet.GetRow(0);
                //一行最后一个cell的编号 即总的列数
                int cellCount = firstRow.LastCellNum;
                //如果第一行是标题列名
                if (isFirstRowColumn)
                {
                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                    {
                        ICell cell = firstRow.GetCell(i);
                        if (cell != null)
                        {
                            string cellValue = cell.StringCellValue;
                            if (cellValue != null)
                            {
                                DataColumn column = new DataColumn(cellValue);
                                data.Columns.Add(column);
                            }
                        }
                    }

                    startRow = sheet.FirstRowNum + 1;
                }
                else
                {
                    startRow = sheet.FirstRowNum;
                }

                //最后一列的标号
                int rowCount = sheet.LastRowNum;
                for (int i = startRow; i <= rowCount; ++i)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || row.FirstCellNum < 0) continue; //没有数据的行默认是null　　　　　　　

                    DataRow dataRow = data.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                    {
                        //同理，没有数据的单元格都默认是null
                        ICell cell = row.GetCell(j);
                        if (cell != null)
                        {
                            if (cell.CellType == CellType.Numeric)
                            {
                                //判断是否日期类型
                                if (DateUtil.IsCellDateFormatted(cell))
                                {
                                    dataRow[j] = row.GetCell(j).DateCellValue;
                                }
                                else
                                {
                                    dataRow[j] = row.GetCell(j).ToString()?.Trim();
                                }
                            }
                            else
                            {
                                dataRow[j] = row.GetCell(j).ToString()?.Trim();
                            }
                        }
                    }

                    data.Rows.Add(dataRow);
                }
            }

            return data;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}