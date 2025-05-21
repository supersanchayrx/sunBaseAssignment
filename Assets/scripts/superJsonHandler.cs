using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Networking;


[Serializable]
public class UserData
{
    public string profileName; //done
    public string profilePicture; //done
    public bool onlineStatus; //done
    public string lastSeen; //done
    public bool following; //done
}

[Serializable]
public class PostContent
{
    public string postPicture; //done
    public string caption; //done
    public int commentCount; //done
    public int likeCount; //done
    public int shareCount; //done
    public bool liked; //done
}

[Serializable]
public class PostData
{
    public UserData user;
    public PostContent post;
}

[Serializable]
public class SocialMediaData
{
    public List<PostData> posts;
}

public class superJsonHandler : MonoBehaviour
{
    public SocialMediaData socialMediaData;
    public List<GameObject> postUis;

    public Color likedColor, unlikedColor, followingColor, notFollowingColor;
    private void Start()
    {
        getThePostsUi();

        likedColor = postUis[4].transform.Find("Like").GetComponent<Image>().color;
        unlikedColor = postUis[0].transform.Find("Like").GetComponent<Image>().color;

        notFollowingColor = postUis[4].transform.Find("Following").GetComponent<Image>().color;
        followingColor = postUis[0].transform.Find("Following").GetComponent<Image>().color;


        loadSocialData("superCoolSocialMediaApp.json");
    }


    void loadSocialData(string pathToJson)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, pathToJson);

        if (!File.Exists(filePath))
        {
            Debug.Log("file doesnt exist");
            return;
        }
        //Debug.Log("mil gya");
        string jsonText = File.ReadAllText(filePath);
        socialMediaData = JsonUtility.FromJson<SocialMediaData>(jsonText);
        //Debug.Log(socialMediaData.posts[1].user.profileName);
        refreshUi();
    }

    void getThePostsUi()
    {
        postUis = new List<GameObject>(GameObject.FindGameObjectsWithTag("PostUI"));
        postUis.Reverse();
    }

    void refreshUi()
    {
        for (int i = 0; i < postUis.Count; i++)
        {
            postUis[i].GetComponent<indexHandler>().index = i;
            if (i < socialMediaData.posts.Count)
            {
                updateFields(i);
            }
        }
    }

    void updateFields(int index)
    {
        postUis[index].transform.Find("UserName").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].user.profileName;
        postUis[index].transform.Find("Caption").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].post.caption;
        postUis[index].transform.Find("last seen").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].user.lastSeen;
        postUis[index].transform.Find("Following").transform.Find("folText").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].user.following ? "FOLLOWING" : "FOLLOW";
        postUis[index].transform.Find("Following").GetComponent<Image>().color = socialMediaData.posts[index].user.following ? followingColor : notFollowingColor;
        postUis[index].transform.Find("onlineStatus").GetComponent<Image>().color = socialMediaData.posts[index].user.onlineStatus ? Color.green : Color.white;

        postUis[index].transform.Find("shareCount").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].post.shareCount.ToString();
        postUis[index].transform.Find("likeCount").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].post.likeCount.ToString();
        postUis[index].transform.Find("commentCount").GetComponent<TextMeshProUGUI>().text = socialMediaData.posts[index].post.commentCount.ToString();
        postUis[index].transform.Find("Like").GetComponent<Image>().color = socialMediaData.posts[index].post.liked ? likedColor : unlikedColor;

        StartCoroutine(loadImage(socialMediaData.posts[index].user.profilePicture, postUis[index].transform.Find("pfp").transform.Find("Image").GetComponent<Image>()));
        StartCoroutine(loadImage(socialMediaData.posts[index].post.postPicture, postUis[index].transform.Find("PostImage").GetComponent<Image>()));

    }

    IEnumerator loadImage(string imagePath, Image pfpOrPost)
    {
        string path = Path.Combine(Application.streamingAssetsPath, imagePath);

        UnityWebRequest imageRequest;

        imageRequest = UnityWebRequestTexture.GetTexture(path);

        yield return imageRequest.SendWebRequest();

        if (imageRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("err" + imagePath);
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        pfpOrPost.sprite = sprite;

    }


    public void modifyJson(int index, int interaction)
    {
        switch (interaction)
        {
            case 0:
                like(index);
                break;
            case 1:
                comment(index);
                break;
            case 2:
                share(index);
                break;
            case 3:
                follow(index);
                break;
        };

        string path = Path.Combine(Application.streamingAssetsPath, "superCoolSocialMediaApp.json");
        string newJson = JsonUtility.ToJson(socialMediaData, true);
        File.WriteAllText(path, newJson);

        loadSocialData("superCoolSocialMediaApp.json");
    }


    void like(int index)
    {
        socialMediaData.posts[index].post.liked = !socialMediaData.posts[index].post.liked;
        socialMediaData.posts[index].post.likeCount += socialMediaData.posts[index].post.liked ? 1 : -1;
    }

    void comment(int index)
    {
        socialMediaData.posts[index].post.commentCount++;
    }

    void share(int index)
    {
        socialMediaData.posts[index].post.shareCount++;
    }

    void follow(int index)
    {
        socialMediaData.posts[index].user.following = !socialMediaData.posts[index].user.following;
    }
}