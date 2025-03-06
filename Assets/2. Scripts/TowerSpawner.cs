using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private int towerBuildGold = 50; //타워건설에 사용되는 골드
    [SerializeField] private EnemySpawner enemySpawner;//현재 맵에 존재하는 적 리스트 정보를 얻기 위해
    [SerializeField] private PlayerGold playerGold; //타워 건설 시 골드 감소를 위해
    public void SpawnTower(Transform tileTransform)
    {
        if (towerBuildGold > playerGold.CurrentGold)//타워 건설할 만큼 돈 없으면 건설 불가
        {
            return;
        }
        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower)//타워 이미 건설된 경우
        {
            return;
        }
        
        tile.IsBuildTower = true;//타워 건설됨
        playerGold.CurrentGold -= towerBuildGold;//건설에 사용한 골드만큼 감소

        Vector3 position = tileTransform.position + Vector3.back;//선택한 타일의 위치에 타워 건설, 타워보다 z축 -1의 위치에 배치
        GameObject clone = Instantiate(towerPrefab, position, Quaternion.identity);//선택한 타일의 위치에 타워 건설
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);//타워무기에 enemySpawner 정보 전달
    }
}
