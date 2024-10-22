using System.ComponentModel;
using NPOI.SS.UserModel;
using System.Data;
using System.Reflection;
using NPOI.XSSF.UserModel;

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
    public static DataTable? ReadFromExcel(string fileName, string sheetName = "", bool isFirstRowColumn = true)
    {
        if (!File.Exists(fileName)) return null;

        //根据指定路径读取文件
        using FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        return ReadStreamToDataTable(fs, sheetName, isFirstRowColumn);
    }

    /// <summary>
    /// 传入固定格式的数据，生成Excel workbook再写入到基于内存的流里边去
    /// </summary>
    /// <returns></returns>
    public static void WriteToExcel(List<ExcelDataResource> dataResources, string saveToFileName)
    {
        IWorkbook workbook = DataToHSSFWorkbook(dataResources);
        using FileStream fs = new FileStream(saveToFileName, FileMode.Create, FileAccess.Write);
        workbook.Write(fs);
    }

    /// <summary>
    /// 将文件流读取到DataTable数据表中
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="sheetName">指定读取excel工作薄sheet的名称</param>
    /// <param name="isFirstRowColumn">第一行是否是DataTable的列名：true=是，false=否</param>
    /// <returns>DataTable数据表</returns>
    private static DataTable ReadStreamToDataTable(Stream fileStream, string sheetName = "", bool isFirstRowColumn = true)
    {
        DataTable data = new DataTable();
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

    /// <summary>
    /// 给定固定格式的数据，可以生成Excel
    /// </summary>
    /// <returns></returns>
    private static IWorkbook DataToHSSFWorkbook(List<ExcelDataResource> dataResources)
    {
        XSSFWorkbook workbook = new XSSFWorkbook();
        //每循环一次就生成一个Sheet页出来
        foreach (var sheetResource in dataResources)
        {
            if (sheetResource.Rows is { Count: 0 })
            {
                break;
            }

            //创建一个页签
            ISheet sheet = workbook.CreateSheet(sheetResource.SheetName);
            //确定当前这一页有多少列---取决保存当前Sheet页数据的实体属性中的标记的特性
            object obj = sheetResource.Rows[0];

            //获取需要导出的所有的列
            Type type = obj.GetType();
            List<PropertyInfo> propList = type.GetProperties().Where(c => c.IsDefined(typeof(DescriptionAttribute), true)).ToList();

            //确定表头在哪一行生成
            int rownum = 0;
            if (sheetResource.RowNum >= 0)
            {
                rownum = sheetResource.RowNum - 1;
            }

            //基于当前的这个Sheet创建表头
            IRow titleRow = sheet.CreateRow(rownum);

            ICellStyle style = workbook.CreateCellStyle();
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            style.FillPattern = FillPattern.SolidForeground;
            style.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Automatic.Index;

            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Center;
            titleRow.Height = 100 * 4;
            //给表头行，分别创建单元格，并赋值字段的名称
            for (int i = 0; i < propList.Count; i++)
            {
                DescriptionAttribute? propertyAttribute = propList[i].GetCustomAttribute<DescriptionAttribute>();
                ICell cell = titleRow.CreateCell(i);
                cell.SetCellValue(propertyAttribute?.Description);
                cell.CellStyle = style;
            }

            //去生成数据
            for (int i = 0; i < sheetResource.Rows.Count; i++)
            {
                IRow row = sheet.CreateRow(i + rownum + 1);
                object objInstance = sheetResource.Rows[i];
                for (int j = 0; j < propList.Count(); j++)
                {
                    ICell cell = row.CreateCell(j);
                    cell.SetCellValue(propList[j].GetValue(objInstance)?.ToString());
                }
            }
        }

        return workbook;
    }
}

public class ExcelDataResource
{
    public string SheetName { get; set; }
    public int RowNum { get; set; }
    public List<ExcelDataRow> Rows { get; set; }
}

public class ExcelDataRow
{
    [Description("Title")]
    public string Title { get; set; }
    
    [Description("Content")]
    public string Content { get; set; }
}