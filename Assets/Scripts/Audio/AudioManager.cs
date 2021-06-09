using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UnityEngine.Networking;
using System;
[RequireComponent(typeof(AudioSource))]

public class AudioManager : SingletonComponent<AudioManager>
{
        [SerializeField] AudioSource audioSource;

     void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    [ContextMenu("Test Download")]
     void DownloadAndPlay()
    {
        //  DownloadAndPlay("https://ciihuy.com/downloads/music.mp3", "music.mp3");
    }
    public IEnumerator DownloadAndPlay(Uri url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //    action(DownloadHandlerAudioClip.GetContent(www));
                DownloadAndPlay(DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }
    public void DownloadAndPlay(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    } 
    public void UploadMusic(string address,string fileName)
    {
        FirebaseManager.Instance.UploadMusic(address,fileName);
    }

}
