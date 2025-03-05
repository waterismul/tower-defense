using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner;//현재 맵에 존재하는 적 리스트 정보를 얻기 위해
    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower)//타워 이미 건설된 경우
        {
            return;
        }
        
        tile.IsBuildTower = true;//타워 건설됨
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);//선택한 타일의 위치에 타워 건설
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
