using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate;//타워 정보(공격력, 공격속도 등등)
    [SerializeField] private EnemySpawner enemySpawner;//현재 맵에 존재하는 적 리스트 정보를 얻기 위해
    [SerializeField] private PlayerGold playerGold; //타워 건설 시 골드 감소를 위해
    [SerializeField] private SystemTextViewer systemTextViewer;//돈 부족, 건설 불가와 같은 시스템 메시지 출력
    public void SpawnTower(Transform tileTransform)
    {
        if (towerTemplate.weapon[0].cost > playerGold.CurrentGold)//타워 건설할 만큼 돈 없으면 건설 불가
        {
            //골드가 부족해서 타워 건설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower)//타워 이미 건설된 경우
        {
            //현재 위치에 타워 걸설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        
        tile.IsBuildTower = true;//타워 건설됨
        playerGold.CurrentGold -= towerTemplate.weapon[0].cost;//건설에 사용한 골드만큼 감소

        Vector3 position = tileTransform.position + Vector3.back;//선택한 타일의 위치에 타워 건설, 타워보다 z축 -1의 위치에 배치
        GameObject clone = Instantiate(towerTemplate.towerPrefab, position, Quaternion.identity);//선택한 타일의 위치에 타워 건설
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);//타워무기에 enemySpawner 정보 전달
    }
}
