using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using ExitGames.Client.Photon;
using System;
using PlayFab.CloudScriptModels;
using System.Globalization;

[System.Serializable]
public class User
{
    public string userName="";
    public int wins=0;
    public int losses=0;
    public string playFabUserId="123";
    public User(string uName)
    {
        this.userName=uName;

    }
}
public  class BasicPlayfab : SingletonComponent<BasicPlayfab>
{
    #region  Fields
    [Header("Playfab Settings")]
    [SerializeField] string playFabTitleId;
    [SerializeField] string photonChatAppId;
    [Header("User Data")]
    [SerializeField] User currentUser;
    #endregion

    #region Properties
    public User CurrentUser { get => currentUser; set => currentUser = value; }
    public string PhotonChatAppId { get => photonChatAppId; set => photonChatAppId = value; }
    public string PlayFabTitleId { get => playFabTitleId; set => playFabTitleId = value; }
    #endregion

    
    public virtual void Start()
    {
        PlayFabSettings.TitleId = PlayFabTitleId;
        
    }

    public virtual void LoginDevice()
    {
        UIController.Instance.LogAppState("Logging In");

    }
    public virtual void DeviceLoginSuccess(LoginResult deviceLoginResult)
    {
       UIController.Instance.LogAppState("Login Successful");
    }

 public virtual void SetUserNameOnPlayFab(string userName)
    {
        UIController.Instance.LogAppState("SettingUpUsername");
    }
public virtual void GetOtherPlayer(string requiredUserName)
    {
        UIController.Instance.LogAppState("SettingUpUsername");
    }


public virtual void WinOrLose(string playfabId,string winOrLose,Action action)
{
 UIController.Instance.LogAppState(playfabId+" Winning");

}


public virtual void LosePlayer(string playfabId,Action<User> action)
{

 UIController.Instance.LogAppState(playfabId+" Losing");
}

}
