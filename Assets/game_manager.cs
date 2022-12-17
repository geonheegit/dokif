using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_manager : MonoBehaviour
{
    public GameObject restart_screen;
    public Text winner_text;
    void Start()
    {
        HeroKnight.health = 100;
        player_controller.health = 100;
        restart_screen.SetActive(false);
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnClickBackToMain()
    {
        SceneManager.LoadScene("Main_Screen");
    }

    void Update()
    {
        if (HeroKnight.health == 0)
        {
            restart_screen.SetActive(true);
            winner_text.text = "Player1 Win!";
        }
        else if (player_controller.health == 0)
        {
            restart_screen.SetActive(true);
            winner_text.text = "Player2 Win!";
        }
    }
}
