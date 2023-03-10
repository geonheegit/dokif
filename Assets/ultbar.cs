using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ultbar : MonoBehaviour
{
    public Image ult_bar_img;
    public Text ult_text;

    public static void UltAdd(int amount, string target)
    {
        if (target == "player1")
        {
            player_controller.ultmeter += amount;
            if (player_controller.ultmeter >= player_controller.max_ultmeter)
            {
                player_controller.ultmeter = player_controller.max_ultmeter;
            }
        }
        else if (target == "player2")
        {
            HeroKnight.ultmeter += amount;
            if (HeroKnight.ultmeter >= HeroKnight.max_ultmeter)
            {
                HeroKnight.ultmeter = HeroKnight.max_ultmeter;
            }
        }
    }
    void Update()
    {
        if (ult_bar_img.name == "p2_ULTbar")
        {
            ult_bar_img.fillAmount = HeroKnight.ultmeter / HeroKnight.max_ultmeter;
            ult_text.text = HeroKnight.ultmeter.ToString();

            if (ult_bar_img.fillAmount == 1) // 게이지 다 차면 파란색으로
            {
                ult_bar_img.color = new Color(0, 0, 255);
            }
            else
            {
                ult_bar_img.color = new Color(255, 0, 0);
            }
        }
        else if (ult_bar_img.name == "p1_ULTbar")
        {
            ult_bar_img.fillAmount = player_controller.ultmeter / player_controller.max_ultmeter;
            ult_text.text = player_controller.ultmeter.ToString();

            if (ult_bar_img.fillAmount == 1) // 게이지 다 차면 파란색으로
            {
                ult_bar_img.color = new Color(0, 0, 255);
            }
            else
            {
                ult_bar_img.color = new Color(255, 0, 0);
            }
        }
    }
}
