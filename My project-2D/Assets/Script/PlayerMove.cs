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
        //���� �ִϸ��̼�
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }
        //��ư ���� ��ž
        if(Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //������ȯ
        if(Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        //�ȴ� �ִϸ��̼�
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) //������ �ִ� �ӵ� ����
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < (-1) * maxSpeed) //���� �ִ� �ӵ� ����
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //���� ����
        if(rigid.velocity.y < 0) //ray�� ������ �� ��
        {
            //RayCast(���� ��ġ, ����, �Ÿ�, LayerMask)
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null) //���� ���𰡶� �ε����� �� 
            {
                if (rayHit.distance < 0.5f) //�� �Ÿ��� 0.5f���� �۴ٸ�
                    anim.SetBool("isJumping", false); //���� �ִϸ��̼��� �����϶�.
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy") //�÷��̾ Enemy�� ������
        {
            //�÷��̾ ���� ���� �ְ� �Ʒ��� ���� ���̶�� (����)
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
                PlaySound("ATTACK");
            }
            else //�ƴϸ� ������ ����.
            {
                OnDamaged(collision.transform.position);
                PlaySound("DAMAGED");
            }
        }
    }

    void OnDamaged(Vector2 targetPos) //������ �����ð� �ο�
    {
        //hp ����
        gameManager.HealthDown();

        //layer�� Player���� PlayerDamaged�� ����
        gameObject.layer = 11; 

        //������ ��������
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //������ ƨ�ܳ���.
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        //�ִϸ��̼�
        anim.SetTrigger("doDamaged");

        //2�� �� ��������
        Invoke("OffDamaged", 1);
    }

    void OffDamaged() //��������
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnAttack(Transform enemy)
    {
        //����Ʈ
        gameManager.stagePoint += 100;

        //���� ����
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //����
            bool isBronze = collision.gameObject.name.Contains("Bronze"); //Contains -> Bronze��� ���ڿ��� ���� �ϳ���?
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            //���� �������
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //���� Ŭ���� -> ���� ����������
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    public void OnDie()
    {
        //���� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //�ٶ󺸴� ���� ����
        spriteRenderer.flipY = true;
        //�����
        collider.enabled = false;
        //��¦ �����ߴ� �߶��ϱ�
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
