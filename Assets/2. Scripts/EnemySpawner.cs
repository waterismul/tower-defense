using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    
    [SerializeField] private GameObject enemyHPSliderPrefab;//적 체력을 나타내는 Silder UI 프리팹
    [SerializeField] private Transform canvasTransform;//UI를 표현하는 canvas 오브젝트의 Transform
    [SerializeField] private Transform[] wayPoints;//현재 스테이지의 이동경로
    [SerializeField] private PlayerHP playerHP;//플레이어 체력 컴포넌트
    [SerializeField] private PlayerGold playerGold;//플레이어의 골드 컴포넌트
    private Wave currentWave; //현재 웨이브 정보, WaveSystem에 있는 Struct를 그냥 저렇게 가져와서 사용할 수 있음![Serializable]해서
    private int currentEnemyCount;//현재 웨이브에 남아있는 적 숫자(웨이브 시작시 max로 설정, 적 사망 시 -1)
    private List<Enemy> enemyList;//맵에 존재하는 모든 적의 정보

    public List<Enemy> EnemyList => enemyList;
    public int CurrentEnemyCount => currentEnemyCount;//현재 웨이브에 남아있는 적, 최대 적 숫자
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        enemyList = new List<Enemy>();
    }

    public void StartWave(Wave wave)
    {
        currentWave = wave;
        currentEnemyCount = currentWave.maxEnemyCount;//현재 웨이브의 최대 적 숫자 저장
        StartCoroutine("SpawnEnemy");//현재 웨이브 시작
    }

    private IEnumerator SpawnEnemy()
    {
        int spawnEnemyCount = 0;
        while (spawnEnemyCount<currentWave.maxEnemyCount)//생성한 적의 숫자가 맥스면 종료
        {
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);//적 랜덤생성
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();//방금 생성된 적의 Enemy 컴포넌트

            enemy.Setup(this, wayPoints);//wayPoint 정보를 매개변수로 Setup() 호출
            enemyList.Add(enemy);//리스트에 적 저장

            SpawnEnemyHPSlider(clone);

            spawnEnemyCount++;//현재 웨이브에서 생성한 적의 숫자+1
            
            yield return new WaitForSeconds(currentWave.spawnTime);//spawnTime 시간동안 대기
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

        currentEnemyCount--;//적이 사망할때 마다 현재 웨이브의 생존 적 숫자 감소
        
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
