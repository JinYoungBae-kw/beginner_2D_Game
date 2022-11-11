using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove; //행동지표를 결정할 변수
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D collider;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CapsuleCollider2D>();

        //Invoke(): 주어진 시간이 지난 뒤, 지정된 함수를 실행하는 함수
        Invoke("Monster_Think", 5); //5초 뒤에 함수 실행
    }



    void FixedUpdate()
    {
        //기본 움직임
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);


        //플랫폼 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.2f, rigid.position.y); //몬스터가 바라보는 방향

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform")); //몬스터가 바라보는 방향으로 한칸 띄워서 ray쏨.

        if (rayHit.collider == null) //부딪히는게 없으면 
        {
            Turn();
        }
    }

    void Monster_Think()
    {
        //Range(): 최소 ~ 최대 범위의 랜덤 수 생성 (최대 제외)
        nextMove = Random.Range(-1, 2);

        //애니메이션
        anim.SetInteger("WalkSpeed", nextMove);
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        //재귀함수
        float ThinkTime = Random.Range(3f, 6f);
        Invoke("Monster_Think", ThinkTime); //random초마다 계속해서 자신 실행.
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke(); // 현재 작동 중인 모든 Invoke함수를 멈추는 함수.
        Invoke("Monster_Think", 5);
    }

    public void OnDamaged() //몬스터가 데미지 입을 때 하는 행동
    {
        //색상 제어
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //바라보는 방향 제어
        spriteRenderer.flipY = true;
        //사라짐
        collider.enabled = false;
        //살짝 점프했다 추락하기
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //5초 뒤에 사라짐.
        Invoke("DeActive", 5);
    }

    void DeActive() //사라지기
    {
        gameObject.SetActive(false);
    }
}
