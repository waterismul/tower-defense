using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public enum WeaponState{SearchTarget, AttackToTarget}
public class TowerWeapon : MonoBehaviour
{
    [SerializeField] private TowerTemplate towerTemplate;//타워 정보
    [SerializeField] private GameObject projectilePrefab;// 발사체 프리팹
    [SerializeField] private Transform spawnPoint;//발사체 생성위치
    private int level = 0;//타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget;//타워 무기 상태
    private Transform attackTarget = null;//공격대상
    private SpriteRenderer spriteRenderer;//타워 오브젝트 이미지 변경용
    private EnemySpawner enemySpawner;//게임에 존재하는 적 정보 획득용
    private PlayerGold playerGold;//플레이어의 골드 정보 획득 및 설정
    private Tile ownerTile;//현재 타워가 배치되어 있는 타일

    public Sprite TowerSprite=>towerTemplate.weapon[level].sprite;
    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level+1;
    public int MaxLevel => towerTemplate.weapon.Length;
    
    

    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;
        
        //최초상태를 WeaponState.SearchTarget으로 설정
        ChangeState(WeaponState.SearchTarget);
        
    }

    public void ChangeState(WeaponState newState)
    {
        StopCoroutine(weaponState.ToString());//이전 상태 종료
        weaponState = newState;//상태 변경
        StartCoroutine(weaponState.ToString());//현재 상태 실행
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()//적이 타겟을 바라보게
    {
        //원점으로부터의 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계이용
        //각도 = arctan(y/x)
        //x, y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        
        //x,y 변위값을 바탕으로 각도 구하기
        //각도가 radian 단위이기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()//적 찾기
    {
        while (true)
        {
            //제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
            float closestDistSqr = Mathf.Infinity;
            
            //EnemySpawner의 EnemyList에 있는 현재 맴에 존재하는 모든 적 검사
            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);//거리계산
                
                //현재 검사중인 적과의 거리가 공격 범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
                if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if (attackTarget != null)
            {
                ChangeState(WeaponState.AttackToTarget);//해당타겟 공격
            }

            yield return null;
        }
        
    }
    private IEnumerator AttackToTarget()
    {
        while (true)
        {
            if (attackTarget == null)//target있는지 체크
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > towerTemplate.weapon[level].range)//공격 범위내에 있다면
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);//시간만큼 대기
            
            SpawnProjectile();//발사체 생성하여 공격
        }
        
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);//생성된 발사체에게 공격대상 정보 제공
    }

    public bool Upgrade()
    {
        //타워 업그레이드에 필요한 골드가 충분한지 검사
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }

        //타워 레벨 증가
        level++;

        //타워 외형 변경
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;

        //골드차감
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        return true;
    }

    public void Sell()
    {
        //골드 증가
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        
        //현재 타일에 다시 타워 건설이 가능하도록 설정
        ownerTile.IsBuildTower = false;
        
        //타워 파괴
        Destroy(gameObject);
    }
}
