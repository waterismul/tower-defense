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
        textDamage.text = "Damage: " + currentTower.Damage;
        textRate.text = "Rate: " + currentTower.Rate;
        textRange.text = "Range: " + currentTower.Range;
        textLevel.text = "Level: " + currentTower.Level;
        
    }
}
