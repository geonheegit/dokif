using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class skill_colldown : MonoBehaviour
{
    public Image parrying_img;
    public GameObject player1;
    public GameObject player2;

    void Update()
    {
        if (parrying_img.name == "p1_parrying_icon")
        {
            parrying_img.fillAmount = player1.GetComponent<player_controller>().parrying_passed_time / player1.GetComponent<player_controller>().parrying_cooldown;
        }

        if (parrying_img.name == "p2_parrying_icon")
        {
            parrying_img.fillAmount = player2.GetComponent<HeroKnight>().parrying_passed_time / player2.GetComponent<HeroKnight>().parrying_cooldown;
        }
    }
}
