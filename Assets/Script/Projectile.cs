using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 12f;
    public float projectileLifeTime = 3f;

    public float _currentLifeTime = 0f;

    
    private IObjectPool<Projectile> _managedPool;


    public void SetManagedPool(IObjectPool<Projectile> pool)
    {
        _managedPool = pool;
    }

    private void OnEnable()
    {
        _currentLifeTime = 0f; // 활성화될 때마다 생명 시간 초기화
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);

        _currentLifeTime += Time.deltaTime;
        if (_currentLifeTime >= projectileLifeTime)
        {
            ReturnToPool();
        }
           
    }

    public void ReturnToPool()
    {
        // Destroy 대신 창고로 반납
        if (_managedPool != null)
            _managedPool.Release(this);
        else
            Destroy(gameObject);
    }

}