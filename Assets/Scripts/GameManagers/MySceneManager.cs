using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Linq;
using TMPro;

public class MySceneManager : Singleton<MySceneManager>
{
    [SerializeField] private LoadingScreenController _loadingScreenParent;

    public void LoadSceneWithLoadingScreen(object sceneId)
    {
        AsyncOperation operation;

        if (sceneId is int)
        {
            operation = SceneManager.LoadSceneAsync((int)sceneId);
        }
        else if (sceneId is string)
        {
            operation = SceneManager.LoadSceneAsync((string)sceneId);
        }
        else
        {
            throw new Exception("SceneID must be int/string");
        }

        _loadingScreenParent.gameObject.SetActive(true);
        _loadingScreenParent.Load(operation);
    }

}