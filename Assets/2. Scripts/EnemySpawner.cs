using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; //적프리팹
    [SerializeField] private float spawnTime;//적 생성추가
    [SerializeField] private Transform[] wayPoints;//현재 스테이지의 이동경로
    private List<Enemy> enemyList;//맵에 존재하는 모든 적의 정보

    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("SpawnEnemy");
        
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);//적 오브젝트 생성
            Enemy enemy = clone.GetComponent<Enemy>();//방금 생성된 적의 Enemy 컴포넌트

            enemy.Setup(this, wayPoints);//wayPoint 정보를 매개변수로 Setup() 호출
            enemyList.Add(enemy);
            
            yield return new WaitForSeconds(spawnTime);//spawnTime 시간동안 대기
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        //리스트에서 사망하는 적 정보 삭제
        enemyList.Remove(enemy);
        //적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }
}
