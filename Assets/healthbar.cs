using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class healthbar : MonoBehaviour
{
    public Image health_bar_img;
    public Text hp_text;

    private void Start()
    {
        
    }

    public static void Heal(int amount, string target)
    {
        if (target == "player1")
        {
            player_controller.health += amount;
            if (player_controller.health >= player_controller.max_health)
            {
                player_controller.health = player_controller.max_health;
            }
        }
        else if (target == "player2")
        {
            HeroKnight.health += amount;
            if (HeroKnight.health >= HeroKnight.max_health)
            {
                HeroKnight.health = HeroKnight.max_health;
            }
        }
    }
    public static void Damage(int amount, string target)
    {
        if (target == "player1")
        {
            player_controller.health -= amount;
            if (player_controller.health <= 0)
            {
                player_controller.health = 0;
            }
        }
        else if (target == "player2")
        {
            HeroKnight.health -= amount;
            if (HeroKnight.health <= 0)
            {
                HeroKnight.health = 0;
            }
        }
    }

    void Update()
    {
        if(health_bar_img.name == "p2_health_bar")
        {
            health_bar_img.fillAmount = HeroKnight.health / HeroKnight.max_health;
            hp_text.text = HeroKnight.health.ToString();
        }
        else
        {
            health_bar_img.fillAmount = player_controller.health / player_controller.max_health;
            hp_text.text = player_controller.health.ToString();
        }
        
    }
}
