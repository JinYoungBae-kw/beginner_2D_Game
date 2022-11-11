using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //���� �ִϸ��̼�
        if(Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
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
            }
            else //�ƴϸ� ������ ����.
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnDamaged(Vector2 targetPos) //������ �����ð� �ο�
    {
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
        }
        else if(collision.gameObject.tag == "Finish")
        {
            //���� Ŭ���� -> ���� ����������
            gameManager.NextStage();
        }
    }
}
