using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage Data", menuName = "Scriptable Object/Stage Data")]
public class KSD_StageGameData : ScriptableObject
{
    public int StageLevel;                  //스테이지
    public int TargetNPCCount;               //목표 주문수
    public float VisitNPCRespawnTIme;       //가게에 들어오는 손님이 생성되는 주기
    public int MaxVisitNPCCount;            //동시간대에 가게 안에 존재할 수 있는 손님의 수
    public float StreetNPCRespawnTime;      //길거리 손생 생성 주기
    public float NPCSearchMinTime;          //손님이 매대 하나를 구경하는 최소 시간
    public float NPCSearchMaxTime;          //손님이 매대 하나를 구경하는 최대 시간
    public int NPCShuttleMinCount;          //손님이 다른 매대로 이동하는 최소 횟수
    public int NPCShuttleMaxCount;          //손님이 다른 매대로 이동하는 최대 횟수
}
