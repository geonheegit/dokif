using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start_scene : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    static void FirstLoad()
    {
        if (SceneManager.GetActiveScene().name.CompareTo("Main_Screen") != 0)
        {
            SceneManager.LoadScene("Main_Screen");
        }
    }
}
