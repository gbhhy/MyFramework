using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
/// <summary>
/// ������Ч�����������ֲ���
/// </summary>
public partial class MusicManager : BaseManager<MusicManager>
{
    private AudioSource bgm = null;

    private float bgmValue = 0.5f;

    public void PlayBGM(string name)
    {
        //�����������ֽڵ㣬��֤�䲻���Ƴ�
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
/// ��Ч����
/// </summary>
public partial class MusicManager : BaseManager<MusicManager>
{


    //��¼��Ч
    private List<AudioSource> sfxList = new List<AudioSource>();
    //��Ч������С
    private float sfxValue = 0.5f;
    //��Ч�Ƿ��ڲ��ţ���Ӵ˱�ʶ������ͣ��Чʱ����Ч������
    private bool sfxIsPause = false;

    //���캯��д��������ڼ�����Ч����Ƴ�����
    private MusicManager()
    {
        MonoManager.Instance.AddUpdateListener(UpdateSFX);
    }
    private void UpdateSFX()
    {
        if (sfxIsPause)
            return;
        //Ϊ����߱������Ƴ������⣬�����������
        //��ͣ������������Ч������Ͼ�����
        for (int i = sfxList.Count - 1; i >= 0; i--)
        {
            if (!sfxList[i].isPlaying)
            {
                //��Ч������ϣ������ÿ�
                sfxList[i].clip = null;
                PoolManager.Instance.PushObj(sfxList[i].gameObject);
                sfxList.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ����ָ����Ч
    /// </summary>
    /// <param name="name">��Ч����</param>
    /// <param name="isLoop">�Ƿ�ѭ��</param>
    /// <param name="isSync">�Ƿ��첽����</param>
    /// <param name="callBack">���ؽ�����Ļص�</param>
    public void PlaySFX(string name, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callBack = null)
    {

        //������Ч�����в���
        ABResManager.Instance.LoadResAsync<AudioClip>("sfx", name, (clip) =>
        {
            //�ӻ������ȡ����Ч���󣬵õ���Ӧ���
            AudioSource source = PoolManager.Instance.PopObj("SFX/SFX").GetComponent<AudioSource>();
            //���ȡ������Ч����ʹ�ã�������ֹͣ��
            source.Stop();

            source.clip = clip;
            source.loop = isLoop;
            source.volume = sfxValue;
            source.Play();
            //������ЧList������ͳһ����
            if (!sfxList.Contains(source))//�����Ǵӻ������ȡ����������Ҫ�жϣ�δ��¼ʱ�ټ�¼����Ҫ�ظ����
                sfxList.Add(source);

            callBack?.Invoke(source);
        }, isSync);
    }

    /// <summary>
    /// ֹͣ������Ч
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
    /// �ı���Ч��С
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
    /// �������Ż���ͣ������Ч
    /// </summary>
    /// <param name="isPlay">�Ƿ񲥷�</param>
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
    /// �����Ч��ؼ�¼��������ʱ�������ջ����֮ǰ����
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
