using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageUI : MonoBehaviour
{
    public string audioMessageURL;



    public void SetUpMessage(voiceMessage vm)
    {
        audioMessageURL = vm.url;
        if (vm.mine) GetComponent<Image>().color = Color.green;
        else GetComponent<Image>().color = Color.blue;


    }
    public void listenMessageButtonClicked()
    {
        FirebaseManager.Instance.DownloadAndPlay(audioMessageURL);

    }



}
