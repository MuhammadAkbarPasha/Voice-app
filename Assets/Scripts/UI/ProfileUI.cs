using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProfileUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Text Name;
    public Text LossCount;
    public Text WinCount;


    public Button WinButton;
    public Button LoseButton;
    [SerializeField] User user;

    public void SetUpProfile(User user)
    {
        this.user=user;
        this.gameObject.name = user.userName;
        Name.text = user.userName;
        LossCount.text = user.losses.ToString();
        WinCount.text = user.wins.ToString();
        WinButton.onClick.AddListener(() =>
        {
            PlayFabManager.Instance.WinOrLose(user.playFabUserId,"Wins",Win);
        });

        LoseButton.onClick.AddListener(() =>
        {
            PlayFabManager.Instance.WinOrLose(user.playFabUserId,"Losses",Lose);
        });
    }
    public void Win()
    {
        this.user.wins=this.user.wins+1;
        WinCount.text=  this.user.wins.ToString();
    }

 public void Lose()
    {
        this.user.losses=this.user.losses+1;
        LossCount.text=( this.user.losses).ToString();
    }
public void OnThisProfileClicked()
{
ChatManager.Instance.PlayerFoundOrClicked(user);


}

}





