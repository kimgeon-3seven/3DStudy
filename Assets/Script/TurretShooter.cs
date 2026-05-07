using UnityEngine;
using UnityEngine.Pool;

public class TurretShooter : MonoBehaviour
{
    [Header("References")]
    public Transform muzzlePoint; // 포신 끝부분
    public Projectile projectilePrefab; // 발사체 프리팹
    public Transform target; // 타겟

    private IObjectPool<Projectile> projectilePool;

    [Header("Shooting Settings")]
    public float fireAngleThreshold = 5f; // 이 각도 이내로 조준해야 발사
    public float fireInterval = 0.5f; // 발사 간격

    private void OnDrawGizmos()
    {
        if (muzzlePoint == null) return;
        //기즈모로 포신 끝에서 앞쪽으로 레이 그려서 발사 방향 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawRay(muzzlePoint.position, muzzlePoint.forward * 2f);
    }

    private float nextFireTime = 0f;

    private void Awake()
    {
        //발사체 생성 
        projectilePool = new ObjectPool<Projectile>(
            createFunc: CreateProjectile,
            actionOnGet: projectile => projectile.gameObject.SetActive(true),
            actionOnRelease: p => p.gameObject.SetActive(false),
            actionOnDestroy: p => Destroy(p.gameObject),
            maxSize: 30

        );
    }

    private Projectile CreateProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab,this.transform);
        newProjectile.SetManagedPool(projectilePool); // 창고 주소 설정
        return newProjectile;
    }   

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
        // Pool에서 총알 꺼내오기 
        var projectile = projectilePool.Get();

        projectile.transform.SetParent(null);

        // 꺼내온 총알을 포신 끝 위치와 회전으로 설정
        projectile.transform.position = muzzlePoint.position;
        projectile.transform.rotation = muzzlePoint.rotation;
    }
}
