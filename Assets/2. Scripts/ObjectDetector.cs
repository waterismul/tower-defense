using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;
    [SerializeField] private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null; //마우스 픽킹으로 선택한 오브젝트 임시 저장

    private void Awake()
    {
        //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();와 동일
        mainCamera = Camera.main;
        
    }

    private void Update()
    {
        //마우스가 UI에 머물러 있을 때는 아래 코드가 실행되지 않도록 함
        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }
        
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
            hitTransform = hit.transform;
            if (hit.transform.CompareTag("Tile"))
            {
                towerSpawner.SpawnTower(hit.transform);//타워생성
            }
            else if (hit.transform.CompareTag("Tower"))
            {
                towerDataViewer.OnPanel(hit.transform);
            }
        }
        
        else if(Input.GetMouseButtonUp(0)){
            
            //마우스를 눌렀을 때 선택한 오브젝트가 없거나 선택한 오브젝트가 타워가 아니면
            if (hitTransform == null || hitTransform.CompareTag("Tower")==false)
            {
                //타워 정보 패널을 비활성화 한다
                towerDataViewer.OffPanel();
            }

            hitTransform = null;

        }
    }
    
}
