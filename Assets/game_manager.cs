using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_manager : MonoBehaviour
{
    public GameObject restart_screen;
    public Text winner_text;
    float start_time;
    void Start()
    {
        HeroKnight.health = 100;
        player_controller.health = 100;
        restart_screen.SetActive(false);
        start_time = Time.time;
    }

    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void OnClickBackToMain()
    {
        SceneManager.LoadScene("Main_Screen");
    }
    void UltMeterPerTime()
    {
        if (Time.time - start_time >= 1f)
        {
            start_time = Time.time;
            ultbar.UltAdd(2, "player1");
            ultbar.UltAdd(2, "player2");
        }
    }
    void Update()
    {
        UltMeterPerTime();

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
