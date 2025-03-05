using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private float maxHP;//최대 체력
    private float currentHP;//현재 체력
    private bool isDie = false;//적이 사망상태면 isDie를 true설정
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;//현재 체력을 최대 체력과 같게 설정
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        //적의 체력이 damage만큼 감소해서 죽을 상황일 때 여러 타워의 공격을 동시에 받으면 OnDie함수가 여러번 실행될 수 있음
        if (isDie == true) return;//true면 사망

        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHP <= 0)
        {
            isDie = true;
            enemy.OnDie(EnemyDestroyType.Kill);
        }
        
    }
    
    
    private IEnumerator HitAlphaAnimation()
    {
        Color color = spriteRenderer.color; //적 색상 저장

        color.a = 0.4f;
        spriteRenderer.color = color;
            
        yield return new WaitForSeconds(0.05f);

        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
