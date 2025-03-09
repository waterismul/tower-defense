using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.0f;
    [SerializeField] private Vector3 moveDir = Vector3.zero;

    public float MoveSpeed => moveSpeed;//왜 굳이 프로퍼티를 써야하는가-> SerializeField는 값만 수정될 뿐 보호해주는 건 아님, 읽기 전용으로

    private void Update()
    {
        transform.position+=moveDir*moveSpeed*Time.deltaTime;
        
    }

    public void MoveTo(Vector3 direction)
    {
        moveDir = direction;
    }
}
