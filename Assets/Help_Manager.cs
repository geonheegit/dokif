using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Help_Manager : MonoBehaviour
{
    public void OnClickReturn()
    {
        SceneManager.LoadScene("Main_Screen");
    }
    public void OnClickHelp2()
    {
        SceneManager.LoadScene("Help2");
    }
    public void OnClickHelp1()
    {
        SceneManager.LoadScene("Help");
    }
    void Update()
    {
        
    }
}
