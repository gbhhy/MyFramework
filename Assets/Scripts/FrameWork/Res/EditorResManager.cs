using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

/// <summary>
/// 编辑器资源管理器
/// 注意：只有在开发时能使用该管理器加载资源，用于开发功能
/// 发布后无法使用该管理器，因为它需要使用编辑器相关功能
/// </summary>
public class EditorResManager : BaseManager<EditorResManager>
{
    private string rootPath = "Assets/Editor/ArtRes/";
    private EditorResManager() { }

    //加载单个资源
    public T LoadEditorRes<T>(string path)where T:Object
    {
#if UNITY_EDITOR
        string suffix = "";
        //预设体、纹理（图片）、材质球、音效等等
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
    //加载图集相关资源
    public Sprite LoadSprite(string path,string spriteName)
    {
#if UNITY_EDITOR
        //加载图集中的所有子资源
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
    //加载图集文件中所有子图片并返回给外部
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
