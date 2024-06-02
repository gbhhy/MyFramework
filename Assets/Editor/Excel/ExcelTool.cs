using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public partial class ExcelTool
{
    /// <summary>
    /// Excel文件存放路径
    /// </summary>
    public static string ExcelPath = Application.dataPath + "/ExcelTable/";


    [MenuItem("GameTool/GenerateExcel")]
    private static void GenerateExcelInfo()
    {
        DirectoryInfo dInfo = Directory.CreateDirectory(ExcelPath);
        //得到指定路径中的所有文件信息，即Excel表
        FileInfo[] files = dInfo.GetFiles();
        //数据表容器
        DataTableCollection tableCollection;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension != ".xlsx" &&
                files[i].Extension != ".xls")
                continue;
            //打开Excel文件读表
            using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = reader.AsDataSet().Tables;
                fs.Close();
            }
            //遍历文件中所有表的信息
            foreach (DataTable table in tableCollection)
            {
                //生成数据结构类
                GenerateExcelDataClass(table);
                //生成容器类
                GenerateExcelContainer(table);
                //生成二进制
                GenerateExcelBinary(table);
            }
        }
    }


}

/// <summary>
/// 具体实现部分
/// </summary>
public partial class ExcelTool
{
    /// <summary>
    /// 数据结构类脚本存放路径
    /// </summary>
    public static string DataClassPath = Application.dataPath + "/Scripts/ExcelData/DataClass/";
    /// <summary>
    /// 容器类脚本存储路径
    /// </summary>
    public static string DataContainerPath = Application.dataPath + "/Scripts/ExcelData/Container/";


    /// <summary>
    /// 生成Excel表对应的数据结构类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelDataClass(DataTable table)
    {
        DataRow nameRow = GetFieldNameRow(table);
        DataRow typeRow = GetFieldTypeRow(table);
        //创建文件存放路径
        if (!Directory.Exists(DataClassPath))
        {
            Directory.CreateDirectory(DataClassPath);
        }
        //字符串拼接
        string str = "public class " + table.TableName + "\n{\n";
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += "  public " + typeRow[i].ToString() + " " + nameRow[i].ToString() + ";\n";
        }
        str += "}";
        //存入指定文件
        File.WriteAllText(DataClassPath + table.TableName + ".cs", str);
        //刷新project窗口
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 得到变量名所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetFieldNameRow(DataTable table)
    {
        return table.Rows[0];
    }
    /// <summary>
    /// 得到变量类型所在行
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetFieldTypeRow(DataTable table)
    {
        return table.Rows[1];
    }
    /// <summary>
    /// 生成Excel表对应的数据容器类
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelContainer(DataTable table)
    {
        int keyIndex = GetKeyIndex(table);
        DataRow typeRow = GetFieldTypeRow(table);

        if (!Directory.Exists(DataContainerPath))
        {
            Directory.CreateDirectory(DataContainerPath);
        }

        string str = "using System.Collections.Generic;\n" + "public class " + table.TableName + "Container\n{\n";
        str += "  public Dictionary<" + typeRow[keyIndex].ToString() + ", " + table.TableName + "> dataDic = new Dictionary<";
        str += typeRow[keyIndex].ToString() + ", " + table.TableName + ">();\n}";

        File.WriteAllText(DataContainerPath + table.TableName + "Container.cs", str);
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// 获取主键索引
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static int GetKeyIndex(DataTable table)
    {
        DataRow row = table.Rows[2];
        for (int i = 0; i < table.Columns.Count; i++)
        {
            if (row[i].ToString() == "key")
                return i;
        }
        return 0;
    }
    /// <summary>
    /// 生成Excel二进制数据
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelBinary(DataTable table)
    {
        if (!Directory.Exists(BinaryDataManager.DataBinaryPath))
            Directory.CreateDirectory(BinaryDataManager.DataBinaryPath);
        //创建一个二进制文件并写入
        using (FileStream fs = new FileStream(BinaryDataManager.DataBinaryPath + table.TableName + ".hhy", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //1.存储需要的行数据，方便读取
            //-4是因为前4行是配置规则，不需要记录
            fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
            //2.存储主键的变量名
            string keyName = GetFieldNameRow(table)[GetKeyIndex(table)].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(keyName);
            //存储字符串字节数组的长度
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            //存储字符串字节数组
            fs.Write(bytes, 0, bytes.Length);
            //遍历所有行，进行二进制写入
            DataRow row;
            DataRow typeRow = GetFieldTypeRow(table);
            for (int i = 4; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    switch (typeRow[j].ToString())
                    {
                        case "int":
                            fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())),0,4);
                            break;
                        case "float":
                            fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                            break;
                        case "bool":
                            fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                            break;
                        case "string":
                            bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                            //写入字符串字节数组的长度
                            fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
                            //写入字符串字节数组
                            fs.Write(bytes, 0, bytes.Length);
                            break;

                    }
                }
            }
            fs.Close();
        }
        AssetDatabase.Refresh();
    }
}