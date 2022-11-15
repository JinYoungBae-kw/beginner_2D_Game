using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //������ �������� �̵� ����
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


    void Update() //������ Update�� ǥ��
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }
    public void NextStage()
    {
        //stage �̵�
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else //���� Ŭ����
        {
            //�÷��̾� ������ ����.
            Time.timeScale = 0; //�ð��� ����.
   
            //����� UI
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartBtn.SetActive(true);
        }

        //����Ʈ ���.
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f); //�پ�� ��� �Ӱ�
        }
        else
        {
            // ��� ��� ����
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            //�÷��̾� ����
            player.OnDie();
            //RETRY UI  
            RestartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) //���� ��������
    {
        if (collision.gameObject.tag == "Player")
        {
            if(health>1)
                //�ٽ� ���ƿ�.
                PlayerReposition();

            //hp ����
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
