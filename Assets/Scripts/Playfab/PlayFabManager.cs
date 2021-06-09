using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using ExitGames.Client.Photon;
using System;
using PlayFab.CloudScriptModels;
using System.Globalization;

public class PlayFabManager : BasicPlayfab
{

    #region Login Management 

    public override void Start()
    {
        base.Start();
        LoginDevice();
    }
    public override void LoginDevice()
    {
        base.LoginDevice();
        Debug.Log("Title id is : " + PlayFabSettings.TitleId);
        PlayerPrefs.GetString("Login_Type", "Device");

#if UNITY_ANDROID || ANDROID
        LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDevice = SystemInfo.deviceModel,
            AndroidDeviceId = ReturnDeviceId(),
            TitleId = PlayFabSettings.TitleId,
            OS = SystemInfo.operatingSystem,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithAndroidDeviceID(request, DeviceLoginSuccess,
                (CustomIdLoginError) =>
                {
                    Debug.Log("Error logging in player with custom ID: ");
                    Debug.Log(CustomIdLoginError.ErrorMessage);
                }
              );
#endif

#if UNITY_IOS || IOS

        //IOS will be here

        LoginWithIOSDeviceIDRequest iosrequest = new LoginWithIOSDeviceIDRequest { TitleId = PlayFabSettings.TitleId, DeviceId = ReturnDeviceId(), CreateAccount = true, OS = SystemInfo.operatingSystem };
        PlayFabClientAPI.LoginWithIOSDeviceID(  iosrequest,DeviceLoginSuccess,
              (CustomIdLoginError) =>
              {
                  Debug.Log("Error logging in player with custom ID: ");
                  Debug.Log(CustomIdLoginError.ErrorMessage);
              }
   );
#endif
    }
    public string ReturnDeviceId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public override void DeviceLoginSuccess(LoginResult deviceLoginResult)
    {
        base.DeviceLoginSuccess(deviceLoginResult);

        CurrentUser.playFabUserId = deviceLoginResult.PlayFabId;
        if (deviceLoginResult.NewlyCreated) Debug.Log("Logged In, New Account");
        else Debug.Log("Logged In, Old Account");
        GetCurrentUserData();

    }



    public override void SetUserNameOnPlayFab(string userName)
    {
        base.SetUserNameOnPlayFab(userName);
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,

            resultCallback =>
            {
                Debug.Log("Successfully Changed");
                UIController.Instance.SetUserNamePanelToggle(false);
                GetPhotonDetails();
            }, errorCallback => { Debug.Log(errorCallback.Error); }
            );
    }
    private void OnLoginSuccess(LoginResult result)
    {
        // AndroidIAPExample.Instance.RefreshIAPItems();

        CurrentUser.playFabUserId = result.PlayFabId;

    }

    private void OnLoginFail(PlayFabError error)
    {
        Debug.Log("i show result here " + error.Error);
        //PlayerPrefs.DeleteAll();
        switch (error.Error.ToString())
        {

        }
    }



    public void GetUserReadOnlyData()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetUserReadOnlyData"

        }, resultCallback =>
        {

        }
        , CloudScriptFailure
        );
    }



    [ContextMenu("GetCurrentUserData")]
    public void GetCurrentUserData()
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetCurrentUserData",

        }, CloudScriptSuccess
        , CloudScriptFailure
        );
    }
    public void GetOtherUserAccountInfo(string otherPlayerUsername) // fixed current member id issue 
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest()
        {
            TitleDisplayName = otherPlayerUsername
        };
        PlayFabClientAPI.GetAccountInfo(request, result =>
        {
            GetOtherPlayerUser(result.AccountInfo.PlayFabId);


        }, errorCallback =>
        {
            Debug.Log(errorCallback.Error);
            UIController.Instance.DebugCurrentState("Player Not Found",2);

        });
    }

    public override void GetOtherPlayer(string requiredUserName)
    {
        base.GetOtherPlayer(requiredUserName);
        GetOtherUserAccountInfo(requiredUserName);

    }
    public void GetOtherPlayerUser(string playFabId)
    {
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "GetOtherPlayerUser",
            FunctionParameter = new
            {
                PlayFabId = playFabId
            }

        }, CloudScriptSuccess
        , CloudScriptFailure
        );

    }

    public override void WinOrLose(string playfabId,string winOrLose,Action action)
    {
    PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest
        {
            FunctionName = "WinOrLose",
            FunctionParameter = new
            {
                playFabId = playfabId,
                stat=winOrLose
            }

        }, CloudScriptSuccess=>
        {
            action();


        }
        , CloudScriptFailure
        );


    }


    

    public void CloudScriptSuccess(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
    {
        Debug.Log(result.FunctionResult);
        Debug.Log(result.FunctionName);
        switch (result.FunctionName)
        {
            case "GetCurrentUserData":
                {
                    JsonUtility.FromJsonOverwrite(result.FunctionResult.ToString(), CurrentUser);
                    if (CurrentUser.userName.Length < 1)
                    {
                        UIController.Instance.SetUserNamePanelToggle(true);
                        return;
                    }
                    else
                    {
                        GetPhotonDetails();
                    }
                }
                break;
            case "GetOtherPlayerUser":
                {
                    if (result.FunctionResult != null)
                    {
                        ChatManager.Instance.
                        PlayerFoundOrClicked(JsonUtility.FromJson<User>(result.FunctionResult.ToString()));
                        UIController.Instance.DebugCurrentState("PlayerFound",1);
                    }
                    else
                    {

                        //
                        ///
                        /// 
                        /// 

                    }
                    UIController.Instance.TurnOffDebug();

                }
                break;
        }
    }

    public void CloudScriptFailure(PlayFabError error)
    {


    }

    public void UnlinkDevices()
    {
#if UNITY_ANDROID || ANDROID
        UnlinkAndroidDeviceIDRequest request = new UnlinkAndroidDeviceIDRequest
        {
            AndroidDeviceId = ReturnDeviceId()
        };
        PlayFabClientAPI.UnlinkAndroidDeviceID(request, (resultCallback) =>
        {

            Debug.Log(resultCallback.ToString());
        }, FailedlLinking =>
        {

            Debug.Log("Linking Failed");

        });
#endif

#if UNITY_IOS || IOS
        UnlinkIOSDeviceIDRequest request = new UnlinkIOSDeviceIDRequest
        {
            DeviceId = ReturnDeviceId()
    };
    PlayFabClientAPI.UnlinkIOSDeviceID(request, (resultCallback) => {
            //if (PlayPabUiController.Instance.isFacebookLoginRequestFromSetting)
            //{
            //    PlayPabUiController.Instance.facebookLoginSuccessPopup.SetActive(true);
            //}
            //PlayPabUiController.Instance.loadingScreenForAllOther.SetActive(false);
            //getDataOnAccountLogin();
            //OnLoginCalls();
            Debug.Log(resultCallback.ToString());
        },  FailedlLinking=>
        {

            Debug.Log("Linking Failed");

        );
#endif
    }

    #endregion
    #region Photon_Setup
    public void GetPhotonDetails()
    {

        Debug.Log("COMING HERE TOO");
        GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest()
        {
            PhotonApplicationId = PhotonChatAppId

        };
        PlayFabClientAPI.GetPhotonAuthenticationToken(request,
            resultCallback =>
            {
                ChatManager.Instance.
                AuthenticateWithPlayfab(resultCallback.PhotonCustomAuthenticationToken);
            }, error =>
            {
                Debug.Log(error.Error);
            }
            );
    }
}
#endregion


