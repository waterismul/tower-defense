using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab; //적프리팹
    [SerializeField] private GameObject enemyHPSliderPrefab;//적 체력을 나타내는 Silder UI 프리팹
    [SerializeField] private Transform canvasTransform;//UI를 표현하는 canvas 오브젝트의 Transform
    [SerializeField] private float spawnTime;//적 생성추가
    [SerializeField] private Transform[] wayPoints;//현재 스테이지의 이동경로
    [SerializeField] private PlayerHP playerHP;//플레이어 체력 컴포넌트
    [SerializeField] private PlayerGold playerGold;//플레이어의 골드 컴포넌트
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

            SpawnEnemyHPSlider(clone);
            
            yield return new WaitForSeconds(spawnTime);//spawnTime 시간동안 대기
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        //적이 목표지점에 도착했을 때
        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);//플레이어 체력 -1
        }
        
        //적이 플레이어의 발사체에게 사망했을 때
        else if (type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;//적 종류에 따라 사망시 골드 획득
        }
        
        //리스트에서 사망하는 적 정보 삭제
        enemyList.Remove(enemy);
        //적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        //적 체력을 나타내는 Slider UI생성
        GameObject sliderColne = Instantiate(enemyHPSliderPrefab);
        
        //Silder UI 오브젝트를 Canvas의 자식으로 설정
        sliderColne.transform.SetParent(canvasTransform);
        
        //계층 설정으로 바뀐 크기를 다시 설정
        sliderColne.transform.localScale = Vector3.one;
        
        //Silder UI가 쫒아다닐 대상을 본인으로 설정
        sliderColne.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        
        //Silder UI에 자신의 체력 정보를 표시하도록 설정
        sliderColne.GetComponent<EnemyHPViewer>().Setup(enemy.GetComponent<EnemyHP>());
    }
}
