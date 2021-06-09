using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : SingletonComponent<UIController>
{
    // Start is called before the first frame update

    [SerializeField] Text stateText;
    [SerializeField] Text debugText;

    [SerializeField] GameObject setUserNamePanel;
    [SerializeField] GameObject Screen1;
    [SerializeField] GameObject Screen2;
    [SerializeField] GameObject SearchUsernameScreen;
    [SerializeField] GameObject ProfilePrefab;
    [SerializeField] GameObject MessagePrefab;
    [SerializeField] GameObject DebugWindow;


    [SerializeField] GameObject Profiles;
    [SerializeField] GameObject Messages;

    [SerializeField] InputField userNameField;
    [SerializeField] InputField searchUserNameField;


    public void LogAppState(string currentState)
    {
        stateText.text = currentState;
    }
    public void SetUserNamePanelToggle(bool enable)
    {
        setUserNamePanel.SetActive(enable);
    }

    public void Screen1Toggle(bool enable)
    {
        Screen1.SetActive(enable);
    }

    public void Screen2Toggle(bool enable)
    {
        Screen2.SetActive(enable);
    }


    public void SetUserName()
    {
        //userNameField.text

        PlayFabManager.Instance.SetUserNameOnPlayFab(userNameField.text);

    }
    public void SearchUserName()
    {
        //userNameField.text
        DebugCurrentState("Searching For Player");
        Debug.Log(searchUserNameField.text);
        PlayFabManager.Instance.GetOtherPlayer(searchUserNameField.text);

    }


    public void NewUserFound(Chat chat)
    {
        SearchUsernameScreen.SetActive(false);
        Screen2Toggle(true);
        GameObject newOb = GameObject.Instantiate(ProfilePrefab, Profiles.transform);
        chat.profile = newOb;
        newOb.GetComponent<ProfileUI>().SetUpProfile(chat.User);
        OnProfileClicked(chat);

    }
    public void OldUserFound(Chat chat)
    {
        SearchUsernameScreen.SetActive(false);
        Screen2Toggle(true);
        OnProfileClicked(chat);
    }
    
    public void OnProfileClicked(Chat chat)
    {
        foreach (voiceMessage vm in chat.VoiceMessageUrls)
        {
            GameObject newOb = Instantiate(MessagePrefab, Messages.transform);
            newOb.GetComponent<MessageUI>().SetUpMessage(vm);
        }
    }
    public void AddNewMessageToUI(voiceMessage vm)
    {

    GameObject newOb = Instantiate(MessagePrefab, Messages.transform);
            newOb.GetComponent<MessageUI>().SetUpMessage(vm);
        


    }
    public void DebugCurrentState(string txt, int timeToTurnOff = 5)
    {
        debugText.text = txt;
        DebugWindow.SetActive(true);
        CancelInvoke();
        Invoke("TurnOffDebug", timeToTurnOff);
    }


    public void TurnOffDebug()
    {
        DebugWindow.SetActive(false);
    }

}
