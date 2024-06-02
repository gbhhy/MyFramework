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
    /// Excel�ļ����·��
    /// </summary>
    public static string ExcelPath = Application.dataPath + "/ExcelTable/";


    [MenuItem("GameTool/GenerateExcel")]
    private static void GenerateExcelInfo()
    {
        DirectoryInfo dInfo = Directory.CreateDirectory(ExcelPath);
        //�õ�ָ��·���е������ļ���Ϣ����Excel��
        FileInfo[] files = dInfo.GetFiles();
        //���ݱ�����
        DataTableCollection tableCollection;
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Extension != ".xlsx" &&
                files[i].Extension != ".xls")
                continue;
            //��Excel�ļ�����
            using (FileStream fs = files[i].Open(FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                tableCollection = reader.AsDataSet().Tables;
                fs.Close();
            }
            //�����ļ������б����Ϣ
            foreach (DataTable table in tableCollection)
            {
                //�������ݽṹ��
                GenerateExcelDataClass(table);
                //����������
                GenerateExcelContainer(table);
                //���ɶ�����
                GenerateExcelBinary(table);
            }
        }
    }


}

/// <summary>
/// ����ʵ�ֲ���
/// </summary>
public partial class ExcelTool
{
    /// <summary>
    /// ���ݽṹ��ű����·��
    /// </summary>
    public static string DataClassPath = Application.dataPath + "/Scripts/ExcelData/DataClass/";
    /// <summary>
    /// ������ű��洢·��
    /// </summary>
    public static string DataContainerPath = Application.dataPath + "/Scripts/ExcelData/Container/";


    /// <summary>
    /// ����Excel���Ӧ�����ݽṹ��
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelDataClass(DataTable table)
    {
        DataRow nameRow = GetFieldNameRow(table);
        DataRow typeRow = GetFieldTypeRow(table);
        //�����ļ����·��
        if (!Directory.Exists(DataClassPath))
        {
            Directory.CreateDirectory(DataClassPath);
        }
        //�ַ���ƴ��
        string str = "public class " + table.TableName + "\n{\n";
        for (int i = 0; i < table.Columns.Count; i++)
        {
            str += "  public " + typeRow[i].ToString() + " " + nameRow[i].ToString() + ";\n";
        }
        str += "}";
        //����ָ���ļ�
        File.WriteAllText(DataClassPath + table.TableName + ".cs", str);
        //ˢ��project����
        AssetDatabase.Refresh();
    }
    /// <summary>
    /// �õ�������������
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetFieldNameRow(DataTable table)
    {
        return table.Rows[0];
    }
    /// <summary>
    /// �õ���������������
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    private static DataRow GetFieldTypeRow(DataTable table)
    {
        return table.Rows[1];
    }
    /// <summary>
    /// ����Excel���Ӧ������������
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
    /// ��ȡ��������
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
    /// ����Excel����������
    /// </summary>
    /// <param name="table"></param>
    private static void GenerateExcelBinary(DataTable table)
    {
        if (!Directory.Exists(BinaryDataManager.DataBinaryPath))
            Directory.CreateDirectory(BinaryDataManager.DataBinaryPath);
        //����һ���������ļ���д��
        using (FileStream fs = new FileStream(BinaryDataManager.DataBinaryPath + table.TableName + ".hhy", FileMode.OpenOrCreate, FileAccess.Write))
        {
            //1.�洢��Ҫ�������ݣ������ȡ
            //-4����Ϊǰ4�������ù��򣬲���Ҫ��¼
            fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
            //2.�洢�����ı�����
            string keyName = GetFieldNameRow(table)[GetKeyIndex(table)].ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(keyName);
            //�洢�ַ����ֽ�����ĳ���
            fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
            //�洢�ַ����ֽ�����
            fs.Write(bytes, 0, bytes.Length);
            //���������У����ж�����д��
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
                            //д���ַ����ֽ�����ĳ���
                            fs.Write(BitConverter.GetBytes(bytes.Length),0,4);
                            //д���ַ����ֽ�����
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