using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using ExitGames.Client.Photon.Encryption;
using Photon.Chat.UtilityScripts;
using System;
using System.Globalization;
[System.Serializable]
public class voiceMessage
{
    public bool mine;
    public string url;
    public GameObject messageObject;

    public voiceMessage(bool isMine, string msgUrl)
    {
        this.mine = isMine;
        this.url = msgUrl;

    }

}
[System.Serializable]
public class Chat
{
    public User User;
    public List<voiceMessage> VoiceMessageUrls = new List<voiceMessage>();
    public GameObject profile;
    public Chat(User user)
    {
        this.User = user;


    }
}
public sealed class ChatManager : SingletonComponent<ChatManager>, IChatClientListener
{

    #region Private_VARIABLES
    private string GroupName;
    private ChatClient chatClient;
    #endregion

    #region PUBLIC_VARIABLES
    public List<Chat> chats = new List<Chat>();
    public bool NewMember = false;
    public Chat currentChat;
    #endregion

    #region Public_Methods

    public void setgroupName(string groupName)
    {
        this.GroupName = groupName;
    }

    public string getgroupName()
    {
        return GroupName;
    }

    /// <summary>
    /// Unsubscribe from a channel
    /// </summary>
    /// <param name="groupName"></param>
    public void Unsubscribe()
    {
        if (chatClient == null)
            return;

    }



    public void DebugReturn(DebugLevel level, string message)
    {
        //Debug.LogError(level.ToString() + "/-/" + message);
    }
    /// <summary>
    /// State change callback
    /// </summary>
    /// <param name="state"></param>
    public void OnChatStateChange(ChatState state)
    {
        //Debug.LogError(state.ToString());
    }


    public void PlayerFoundOrClicked(User user)
    {
        if (chats.Exists((ob) => ob.User.userName == user.userName))
        {
            currentChat = chats.Find((ob) => ob.User.userName == user.userName);
            UIController.Instance.OldUserFound(currentChat);
        }
        else
        {
            chats.Add(new Chat(user));
            UIController.Instance.NewUserFound(chats[chats.Count - 1]);
            currentChat = chats[chats.Count - 1];
        }
    }
    public void OnConnected()
    {
        UIController.Instance.Screen1Toggle(true);
        Debug.Log("Connected To Photon");
    }


    /// <summary>
    /// Ondisconnected callback
    /// </summary>
    public void OnDisconnected()
    {
        chatClient.SetOnlineStatus(0);
        //throw new System.NotImplementedException();
        //Debug.LogError(chatClient.DisconnectedCause);
    }

    /// <summary>
    /// On Get Messages callback , this is called on both receiving and sending ends
    /// </summary>
    /// <param name="channelName"></param>
    /// <param name="senders"></param>
    /// <param name="messages"></param>
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {



        //need to cater here for life message
    }

    public void SendPrivateMessage(string fileName)
    {
        SendPrivateMessage(currentChat.User.userName, fileName);

    }

    public void SendPrivateMessage(string user, string message)
    {
        if (!chatClient.CanChat) return;
        Debug.Log("I can chat");
        UIController.Instance.DebugCurrentState("Sending Audio", 1);
        chatClient.SendPrivateMessage(user, message);
    }
    /// <summary>
    /// Onsend private message callback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <param name="channelName"></param>
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Console.WriteLine("OnPrivateMessage: {0} ({1}) > {2}", channelName, sender, message);
        AddNewMessage(sender, message.ToString());
    }


    public void AddNewMessage(string sender, string message)
    {

        if(sender==PlayFabManager.Instance.CurrentUser.userName )
        {
            currentChat.VoiceMessageUrls.Add(new voiceMessage(true,message)   );
         
            int index = chats.FindIndex((ob) => ob.User.userName == currentChat.User.userName);
            chats[index]=currentChat;
            UIController.Instance.AddNewMessageToUI(new voiceMessage(true,message)     );
            return;
        }

        if (chats.Exists((ob) => ob.User.userName == sender))
        {
            int index = chats.FindIndex((ob) => ob.User.userName == sender);
            chats[index].VoiceMessageUrls.Add(new voiceMessage(false, message));
            if(currentChat.User.userName==sender)
            UIController.Instance.AddNewMessageToUI(new voiceMessage(true,message)     );
            
        }
        else
        {


              chats.Add(new Chat(new User(sender)       ));
                chats[chats.Count-1].VoiceMessageUrls.Add(new voiceMessage(false, message));
            PlayFabManager.Instance.GetOtherPlayer(sender);
            

        }

    }
    /// <summary>
    /// Send message function message item class object is converted to json here
    /// </summary>
    public void SendMessages()
    {


    }

    //chirag......
    public void SendMessagesByPassingValue(string message)
    {

    }


    /// <summary>
    /// Authenticate this user with the playfab
    /// </summary>
    /// <param name="token"></param>
    public void AuthenticateWithPlayfab(string token)
    {
        chatClient = new ChatClient(this);
        chatClient.ChatRegion = "EU";
        chatClient.Connect(PlayFabManager.Instance.PhotonChatAppId, "1.0", new Photon.Chat.AuthenticationValues(PlayFabManager.Instance.CurrentUser.userName));
    }

    /// <summary>
    /// Update is used for keeping the chat client service working
    /// </summary>
    public void Update()
    {

        if (chatClient != null)
            chatClient.Service();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        throw new NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    #endregion
}