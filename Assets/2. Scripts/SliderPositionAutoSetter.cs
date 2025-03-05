using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField] private Vector3 distance = Vector3.down * 20.0f;
    private Transform targetTransform;
    private RectTransform rectTransform;

    public void Setup(Transform target)
    {
        targetTransform = target;//Silder Ui가 쫒아다딜 target설정
        rectTransform = GetComponent<RectTransform>();//위치 정보 가져오기
    }

    private void LateUpdate()
    {
        //적이파괴되어 쫒아다닐 대상이 사라지면 Silder UI도 삭제
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }
        
        //오브젝트의 위치가 갱신된 이후에 Slider UI도 함께ㅐ 위치를 설정하도록 하기위해 LateUpdate()에서 호출한다
        
        //오브젝트의 월드 좌표를 기준으로 화면에서의 좌표 값을 구함
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        
        //화면 내에서 좌표 + distance만큼 떨어진 위치를 Silder UI의 위치로 설정
        rectTransform.position = screenPosition + distance;
    }
}
