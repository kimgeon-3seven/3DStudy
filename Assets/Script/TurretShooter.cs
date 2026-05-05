using UnityEngine;

public class TurretShooter : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Transform muzzlePoint; // 포신 끝부분
    public GameObject projectilePrefab; // 발사체 프리팹

    [Header("Shooting Settings")]
    public float fireAngleThreshold = 5f; // 이 각도 이내로 조준해야 발사
    public float fireInterval = 0.5f; // 발사 간격

    private float nextFireTime = 0f;

    private void Update()
    {
        if (target == null || muzzlePoint == null) return;

        // 포신 끝(muzzlePoint)에서 타겟을 향하는 방향 벡터
        Vector3 directionToTarget = (target.position - muzzlePoint.position).normalized;

        // 포신이 바라보는 앞쪽(forward)과 타겟 방향 사이의 각도 계산
        float angleToTarget = Vector3.Angle(muzzlePoint.forward, directionToTarget);

        // 타겟이 조준 허용 각도 안에 들어왔는지 확인
        if (angleToTarget <= fireAngleThreshold)
        {
            // 발사 쿨타임이 지났다면 사격
            if (Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + fireInterval;
            }
        }
    }

    private void Fire()
    {
        // 포신 끝에서 발사체 생성
        Instantiate(projectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
    }
}
