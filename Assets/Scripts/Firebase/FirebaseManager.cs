using System.Collections;
using System.Collections.Generic;

using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Storage;
using Firebase.Extensions;
public class FirebaseManager : SingletonComponent<FirebaseManager>
{

    void Start()
    {
      //  DownloadAndPlay("test.wav");
    }
    public void DownloadAndPlay(string fileName)
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference reference =
        storage.GetReferenceFromUrl("gs://voicechatapp-f80a9.appspot.com/" + fileName);
        // Fetch the download URL
        reference.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                StartCoroutine(AudioManager.Instance.DownloadAndPlay(task.Result));
                // ... now download the file via WWW or UnityWebRequest.
            }
            else
            {
                Debug.Log(task.Status);

            }
        });
    }
    public void UploadMusic(string address, string fileName)
    {
UIController.Instance.DebugCurrentState("Saving Audio",1);
        
        // File located on disk
        // Uri.UriSchemeFile
        // s
        string localFile = string.Format("{0}://{1}", Uri.UriSchemeFile, address);
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        // Create a reference to the file you want to upload
        StorageReference reference =
         storage.GetReferenceFromUrl("gs://voicechatapp-f80a9.appspot.com/" + fileName);
        // Upload the file to the path "images/rivers.jpg"
        reference.PutFileAsync(localFile)
            .ContinueWith((Task<StorageMetadata> task) =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log(task.Exception.ToString());
                    // Uh-oh, an error occurred!
                }
                else
                {
                    // Metadata contains file metadata such as size, content-type, and download URL.
                    StorageMetadata metadata = task.Result;
                    string md5Hash = metadata.Md5Hash;
                    if(ChatManager.Instance!=null)   ChatManager.Instance.SendPrivateMessage(fileName);
                    Debug.Log("Finished uploading...");
                    Debug.Log("md5 hash = " + md5Hash);
                }
            });

    }
}
