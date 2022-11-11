using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; //�ൿ��ǥ�� ������ ����
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();

        //Invoke(): �־��� �ð��� ���� ��, ������ �Լ��� �����ϴ� �Լ�
        Invoke("Monster_Think", 5); //5�� �ڿ� �Լ� ����
    }



    void FixedUpdate()
    {
        //�⺻ ������
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        //�÷��� üũ
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y); //���Ͱ� �ٶ󺸴� ����

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); //���Ͱ� �ٶ󺸴� �������� ��ĭ ����� ray��.

        if (rayHit.collider == null) //�ε����°� ������ 
        {
            Turn();
        }
    }

    void Monster_Think()
    {
        //Range(): �ּ� ~ �ִ� ������ ���� �� ���� (�ִ� ����)
        nextMove = Random.Range(-1, 2);

        //�ִϸ��̼�
        anim.SetInteger("WalkSpeed", nextMove);
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //����Լ�
        float ThinkTime = Random.Range(3f, 6f);
        Invoke("Monster_Think", ThinkTime); //random�ʸ��� ����ؼ� �ڽ� ����.
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); // ���� �۵� ���� ��� Invoke�Լ��� ���ߴ� �Լ�.
        Invoke("Monster_Think", 5);
    }

    public void OnDamaged() //���Ͱ� ������ ���� �� �ϴ� �ൿ
    {
        //���� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //�ٶ󺸴� ���� ����
        spriteRenderer.flipY = true;
        //�����
        collider.enabled = false;
        //��¦ �����ߴ� �߶��ϱ�
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //5�� �ڿ� �����.
        Invoke("DeActive", 5);
    }

    void DeActive() //�������
    {
        gameObject.SetActive(false);
    }
}
