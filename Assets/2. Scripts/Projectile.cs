using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;

    public void Setup(Transform target)
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;//타워가 설정해준 target
    }

    private void Update()
    {
        if (target != null)//target이 존재하면
        {
            Vector3 direction = (target.position-transform.position).normalized;//발사체를 target의 위치로 이동
            movement2D.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;//적이 아닌 대상과 부딪히면
        if (other.transform != target) return; //현재 적이 아니면
        
        other.GetComponent<Enemy>().OnDie();//적 사망 함수 호출
        Destroy(gameObject);//발사체 오브젝트 삭제

    }
}
