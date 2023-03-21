using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    string SceneName;
    [SerializeField]
    bool LoadAdditive;

    

    public void LoadScene()
    {
        if (LoadAdditive)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
        }
    }
}
