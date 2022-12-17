using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{
    float m_direc;
    bool is_hitting;
    bool is_reflecting;

    public static float health = 100f;
    public static float max_health = 100f;
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
    public int attack2_dmg = 20;

    [SerializeField] private GameObject floatingTextPrefab;

    public GameObject enemy;
    public GameObject reflection_effect;
    public GameObject reflection_ready_effect;
    Rigidbody2D enemy_rig;

    public GameObject main_cam;
    Rigidbody2D rig;
    Animator anim;
    public ParticleSystem blood;
    SpriteRenderer spriteRenderer;
    Transform main_player_trans;
    public ParticleSystem dust;
    public GameObject sword_h;
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        enemy_rig = enemy.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        main_player_trans = GameObject.Find("HeroKnight").GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (!is_hitting && !is_reflecting && player_controller.health != 0)
        {
            rig.velocity = new Vector2(m_direc * player_speed * Time.deltaTime, rig.velocity.y);
        }
    }

    void Update()
    {
        PlayerSettings();
    }

    void PlayerSettings()
    {
        if (Input.GetKey(KeyCode.A))
        {
            m_direc = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            m_direc = 1f;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            StartCoroutine("Reflection");
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            m_direc = 0f;
        }

        if (jump_count < max_jump - 1 && player_controller.health != 0)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                rig.velocity = new Vector2(rig.velocity.x, 0);
                rig.AddForce(new Vector3(0, jump_power, 0), ForceMode2D.Impulse);
                jump_count += 1;
                CreateDust();
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
        if (Input.GetKeyDown(KeyCode.A) && !spriteRenderer.flipX)
        {
            spriteRenderer.flipX = true;
            sword_h.transform.localPosition = new Vector3(-0.35f, 0, 0);
            CreateDust();
        }
        else if (Input.GetKeyDown(KeyCode.D) && spriteRenderer.flipX)
        {
            spriteRenderer.flipX = false;
            sword_h.transform.localPosition = new Vector3(0.37f, 0, 0);
            CreateDust();
        }

        // 애니메이션 상태 전환
        if (Mathf.Abs(rig.velocity.x) <= 1)
        {
            anim.SetBool("is_running", false);
        }
        else
        {
            anim.SetBool("is_running", true);
        }

        // 검 공격 모션
        if (Input.GetKeyDown(KeyCode.C) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            anim.SetTrigger("is_attack1");
            StartCoroutine("Sword_Onhb");
        }
        else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            sword_h.SetActive(false);
        }

        // 창 공격 모션
        if (Input.GetKeyDown(KeyCode.V) && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            anim.SetTrigger("is_attack3");
        }

        Debug.DrawRay(rig.position, new Vector3(0, -0.9f, 0), new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rig.position, Vector3.down, 0.9f, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null)
        {
            if (rayHit.distance < 0.9f)
            {
                anim.SetBool("is_jumping", false);
                anim.SetBool("is_falling", false);
            }
        }
        else if (rig.velocity.y > 0.1f)
        {
            anim.SetBool("is_jumping", true);
            anim.SetBool("is_falling", false);
        }
        else if (rig.velocity.y < -0.1f)
        {
            anim.SetBool("is_falling", true);
            anim.SetBool("is_jumping", false);
        }

        // 점프 카운트 초기화
        if (anim.GetBool("is_falling") == false && anim.GetBool("is_jumping") == false)
        {
            jump_count = 0;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "att1_hb")
        {
            if (this.transform.position.x > main_player_trans.transform.position.x && !is_reflecting)
            {
                rig.velocity = new Vector2(0, 0);
                rig.AddForce(new Vector3(knock_back_x, knock_back_y, 0), ForceMode2D.Impulse);
                healthbar.Damage(attack1_dmg, "player1"); // 공격1 딜
                ShowDamage(attack1_dmg.ToString());
                CreateBlood();
                StartCoroutine("Hit_Duration");
                StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.5f));
            }
            else if (this.transform.position.x < main_player_trans.transform.position.x && !is_reflecting)
            {
                rig.velocity = new Vector2(0, 0);
                rig.AddForce(new Vector3(-knock_back_x, knock_back_y, 0), ForceMode2D.Impulse);
                healthbar.Damage(attack1_dmg, "player1"); // 공격1 딜
                ShowDamage(attack1_dmg.ToString());
                CreateBlood();
                StartCoroutine("Hit_Duration");
                StartCoroutine(main_cam.GetComponent<cam_movement>().CamShake(0.07f, 0.5f));
            }

            if (this.transform.position.x > main_player_trans.transform.position.x && is_reflecting)
            {
                rig.AddForce(new Vector3(reflect_knockback_x, 0, 0), ForceMode2D.Impulse); // 패링성공시 자신한테 반동
                enemy_rig.AddForce(new Vector3(-reflect_knockback_x, reflect_knockback_y, 0), ForceMode2D.Impulse); // 패링성공시 적한테 반동
                StartCoroutine("Reflection_Success_Eff"); // 패링성공시 성공 효과 재생
            }
            else if (this.transform.position.x < main_player_trans.transform.position.x && is_reflecting)
            {
                rig.AddForce(new Vector3(-reflect_knockback_x, 0, 0), ForceMode2D.Impulse); // 패링성공시 자신한테 반동
                enemy_rig.AddForce(new Vector3(reflect_knockback_x, reflect_knockback_y, 0), ForceMode2D.Impulse); // 패링성공시 적한테 반동
                StartCoroutine("Reflection_Success_Eff"); // 패링성공시 성공 효과 재생
            }
        }
    }

    void CreateDust()
    {
        dust.Play();
    }
    void CreateBlood()
    {
        blood.Play();
    }
    void ShowDamage(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
        }
    }

    IEnumerator Sword_Onhb()
    {
        yield return new WaitForSeconds(0.2f);
        sword_h.SetActive(true);
        yield return new WaitForSeconds(0.03f);
        sword_h.SetActive(false);
    }
    IEnumerator Hit_Duration()
    {
        is_hitting = true;
        yield return new WaitForSeconds(0.3f);
        is_hitting = false;
    }
    IEnumerator Reflection() // 패링시도시 0.5초간 움직이지 못하는 방어자세 코루틴
    {
        is_reflecting = true;
        reflection_ready_effect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        reflection_ready_effect.SetActive(false);
        is_reflecting = false;
    }
    IEnumerator Reflection_Success_Eff() // 패링성공시
    {
        reflection_ready_effect.SetActive(false);
        reflection_effect.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        reflection_effect.SetActive(false);
        is_reflecting = false;
    }
}
