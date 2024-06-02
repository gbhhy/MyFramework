using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public partial class BinaryDataManager : BaseManager<BinaryDataManager>
{
    private static string SavePath = Application.persistentDataPath + "/Data/";

    private BinaryDataManager()
    {
        InitData();
    }

    /// <summary>
    /// 存储类对象数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="fileName"></param>
    public void Save(object data, string fileName)
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);
        using (FileStream fs = new FileStream(SavePath + fileName + ".hhy", FileMode.OpenOrCreate, FileAccess.Write))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, data);
            fs.Close();
        }
    }
    /// <summary>
    /// 读取二进制数据转化为类对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public T Load<T>(string fileName) where T : class
    {
        if (!File.Exists(SavePath + fileName + ".hhy"))
            return default(T);

        T data;
        using (FileStream fs = File.Open(SavePath + fileName + ".hhy", FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            data = bf.Deserialize(fs) as T;
            fs.Close();
        }
        return data;
    }
}

/// <summary>
/// Excel表相关
/// </summary>
public partial class BinaryDataManager : BaseManager<BinaryDataManager>
{
    //存储所有Excel表的容器
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();

    /// <summary>
    /// 2进制数据存储位置路径
    /// </summary>
    public static string DataBinaryPath = Application.streamingAssetsPath + "/Binary/";

    /// <summary>
    /// 初始化数据，需要加载数据时在此处添加
    /// </summary>
    public void InitData()
    {
        LoadTable<referenceTableContainer, referenceTable>();
    }

    /// <summary>
    /// 加载Excel表的二进制数据到内存中
    /// </summary>
    /// <typeparam name="T">容器类名</typeparam>
    /// <typeparam name="K">数据结构类类名</typeparam>
    public void LoadTable<T, K>()
    {
        //读取二进制文件进行解析，对应ExcelTool中的GenerateExcelBinary方法
        using (FileStream fs = File.Open(DataBinaryPath + typeof(K).Name + ".hhy", FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            //记录当前读取多少字节
            int index = 0;
            //读取多少行数据
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            //读取主键的名字
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;
            //创建容器类对象
            Type containerType = typeof(T);
            object containerObj = Activator.CreateInstance(containerType);
            //得到数据结构类的Type
            Type classType = typeof(K);
            //得到K的所有字段信息
            FieldInfo[] infos = classType.GetFields();
            //读取每一行的信息
            for (int i = 0; i < count; i++)
            {
                object dataObj = Activator.CreateInstance(classType);
                foreach (FieldInfo fi in infos)
                {
                    //switch不能用typeof
                    if (fi.FieldType == typeof(int))
                    {
                        //把二进制数据转化为int，赋值给对应字段
                        fi.SetValue(dataObj, BitConverter.ToInt32(bytes, index));
                        index += 4;
                    }
                    else if (fi.FieldType == typeof(string))
                    {
                        int length = BitConverter.ToInt32(bytes, index);
                        index += 4;
                        fi.SetValue(dataObj, Encoding.UTF8.GetString(bytes, index, length));
                        index += length;
                    }
                    else if (fi.FieldType == typeof(float))
                    {
                        fi.SetValue(dataObj, BitConverter.ToSingle(bytes, index));
                        index += 4;
                    }
                    else if (fi.FieldType == typeof(bool))
                    {
                        fi.SetValue(dataObj, BitConverter.ToBoolean(bytes, index));
                        index += 1;
                    }
                }
                //读取完一行数据，将数据添加到容器对象中
                object dicObject = containerType.GetField("dataDic").GetValue(containerObj);
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObj });
            }
            //记录读取完的表
            tableDic.Add(typeof(T).Name, containerObj);
            fs.Close();
        }
    }

    /// <summary>
    /// 得到一张表的信息
    /// </summary>
    /// <typeparam name="T">容器类名</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (tableDic.ContainsKey(tableName))
            return tableDic[tableName] as T;
        return null;
    }
}
