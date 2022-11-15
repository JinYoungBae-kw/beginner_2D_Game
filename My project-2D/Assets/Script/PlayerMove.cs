using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D collider;
    AudioSource audioSource;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                audioSource.Play();
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                audioSource.Play();
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                audioSource.Play();
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                audioSource.Play();
                break;
            case "DIE":
                audioSource.clip = audioDie;
                audioSource.Play();
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                audioSource.Play();
                break;
        }
    }
    void Update()
    {
        //점프 애니메이션
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }
        //버튼 떼면 스탑
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //방향전환
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //걷는 애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) //오른쪽 최대 속도 제한
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < (-1) * maxSpeed) //왼쪽 최대 속도 제한
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //착지 구현
        if(rigid.velocity.y < 0) //ray를 밑으로 쏠 때
        {
            //RayCast(시작 위치, 방향, 거리, LayerMask)
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null) //빔이 무언가랑 부딪혔을 때 
            {
                if (rayHit.distance < 0.5f) //그 거리가 0.5f보다 작다면
                    anim.SetBool("isJumping", false); //점프 애니메이션을 중지하라.
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy") //플레이어가 Enemy랑 닿으면
        {
            //플레이어가 몬스터 위에 있고 아래로 낙하 중이라면 (공격)
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            else //아니면 데미지 입음.
            {
                OnDamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
        }
    }

    void OnDamaged(Vector2 targetPos) //맞으면 무적시간 부여
    {
        //hp 깎임
        gameManager.HealthDown();

        //layer를 Player에서 PlayerDamaged로 설정
        gameObject.layer = 11; 

        //맞으면 투명해짐
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //맞으면 튕겨나감.
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //애니메이션
        anim.SetTrigger("doDamaged");

        //2초 후 무적해제
        Invoke("OffDamaged", 1);
    }

    void OffDamaged() //무적해제
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //포인트
        gameManager.stagePoint += 100;

        //몬스터 죽음
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //점수
            bool isBronze = collision.gameObject.name.Contains("Bronze"); //Contains -> Bronze라는 문자열을 포함 하나요?
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            //동전 사라지기
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //게임 클리어 -> 다음 스테이지로
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    public void OnDie()
    {
        //색상 제어
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //바라보는 방향 제어
        spriteRenderer.flipY = true;
        //사라짐
        collider.enabled = false;
        //살짝 점프했다 추락하기
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
