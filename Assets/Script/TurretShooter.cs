using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;

public class TurretShooter : MonoBehaviour
{
    [SerializeField] private Renderer turretRenderer; // 터렛의 렌더러 (발사 시 색상 변경용)
    [SerializeField] private float ShootCooldown = 1f; // 발사 후 재발사까지의 대기 시간
    [SerializeField] private float shootRange = 10f; // 발사 범위
    [SerializeField] private float shootAngleThreshold = 5f; // 발사 허용 각도
    [SerializeField] private int burstBullet = 3; // 연발 총알 수
    [SerializeField] private float burstInterval = 0.1f; // 연발 간격
    [SerializeField] private TurretRecoilCoroutine recoilScript; // 반동 코루틴 참조



    [Header("References")]
    public Transform muzzlePoint; // 포신 끝부분
    public Projectile projectilePrefab; // 발사체 프리펩
    public Transform target; // 타겟 위치 (예: 플레이어)

    private IObjectPool<Projectile> projectilePool;
    private bool canShoot = true;
    private Color originalColor;

    [Header("Shooting Settings")]
    public float fireAngleThreshold = 5f; // 이 각도 이내로 조준해야 발사
    public float fireInterval = 0.5f; // 발사 간격

    #region 기즈모
    private void OnDrawGizmos()
    {
        if (muzzlePoint == null) return;
        //기즈모로 포신 끝에서 앞쪽으로 레이 그려서 발사 방향 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawRay(muzzlePoint.position, muzzlePoint.forward * 10f);
    }
    #endregion

    #region 발사체 풀링 시스템
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
    #endregion

    private void Start()
    {
        originalColor = turretRenderer.material.color;
    }

    #region 포신 각도 조준 시스템
    private void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindTarget();
            if(target == null)
            {
                return; // 타겟이 없으면 발사하지 않음
            }
        }

        //포신 끝(muzzlePoint)에서 타겟을 향하는 방향 벡터
        Vector3 directionToTarget = (target.position - muzzlePoint.position).normalized;
        // 포신이 바라보는 앞쪽(forward)과 타겟 방향 사이의 각도 계산
        float angleToTarget = Vector3.Angle(muzzlePoint.forward, directionToTarget);

        if (canShoot && angleToTarget <= fireAngleThreshold && Vector3.Distance(muzzlePoint.position, target.position) <= shootRange)
        {
            StartCoroutine(ShootingRoutine());
        }
    }
    #endregion

    private void FindTarget()
    {
        //씬에서 Enemy 태그가 붙은 오브젝트를 찾아 타겟으로 설정
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeInHierarchy)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }

            }
        }
        target =  closestEnemy;
    }

    #region 재장전 및 발사 연출 시스템
    private IEnumerator ShootingRoutine()
    {
        canShoot = false;

        for (int i = 0; i < burstBullet; i++)
        {
            Fire();
            yield return new WaitForSeconds(burstInterval);
        }

        if (turretRenderer != null)
        {
            turretRenderer.material.color = Color.red; // 발사 시 빨간색으로 변경
        }


        yield return new WaitForSeconds(fireInterval);

        if(turretRenderer != null)
        {
            turretRenderer.material.color = originalColor; // 원래 색상으로 복원
        }

        canShoot = true;
    }
    #endregion




    private Projectile CreateProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab,this.transform);
        newProjectile.SetManagedPool(projectilePool); // 창고 주소 설정
        return newProjectile;
    }   

    private void Fire()
    {
        // Pool에서 총알 꺼내오기 
        var projectile = projectilePool.Get();

        projectile.transform.SetParent(null);

        // 꺼내온 총알을 포신 끝 위치와 회전으로 설정
        projectile.transform.position = muzzlePoint.position;
        projectile.transform.rotation = muzzlePoint.rotation;

        // 반동 코루틴 실행
        if (recoilScript != null)
        {
            recoilScript.TriggerRecoil();
        }
    }
}
