using UnityEngine;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    [SerializeField ] private float moveSpeed = 2f; // 적의 이동 속도
    [SerializeField] private Transform turret; // 터렛의 위치
    [SerializeField] private float stopDistance = 1.5f; // 터렛과의 최소 거리


    private void OnEnable()
    {
        // 1. 만약 터렛 변수가 비어있다면 스스로 찾습니다.
        if (turret == null)
        {
            // 씬에서 TurretShooter 스크립트가 붙은 오브젝트를 찾아 그 위치(transform)를 저장합니다.
            turret = FindAnyObjectByType<TurretShooter>().transform;
        }

        // 2. 터렛을 향해 걷는 코루틴 실행
        StartCoroutine(MoveTowardsTurret());
    }

    private IEnumerator MoveTowardsTurret()
    {
        // 터렛과의 거리가 stopDistance(3f)보다 먼 동안만 반복해서 다가감
        while (Vector3.Distance(transform.position, turret.position) > stopDistance)
        {
            // 내 위치에서 터렛 위치를 향해 moveSpeed 속도로 이동
            transform.position = Vector3.MoveTowards(transform.position, turret.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
   
    }
}
