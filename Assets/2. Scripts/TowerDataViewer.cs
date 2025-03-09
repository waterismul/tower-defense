using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TowerDataViewer : MonoBehaviour
{
    [SerializeField] private Image imageTower;
    [SerializeField] private TextMeshProUGUI textDamage;
    [SerializeField] private TextMeshProUGUI textRate;
    [SerializeField] private TextMeshProUGUI textRange;
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private TowerAttackRange towerAttackRange;
    [SerializeField] private Button buttonUpgrades;
    [SerializeField] private SystemTextViewer systemTextViewer;
    private TowerWeapon currentTower;
    private void Awake()
    {
        OffPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    public void OnPanel(Transform towerWeapon)//타워 정보 Panel on
    {
        currentTower = towerWeapon.GetComponent<TowerWeapon>();//출력해야하는 타워 정보를 받아와서 저장
        gameObject.SetActive(true);//타워 정보 Panel On
        UpdateTowerData();//타워 정보를 갱신
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);//타워 오브젝트 주변에 표시되는 타워 공격 범위
    }

    public void OffPanel()//타워 정보 Panel off
    {
        gameObject.SetActive(false);
    }

    private void UpdateTowerData()
    {
        imageTower.sprite = currentTower.TowerSprite;
        textDamage.text = "Damage: " + currentTower.Damage;
        textRate.text = "Rate: " + currentTower.Rate;
        textRange.text = "Range: " + currentTower.Range;
        textLevel.text = "Level: " + currentTower.Level;
        
        //업그레이드가 불가능해지면 버튼 비활성화, interactable
        buttonUpgrades.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    public void OnClickEventTowerUpgrade()
    {
        //타워 업그레이드 시도, 성공: true, 실패: false
        bool isSuccess = currentTower.Upgrade();
        if (isSuccess == true)
        {
            //타워 업그레이드 되었기 때문에 타워 정보 갱신
            UpdateTowerData();
        
            //타워 주변에 보이는 공격범위도 갱신
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            //타워 업그레이드에 필요한 비용이 부족하다고 출력
            systemTextViewer.PrintText(SystemType.Money);
        }
    }

    public void OnClickEventTowerSell()
    {
        //타워 판매
        currentTower.Sell();
        
        //선택한 타워가 사라져서 Panel, 공격범위 Off
        OffPanel();
    }
}
