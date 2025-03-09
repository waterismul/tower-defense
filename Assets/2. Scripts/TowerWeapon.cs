using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public enum WeaponType{Cannon, Laser}
public enum WeaponState{SearchTarget, TryAttackCannon, TryAttackLaser}
public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    [SerializeField] private TowerTemplate towerTemplate;//타워 정보
    [SerializeField] private Transform spawnPoint;//발사체 생성위치
    [SerializeField] private WeaponType weaponType;// 무기속성 설정
    
    [Header("Cannon")]
    [SerializeField] private GameObject projectilePrefab;// 발사체 프리팹
    
    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer; //레이저로 사용되는 선
    [SerializeField] private Transform hitEffect;//타격효과
    [SerializeField] private LayerMask targetLayer; //광선에 부딛히는 레이어 설정
    
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

    private Transform FindClosesAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;

        for (int i = 0; i < enemySpawner.EnemyList.Count; ++i)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            if (distance <= towerTemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        if (attackTarget == null)//target이 있는지 검사
        {
            return false;
        }
        
        //target이 공격 범위 안에 있는지 검사
        float distance=Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
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
            attackTarget = FindClosesAttackTarget();//현재 타워에 가장 가까이 있는 공격 대상 탐색
            if (attackTarget != null)
            {
                if (weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);//해당타겟 공격
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);//해당 타겟 공격
                }
                
            }

            yield return null;
        }
        
    }
    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);//시간만큼 대기
            
            SpawnProjectile();//발사체 생성하여 공격
        }
        
    }

    private IEnumerator TryAttackLaser()
    {
        EnableLaser();//레이저 활성화
        
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                DisableLaser();//레이저 비활성화
                ChangeState(WeaponState.SearchTarget);
                break;
            }
            SpawnLaser();//레이저 공격
            yield return null;
        }
        
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - spawnPoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range,
            targetLayer);

        for (int i = 0; i < hit.Length; ++i)//같은 방향으로 광산 여러개 쏴서 그 중 현재 attackTarget과 동일한 오브젝틀르 검출
        {
            if (hit[i].transform == attackTarget)
            {
                //선의 시작지점
                lineRenderer.SetPosition(0,spawnPoint.position);
                
                //선의 목표지점
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                
                //타격효과위치 설정
                hitEffect.position = hit[i].point;
                
                //적 체력 감소 (1초에 damage만큼 감소)
                attackTarget.GetComponent<EnemyHP>().TakeDamage(towerTemplate.weapon[level].damage*Time.deltaTime);
            }
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
        
        //무기 속성이 레이저이면
        if (weaponType == WeaponType.Laser)
        {
            //레벨에 따라 레이저 굵기 설정
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth =  0.05f;
        }

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
