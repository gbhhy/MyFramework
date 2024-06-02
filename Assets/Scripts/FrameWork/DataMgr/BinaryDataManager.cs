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
    /// �洢���������
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
    /// ��ȡ����������ת��Ϊ�����
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
/// Excel�����
/// </summary>
public partial class BinaryDataManager : BaseManager<BinaryDataManager>
{
    //�洢����Excel�������
    private Dictionary<string, object> tableDic = new Dictionary<string, object>();

    /// <summary>
    /// 2�������ݴ洢λ��·��
    /// </summary>
    public static string DataBinaryPath = Application.streamingAssetsPath + "/Binary/";

    /// <summary>
    /// ��ʼ�����ݣ���Ҫ��������ʱ�ڴ˴����
    /// </summary>
    public void InitData()
    {
        LoadTable<referenceTableContainer, referenceTable>();
    }

    /// <summary>
    /// ����Excel��Ķ��������ݵ��ڴ���
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <typeparam name="K">���ݽṹ������</typeparam>
    public void LoadTable<T, K>()
    {
        //��ȡ�������ļ����н�������ӦExcelTool�е�GenerateExcelBinary����
        using (FileStream fs = File.Open(DataBinaryPath + typeof(K).Name + ".hhy", FileMode.Open, FileAccess.Read))
        {
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            //��¼��ǰ��ȡ�����ֽ�
            int index = 0;
            //��ȡ����������
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            //��ȡ����������
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;
            //�������������
            Type containerType = typeof(T);
            object containerObj = Activator.CreateInstance(containerType);
            //�õ����ݽṹ���Type
            Type classType = typeof(K);
            //�õ�K�������ֶ���Ϣ
            FieldInfo[] infos = classType.GetFields();
            //��ȡÿһ�е���Ϣ
            for (int i = 0; i < count; i++)
            {
                object dataObj = Activator.CreateInstance(classType);
                foreach (FieldInfo fi in infos)
                {
                    //switch������typeof
                    if (fi.FieldType == typeof(int))
                    {
                        //�Ѷ���������ת��Ϊint����ֵ����Ӧ�ֶ�
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
                //��ȡ��һ�����ݣ���������ӵ�����������
                object dicObject = containerType.GetField("dataDic").GetValue(containerObj);
                MethodInfo mInfo = dicObject.GetType().GetMethod("Add");
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                mInfo.Invoke(dicObject, new object[] { keyValue, dataObj });
            }
            //��¼��ȡ��ı�
            tableDic.Add(typeof(T).Name, containerObj);
            fs.Close();
        }
    }

    /// <summary>
    /// �õ�һ�ű����Ϣ
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <returns></returns>
    public T GetTable<T>() where T : class
    {
        string tableName = typeof(T).Name;
        if (tableDic.ContainsKey(tableName))
            return tableDic[tableName] as T;
        return null;
    }
}
