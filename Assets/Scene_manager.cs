using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scene_manager : MonoBehaviour
{
    public void OnClickGameStart()
    {
        SceneManager.LoadScene("MainFightGame");
    }
    public void OnClickHelp()
    {
        SceneManager.LoadScene("Help");
    }
    public void OnClickExit()
    {
        Application.Quit();
    }

    void Update()
    {
        
    }
}
