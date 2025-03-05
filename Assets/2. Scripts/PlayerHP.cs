﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private Image imageScreen;//전체화면을 덮는 빨간색 이미지
    [SerializeField] private float maxHP = 20;//최대 체력
    private float currentHP;//현대 체력

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;//현재 체력을 최대 체력과 같게 설정
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;//현재 체력을 damage만큼 감소
        
        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHP <= 0)//체력이 0이되면 게임오버
        {
            
        }
    }

    private IEnumerator HitAlphaAnimation() //플레이어의 체력이 깎일때 마다 
    {
        Color color = imageScreen.color;
        color.a = 0.4f;
        imageScreen.color = color;

        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;
            
            yield return null;
        }
    }
}
