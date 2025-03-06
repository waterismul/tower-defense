﻿using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public enum WeaponState{SearchTarget, AttackToTarget}
public class TowerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;// 발사체 프리팹
    [SerializeField] private Transform spawnPoint;//발사체 생성위치
    [SerializeField] private float attackRate = 0.5f;//공격속도
    [SerializeField] private float attackRange = 2.0f;//공격범위
    [SerializeField] private int attackDamage = 1;//공격력
    private int level = 0;//타워 레벨
    private WeaponState weaponState = WeaponState.SearchTarget;//타워 무기 상태
    private Transform attackTarget = null;//공격대상
    private EnemySpawner enemySpawner;//게임에 존재하는 적 정보 획득용

    public float Damage => attackDamage;
    public float Rate => attackRate;
    public float Range => attackRange;
    public int Level => level+1;
    
    

    public void Setup(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;
        
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
                if (distance <= attackRange && distance <= closestDistSqr)
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
            if (distance > attackRange)//공격 범위내에 있다면
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(attackRate);//시간만큼 대기
            
            SpawnProjectile();//발사체 생성하여 공격
        }
        
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);//생성된 발사체에게 공격대상 정보 제공
    }
}
