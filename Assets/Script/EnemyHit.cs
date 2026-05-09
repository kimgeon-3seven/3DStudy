using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyHit : MonoBehaviour
{
    [SerializeField] private Renderer enemyRenderer;
    [SerializeField] private Color hitColor = Color.blue;

    private bool isHit = false;
    private Color originalColor;

    private IObjectPool<GameObject> _enemyPool;

    private void Awake()
    {
        // Start에서 Awake로 변경: 풀에서 꺼내질 때마다 Start는 호출되지 않으므로, Awake에서 초기화하여 원래 색상을 저장
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    
    private void OnEnable()
    {
        ResetEnemyState();
    }

    // 💡 오브젝트가 비활성화될 때(풀로 돌아갈 때) 호출됨
    private void OnDisable()
    {
        // 혹시 실행 중일지 모르는 코루틴을 안전하게 중단
        StopAllCoroutines();
    }

    //상태를 처음처럼 되돌리는 전용 함수
    private void ResetEnemyState()
    {
        isHit = false;
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        _enemyPool = pool;
    }

    public void TriggerHitEffect()
    {
        // 이미 맞고 있는 상태이거나, 오브젝트가 꺼져있다면 실행 안 함
        if (isHit || !gameObject.activeInHierarchy) return;

        StartCoroutine(HitEffect());
    }

    public IEnumerator HitEffect()
    {
        isHit = true;

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = hitColor;
            yield return new WaitForSeconds(0.2f);
            enemyRenderer.material.color = originalColor;
        }

        isHit = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!gameObject.activeInHierarchy) return;

        if (other.CompareTag("Turret"))
        {
            if (_enemyPool != null)
            {
                _enemyPool.Release(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
            return;
        }

        if (other.CompareTag("Projectile"))
        {
            TriggerHitEffect();

            Projectile projectileScript = other.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.ReturnToPool();
            }
        }
    }
}