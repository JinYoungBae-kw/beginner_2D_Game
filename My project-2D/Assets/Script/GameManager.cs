using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //점수와 스테이지 이동 관리
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    // UI
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;


    void Update() //점수는 Update로 표시
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        //stage 이동
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else //게임 클리어
        {
            //플레이어 움직임 멈춤.
            Time.timeScale = 0; //시간이 멈춤.
   
            //재시작 UI
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartBtn.SetActive(true);
        }

        //포인트 계산.
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f); //줄어든 목숨 붉게
        }
        else
        {
            // 모든 목숨 소진
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //플레이어 죽음
            player.OnDie();
            //RETRY UI  
            RestartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //땅에 떨어지면
    {
        if (collision.gameObject.tag == "Player")
        {
            if(health>1)
                //다시 돌아옴.
                PlayerReposition();

            //hp 깍임
            HealthDown();
        }


    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Example");
    }
}
