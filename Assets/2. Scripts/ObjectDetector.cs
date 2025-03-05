using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();와 동일
        mainCamera = Camera.main;
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            //ray.origin: 광선의 시작 위치(=카메라위치)
            //ray.direction: 광선의 진행방향
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        }

        //2D모니터를 통해 3D월드의 오브젝트를 마우스로 선택하는 방법
        //광선에 부딪히는 오브젝트를 검출해서 hit에 저장
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.transform.CompareTag("Tile"))
            {
                towerSpawner.SpawnTower(hit.transform);//타워생성
            }
        }
    }
}
