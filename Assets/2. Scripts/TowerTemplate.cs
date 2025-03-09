using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] //Create메뉴에 생성
public class TowerTemplate : ScriptableObject//싱글톤을 쓰긴 애매한데 데이터는 공유해야할때, 싱글톤과 달리 런타임시 변경된 데이터는 변경되지 않음.
{
    public GameObject towerPrefab; //타워 생성을 위한 프리팹
    public GameObject followTowerPrefab;//임시 타워 프리팹
    public Weapon[] weapon;//레벨별 타워(무기)정보

    [System.Serializable]
    public struct Weapon
    {
        public Sprite sprite;//보여지는 타워 이미지
        public float damage;//공격력
        public float rate;//공격 속도
        public float range;//공격 범위
        public int cost;//필요 골드(0레벨 : 건설, 1~레벨 : 업그레이드
        public int sell;//타워 판매 시 획득 골드
    }
}
