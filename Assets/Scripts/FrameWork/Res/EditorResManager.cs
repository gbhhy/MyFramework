using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/// <summary>
/// �༭����Դ������
/// ע�⣺ֻ���ڿ���ʱ��ʹ�øù�����������Դ�����ڿ�������
/// �������޷�ʹ�øù���������Ϊ����Ҫʹ�ñ༭����ع���
/// </summary>
public class EditorResManager : BaseManager<EditorResManager>
{
    private string rootPath = "Assets/Editor/ArtRes/";
    private EditorResManager() { }

    //���ص�����Դ
    public T LoadEditorRes<T>(string path)where T:Object
    {
#if UNITY_EDITOR
        string suffix = "";
        //Ԥ���塢����ͼƬ������������Ч�ȵ�
        if (typeof(T) == typeof(GameObject))
            suffix = ".prefab";
        else if (typeof(T) == typeof(Material))
            suffix = ".mat";
        else if (typeof(T) == typeof(Texture))
            suffix = ".png";
        else if (typeof(T) == typeof(AudioClip))
            suffix = ".mp3";
        T res=AssetDatabase.LoadAssetAtPath<T>(rootPath+path+suffix);
        return res;
#else
        return null;
#endif
    }
    //����ͼ�������Դ
    public Sprite LoadSprite(string path,string spriteName)
    {
#if UNITY_EDITOR
        //����ͼ���е���������Դ
        Object[] sprites=AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (Object obj in sprites)
        {
            if(spriteName==obj.name)
                return obj as Sprite;
        }
        return null;
#else
        return null;
#endif
    }
    //����ͼ���ļ���������ͼƬ�����ظ��ⲿ
    public Dictionary<string,Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string,Sprite> spriteDic= new Dictionary<string,Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (Object obj in sprites)
        {
            spriteDic.Add(obj.name, obj as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif
    }
}
