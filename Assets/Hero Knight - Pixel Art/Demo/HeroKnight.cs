using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class HeroKnight : MonoBehaviour
{
    float m_direc;
    bool is_hitting;
    bool is_reflecting;
    bool is_stunning;
    bool is_unabletomove;
    bool is_ready_ult;
    bool debug;

    public static float health = 100f;
    public static float max_health = 100f;
    public static float ultmeter = 0f;
    public static float max_ultmeter = 100f;
    public int player_speed;
    public int jump_power;
    public int max_speed = 10;
    public int max_jump = 2;
    public int jump_count = 0;
    public int knock_back_x = 15;
    public int knock_back_y = 8;
    public int reflect_knockback_x = 10;
    public int reflect_knockback_y = 5;
    public int attack1_dmg = 10;
    public int p1_ult_dmg = 50;
    public int parrying_cooldown = 3;
    float parrying_start_cool;
    public float parrying_passed_time;


    [SerializeField] private GameObject floatingTextPrefab;

    public GameObject enemy;
    public GameObject main_cam;
    public GameObject reflection_effect;
    public GameObject reflection_ready_effect;
    public GameObject stun_icon;
    
    Rigidbody2D rig;
    Rigidbody2D enemy_rig;
    Animator anim;
    SpriteRenderer spriteRenderer;
    public ParticleSystem blood;
    public ParticleSystem parrying_eff;
    Transform main_player_trans;
    public GameObject hit_effect;
    public GameObject att1_hb;
    public GameObject att2_hb;
    public GameObject ult_highlight;
    public GameObject ult_highlight_left;
    public GameObject ult_hb_L;
    public GameObject ult_hb_R;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        enemy_rig = enemy.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        main_player_trans = GameObject.Find("player").GetComponent<Transform>();
        is_unabletomove = false;
        is_ready_ult = false;
        ultmeter = 0;
        debug = false;
    }
    void FixedUpdate()
    {
        if (!is_hitting && !is_reflecting && HeroKnight.health != 0 && !is_stunning && !is_unabletomove)
        {
            rig.velocity = new Vector2(m_direc * player_speed * Time.deltaTime, rig.velocity.y);
        }

        if (is_ready_ult)
        {
            if (!spriteRenderer.flipX) // 궁 히트박스 방향 전환 유지
            {
                ult_highlight.SetActive(true);
                ult_highlight_left.SetActive(false);
            }
            else if (spriteRenderer.flipX)
            {
                ult_highlight_left.SetActive(true);
                ult_highlight.SetActive(false);
            }
        }
        
    }
    void Update()
    {
        PlayerSettings();
        parrying_passed_time = Time.time - parrying_start_cool;

        if (Input.GetKey(KeyCode.LeftBracket) && Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (!debug)
            {
                debug = true;
            }
            else
            {
                debug = false;
            }
        }
    }
    void PlayerSettings()
    {

        if (Input.GetKeyDown(KeyCode.Y) && debug) // 디버그용 궁극기 게이치 채우기
        {
            ultmeter = 100;
        }


        if (!is_stunning) // 스턴 상태가 아닐 때
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                m_direc = -1f;
                att1_hb.transform.localPosition = new Vector3(-1.13f, 0.721f, 0);
                att2_hb.transform.localPosition = new Vector3(-1.13f, 0.721f, 0);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                m_direc = 1f;
                att1_hb.transform.localPosition = new Vector3(1.01f, 0.721f, 0);
                att2_hb.transform.localPosition = new Vector3(1.01f, 0.721f, 0);
            }

            if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                StartCoroutine("Reflection");
            }

            if (Input.GetKeyDown(KeyCode.Quote) && ultmeter == 100)
            {
                ultmeter = 0;
                StartCoroutine("Ultimate");
            }

            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                m_direc = 0f;
            }

            if (jump_count <= max_jump - 1 && HeroKnight.health != 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    rig.velocity = new Vector2(rig.velocity.x, 0);
                    rig.AddForce(new Vector3(0, jump_power, 0), ForceMode2D.Impulse);
                    jump_count += 1;
                }
            }
        }


        // 속도 한계치 제한
        if (rig.velocity.x > max_speed)
        {
            rig.velocity = new Vector3(max_speed, rig.velocity.y, 0);
        }
        else if (rig.velocity.x < -max_speed)
        {
            rig.velocity = new Vector3(-max_speed, rig.velocity.y, 0);
        }

        // 방향 전환
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !spriteRenderer.flipX && !is_unabletomove)
        {
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && spriteRenderer.flipX && !is_unabletomove)
        {
            spriteRenderer.flipX = false;
        }

        // 애니메이션 상태 전환
        if (Mathf.Abs(rig.velocity.x) <= 1)
        {
            anim.SetInteger("AnimState", 0);
        }
        else
        {
            anim.SetInteger("AnimState", 1);
        }

        if (!is_stunning && !is_unabletomove) // 스턴 상태나 이동 불가 상태가 아닐 때
        {
            // 검 공격 모션
            if (Input.GetKeyDown(KeyCode.L) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
            {
                anim.SetTrigger("Attack1");
                StartCoroutine("Att1_Onhb");
            }
            else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
            {
                att1_hb.SetActive(false);
            }
        }


        Debug.DrawRay(rig.position, new Vector3(0, -0.9f, 0), new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rig.position, Vector3.down, 0.9f, LayerMask.GetMask("Platform"));

        if (rig.velocity.y > 0.1f)
        {
            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);
        }
        if (rayHit.collider != null)
        {
            if (rig.velocity.y == 0)
            {
                anim.SetBool("Grounded", true);
                jump_count = 0;
            }
        }

        anim.SetFloat("AirSpeedY", rig.velocity.y);
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.gameObject.name == "sword_hitbox")
        {
            if (this.transform.position.x > main_player_trans.transform.position.x && !is_reflecting)
            {
                rig.velocity = new Vector2(0, 0);
                rig.AddForce(new Vector3(knock_back_x, knock_back_y, 0), ForceMode2D.Impulse);
                healthbar.Damage(attack1_dmg, "player2"); // 검 딜
                ShowText(attack1_dmg.ToString());
                CreateBlood();
                StartCoroutine("HitEffect");
                StartCoroutine("Hit_Duration");
                StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.5f));
            }
            else if (this.transform.position.x < main_player_trans.transform.position.x && !is_reflecting)
            {
                rig.velocity = new Vector2(0, 0);
                rig.AddForce(new Vector3(-knock_back_x, knock_back_y, 0), ForceMode2D.Impulse);
                healthbar.Damage(attack1_dmg, "player2"); // 검 딜
                ShowText(attack1_dmg.ToString());
                CreateBlood();
                StartCoroutine("HitEffect");
                StartCoroutine("Hit_Duration");
                StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.5f));
            }
            if (this.transform.position.x > main_player_trans.transform.position.x && is_reflecting)
            {
                rig.AddForce(new Vector3(reflect_knockback_x, 0, 0), ForceMode2D.Impulse); // 패링성공시 자신한테 반동
                enemy_rig.AddForce(new Vector3(-reflect_knockback_x, reflect_knockback_y, 0), ForceMode2D.Impulse); // 패링성공시 적한테 반동
                StartCoroutine("Reflection_Success_Eff"); // 패링성공시 성공 효과 재생
                enemy.GetComponent<player_controller>().ShowText("기절함!");
                Parrying(); // 패링 파티클 재생
                StartCoroutine(enemy.GetComponent<player_controller>().Stun()); // 패링 성공시 적 스턴
                ultbar.UltAdd(20, "player2"); // 궁극기 게이지 ADD
            }
            else if (this.transform.position.x < main_player_trans.transform.position.x && is_reflecting)
            {
                rig.AddForce(new Vector3(-reflect_knockback_x, 0, 0), ForceMode2D.Impulse); // 패링성공시 자신한테 반동
                enemy_rig.AddForce(new Vector3(reflect_knockback_x, reflect_knockback_y, 0), ForceMode2D.Impulse); // 패링성공시 적한테 반동
                StartCoroutine("Reflection_Success_Eff"); // 패링성공시 성공 효과 재생
                enemy.GetComponent<player_controller>().ShowText("기절함!");
                Parrying(); // 패링 파티클 재생
                StartCoroutine(enemy.GetComponent<player_controller>().Stun()); // 패링 성공시 적 스턴
                ultbar.UltAdd(20, "player2"); // 궁극기 게이지 ADD
            }
        }

        if (other.gameObject.name == "ult_hb_L" || other.gameObject.name == "ult_hb_R") // 상대 궁극기에 맞았을 때
        {
            StartCoroutine("HitByUlt");
            StartCoroutine("Stun");
            StartCoroutine("Hit_Duration");
        }
    }

    void CreateBlood()
    {
        blood.Play();
    }
    void Parrying()
    {
        parrying_eff.Play();
    }
    public void ShowText(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }
    IEnumerator HitEffect()
    {
        hit_effect.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        hit_effect.SetActive(false);
    }
    IEnumerator Att1_Onhb()
    {
        yield return new WaitForSeconds(0.2f);
        att1_hb.SetActive(true);
        yield return new WaitForSeconds(0.03f);
        att1_hb.SetActive(false);
    }
    IEnumerator Hit_Duration() // 맞고 적용되는 반동동안 못움직이는 코루틴
    {
        is_hitting = true;
        yield return new WaitForSeconds(0.3f);
        is_hitting = false;
    }
    IEnumerator Reflection() // 패링시도시 0.5초간 움직이지 못하는 방어자세 코루틴
    {
        if (parrying_passed_time >= parrying_cooldown)
        {
            parrying_start_cool = Time.time; //
            is_reflecting = true;
            reflection_ready_effect.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            reflection_ready_effect.SetActive(false);
            is_reflecting = false;
        }
    }
    IEnumerator Reflection_Success_Eff() // 패링성공시
    {
        reflection_ready_effect.SetActive(false);
        reflection_effect.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        reflection_effect.SetActive(false);
        is_reflecting = false;
    }

    public IEnumerator Stun()
    {
        is_stunning = true;
        stun_icon.SetActive(true);
        yield return new WaitForSeconds(1.1f);
        is_stunning = false;
        stun_icon.SetActive(false);
    }
    IEnumerator HitByUlt() // 상대방 궁 맞았을 때
    {
        int each_dmg = p1_ult_dmg / 2;
        if (this.transform.position.x > main_player_trans.transform.position.x)
        {
            healthbar.Damage(each_dmg, "player2"); // 궁 딜
            ShowText(each_dmg.ToString());
            CreateBlood();
            StartCoroutine("HitEffect");
            ShowText("기절함!");
            StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.9f));
            rig.AddForce(new Vector3(knock_back_x * 2, knock_back_y * 2, 0), ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.3f); // 돌진하고 되돌아가는데 걸리는 시간 텀

            healthbar.Damage(each_dmg, "player2"); // 궁 딜
            ShowText(each_dmg.ToString());
            CreateBlood();
            StartCoroutine("HitEffect");
            ShowText("기절함!");
            StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.9f));
            rig.AddForce(new Vector3(-knock_back_x * 2, knock_back_y * 2, 0), ForceMode2D.Impulse);
        }
        else if (this.transform.position.x < main_player_trans.transform.position.x)
        {
            healthbar.Damage(each_dmg, "player2"); // 궁 딜
            ShowText(each_dmg.ToString());
            CreateBlood();
            StartCoroutine("HitEffect");
            ShowText("기절함!");
            StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.9f));
            rig.AddForce(new Vector3(-knock_back_x * 2, knock_back_y * 2, 0), ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.3f); // 돌진하고 되돌아가는데 걸리는 시간 텀

            healthbar.Damage(each_dmg, "player2"); // 궁 딜
            ShowText(each_dmg.ToString());
            CreateBlood();
            StartCoroutine("HitEffect");
            ShowText("기절함!");
            StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.9f));
            rig.AddForce(new Vector3(knock_back_x * 2, knock_back_y * 2, 0), ForceMode2D.Impulse);
        }
    }

    public IEnumerator Ultimate()
    {
        is_ready_ult = true; // 히트박스 방향 전환 판별용 bool
        yield return new WaitForSeconds(0.4f); // 궁극기 시전시간
        is_ready_ult = false; // 히트박스 방향 전환 판별용 bool

        is_unabletomove = true;

        if (!spriteRenderer.flipX)
        {
            ult_highlight.SetActive(false);
            ult_hb_R.SetActive(true);
            yield return new WaitForSeconds(0.02f);
            ult_hb_R.SetActive(false);
        }
        else if (spriteRenderer.flipX)
        {
            ult_highlight_left.SetActive(false);
            ult_hb_L.SetActive(true);
            yield return new WaitForSeconds(0.02f);
            ult_hb_L.SetActive(false);
        }
        is_unabletomove = false;
    }
}