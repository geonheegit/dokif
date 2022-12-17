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
    void Update()
    {
        
    }
}
