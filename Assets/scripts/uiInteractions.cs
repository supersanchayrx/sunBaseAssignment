using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiInteractions : MonoBehaviour
{
    public superJsonHandler jsonHandler;
    private void Start()
    {
        jsonHandler = GameObject.Find("jsonHandler").GetComponent<superJsonHandler>();
    }
    public void like(GameObject postUi)
    {
        StartCoroutine(likeAnim(postUi.transform.Find("PostImage").transform.Find("like").gameObject, postUi.GetComponent<indexHandler>().index));
        jsonHandler.modifyJson(postUi.GetComponent<indexHandler>().index, 0);

        //bool wasAlreadyLiked = jsonHandler.socialMediaData.posts[index].post.liked;
    }
    public void comment(GameObject postUi)
    {
        jsonHandler.modifyJson(postUi.GetComponent<indexHandler>().index, 1);
    }
    public void share(GameObject postUi)
    {
        jsonHandler.modifyJson(postUi.GetComponent<indexHandler>().index, 2);
    }
    
    public void follow(GameObject postUi)
    {
        jsonHandler.modifyJson(postUi.GetComponent<indexHandler>().index, 3);
    }


    IEnumerator likeAnim(GameObject likeSprite, int index)
    {
        if (jsonHandler.socialMediaData.posts[index].post.liked)
            yield break;

        RectTransform rectPos = likeSprite.GetComponent<RectTransform>();

        LeanTween.move(rectPos, new Vector2(0, 0), 0.9f).setEase(LeanTweenType.easeOutBack);

        yield return new WaitForSeconds(1.2f);

        LeanTween.move(rectPos, new Vector2(0, -850), 0.5f).setEase(LeanTweenType.easeOutBack);
        //rectPos.transform.position = new Vector2(0,-850f);
    }
}
