using System.Collections;
using UnityEngine;

public class TurretRecoilCoroutine : MonoBehaviour
{
    [SerializeField] private float recoilDistance = 0.5f; // 반동 거리
    [SerializeField] private float recoilDuration = 0.3f; // 반동 지속 시간

    private bool isRecoiling = false; // 반동 중인지 여부
    private Vector3 originalPosition; // 원래 위치 저장

    public void Start()
    {
        originalPosition = transform.localPosition; // 시작 시 원래 위치 저장
    }
    public void TriggerRecoil()
    {
        if (!isRecoiling)
        {
            StartCoroutine(RecoilRoutine());
        }
    }
    private IEnumerator RecoilRoutine()
    {
        isRecoiling = true;
        
        
        Vector3 recoilPosition = originalPosition + Vector3.back * recoilDistance; // 반동 위치 계산
        transform.localPosition = recoilPosition; 
        
        float elapsedTime = 0f;
       
        while (elapsedTime < recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(recoilPosition,originalPosition, elapsedTime / recoilDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 반동이 끝난 후 원래 위치로 돌아감
        transform.localPosition = originalPosition;
        isRecoiling = false;
    }
}
