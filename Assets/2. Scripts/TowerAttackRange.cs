using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{

    public void OnAttackRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);
        float diameter = range * 2.0f;//공격 범위 크기, 지름
        transform.localScale = Vector3.one * diameter;
        transform.position = position;//공격 범위 위치
    }

    public void OffAttackRange()
    {
        gameObject.SetActive(false);
    }
    
}
