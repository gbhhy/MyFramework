using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
/// <summary>
/// 音乐音效管理器，音乐部分
/// </summary>
public partial class MusicManager : BaseManager<MusicManager>
{
    private AudioSource bgm = null;

    private float bgmValue = 0.5f;

    public void PlayBGM(string name)
    {
        //创建背景音乐节点，保证其不被移除
        if (bgm == null)
        {
            GameObject obj = new GameObject("BGM");
            GameObject.DontDestroyOnLoad(obj);
            bgm = obj.AddComponent<AudioSource>();
        }
        ABResManager.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
        {
            bgm.clip = clip;
            bgm.loop = true;
            bgm.Play();
        });
    }

    public void StopBGM(string name)
    {
        if (bgm == null)
            return;
        bgm.Stop();
    }

    public void PauseBGM(string name)
    {
        if (bgm == null)
            return;
        bgm.Pause();
    }

    public void ChangeBGMValue(float value)
    {
        bgmValue = value;
        if (bgm == null)
            return;
        bgm.volume = bgmValue;
    }
}


/// <summary>
/// 音效部分
/// </summary>
public partial class MusicManager : BaseManager<MusicManager>
{


    //记录音效
    private List<AudioSource> sfxList = new List<AudioSource>();
    //音效音量大小
    private float sfxValue = 0.5f;
    //音效是否在播放，添加此标识避免暂停音效时，音效被销毁
    private bool sfxIsPause = false;

    //构造函数写在这里，用于监听音效组件移除方法
    private MusicManager()
    {
        MonoManager.Instance.AddUpdateListener(UpdateSFX);
    }
    private void UpdateSFX()
    {
        if (sfxIsPause)
            return;
        //为避免边遍历边移除出问题，采用逆向遍历
        //不停遍历容器，音效播放完毕就销毁
        for (int i = sfxList.Count - 1; i >= 0; i--)
        {
            if (!sfxList[i].isPlaying)
            {
                //音效播放完毕，将它置空
                sfxList[i].clip = null;
                PoolManager.Instance.PushObj(sfxList[i].gameObject);
                sfxList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 播放指定音效
    /// </summary>
    /// <param name="name">音效名字</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否异步加载</param>
    /// <param name="callBack">加载结束后的回调</param>
    public void PlaySFX(string name, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callBack = null)
    {

        //加载音效，进行播放
        ABResManager.Instance.LoadResAsync<AudioClip>("sfx", name, (clip) =>
        {
            //从缓存池中取出音效对象，得到对应组件
            AudioSource source = PoolManager.Instance.PopObj("SFX/SFX").GetComponent<AudioSource>();
            //如果取出的音效正在使用，我们先停止它
            source.Stop();

            source.clip = clip;
            source.loop = isLoop;
            source.volume = sfxValue;
            source.Play();
            //存入音效List，方便统一管理
            if (!sfxList.Contains(source))//由于是从缓存池中取出，所以需要判断，未记录时再记录，不要重复添加
                sfxList.Add(source);

            callBack?.Invoke(source);
        }, isSync);
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="source"></param>
    public void StopSFX(AudioSource source)
    {
        if (sfxList.Contains(source))
        {
            source.Stop();
            sfxList.Remove(source);
            source.clip = null;
            PoolManager.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// 改变音效大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSFXValue(float value)
    {
        sfxValue = value;
        foreach (var sfx in sfxList)
        {
            sfx.volume = value;
        }
    }
    /// <summary>
    /// 继续播放或暂停所有音效
    /// </summary>
    /// <param name="isPlay">是否播放</param>
    public void PlayOrPauseSFX(bool isPlay)
    {
        if (isPlay)
        {
            sfxIsPause = false;
            foreach (var sfx in sfxList)
                sfx.Play();
        }
        else
        {
            sfxIsPause = true;
            foreach (var sfx in sfxList)
                sfx.Pause();
        }
    }

    /// <summary>
    /// 清空音效相关记录，过场景时务必在清空缓存池之前调用
    /// </summary>
    public void ClearSound()
    {
        foreach(var sfx in sfxList)
        {
            sfx.Stop();
            sfx.clip = null;
            PoolManager.Instance.PushObj(sfx.gameObject);
        }
        sfxList.Clear();
    }
}
