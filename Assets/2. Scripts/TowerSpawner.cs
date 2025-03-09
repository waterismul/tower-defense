using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate[] towerTemplate; //타워 정보(공격력, 공격속도 등등)
    [SerializeField] private EnemySpawner enemySpawner; //현재 맵에 존재하는 적 리스트 정보를 얻기 위해
    [SerializeField] private PlayerGold playerGold; //타워 건설 시 골드 감소를 위해
    [SerializeField] private SystemTextViewer systemTextViewer; //돈 부족, 건설 불가와 같은 시스템 메시지 출력
    private bool isOnTowerButton = false; //타워건설 버튼을 눌렀는지 체크
    private GameObject followTowerClone = null;//임시 타워 사용 완료 시 삭제를 위해 저장하는 변수
    private int towerType;//타워 속성

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;
        if (isOnTowerButton == true)//버튼 중복 누름 방지
        {
            return;
        }
        
        //타워 건설 가능 여부 확인
        //타워를 건설할 만큼 돈이 없으면 타워 건설 실패
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            //돈이 없어서 타워 건설 불가능한.
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }
        //타워 건설 버튼을 눌렀다고 설정
        isOnTowerButton = true;
        
        //마우스를 따라다니는 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        
        //타워 건설을 취소할 수 있는 코루틴 함수 시작
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tileTransform)
    {
        if (isOnTowerButton == false)//타워 건설 버튼이 안눌렸다면 리턴
        {
            return;
        }
        
        Tile tile = tileTransform.GetComponent<Tile>();

        if (tile.IsBuildTower==true)//타워 이미 건설된 경우
        {
            //현재 위치에 타워 걸설이 불가능하다고 출력
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        
        //다시 타워 건설 버튼을 눌러서 타워를 건설하도록 변수 설정
        isOnTowerButton = false;
        
        tile.IsBuildTower = true;//타워 건설됨
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;//건설에 사용한 골드만큼 감소

        Vector3 position = tileTransform.position + Vector3.back;//선택한 타일의 위치에 타워 건설, 타워보다 z축 -1의 위치에 배치
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);//선택한 타일의 위치에 타워 건설
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner, playerGold, tile);//타워무기에 enemySpawner 정보 전달
        
        //타워를 배치했기 때문에 마우스를 따라다니는 임시 타워 삭제
        Destroy(followTowerClone);
        
        //타워건설을 취소할 수 잇는 코루틴 함수 중지
        StopCoroutine("OnTowerCancelSystem");
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))//타워 건설 취소
            {
                isOnTowerButton = false;
                Destroy(followTowerClone);//마우스 따라다니는 임시 타워 삭제
                break;
            }
            
            yield return null;
        }
        
    }



}
