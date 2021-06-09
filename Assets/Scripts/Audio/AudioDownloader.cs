using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
public  class AudioDownloader :SingletonComponent<AudioDownloader> 
{
    // Start is called before the first frame update

    public  IEnumerator LoadMusic(string songPath, Action<AudioClip> action)
    {
        Debug.Log("123"+songPath);
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(songPath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();
 
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                action(DownloadHandlerAudioClip.GetContent(www));
            }
        }
    }

    // Update is called once per frame
   
}
