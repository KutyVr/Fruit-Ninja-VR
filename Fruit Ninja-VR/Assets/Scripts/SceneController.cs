using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public void LoadThisScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);

    }
}
