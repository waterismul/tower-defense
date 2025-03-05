using TMPro;
using UnityEngine;

public class TextTMPViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPlayerHP; //플레이어 체력
    [SerializeField] private TextMeshProUGUI textPlayerGold;//플레이어 골드
    [SerializeField] private PlayerHP playerHP; //플레이어 체력 정보
    [SerializeField] private PlayerGold playerGold;//플레이어 골드 정보

    private void Update()
    {
        textPlayerHP.text = playerHP.CurrentHP+"/"+playerHP.MaxHP;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
    }
}
