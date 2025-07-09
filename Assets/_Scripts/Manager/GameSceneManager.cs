using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    public static string nextScene;
    public TextMeshProUGUI loadingText;
    public RectTransform loadingIMG;
    public float rotateSpeed;
    public Image bgIMG;

    private void Start()
    {
        StartCoroutine(LoadSceneCoroutine());
        StartCoroutine(LoadingProduction());
    }
    public static void SceneLoad(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");

    }
    private IEnumerator LoadSceneCoroutine()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            bgIMG.color = new Color(bgIMG.color.r, bgIMG.color.g, bgIMG.color.b, bgIMG.color.a + (op.progress * -255));

            if (op.progress >= 0.9f)
            {
                yield return new WaitForSeconds(3f);
                op.allowSceneActivation = true;
                yield break;
            }

        }
    }

    private IEnumerator LoadingProduction()
    {
        while (true)
        {
            loadingIMG.Rotate(0, 0, -rotateSpeed * Time.deltaTime);
            yield return null;
        }

    }
}
