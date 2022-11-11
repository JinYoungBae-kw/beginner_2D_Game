using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //점수와 스테이지 이동 관리
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    
    public void NextStage()
    {
        stageIndex++;
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    
    void Update()
    {
        
    }
}
