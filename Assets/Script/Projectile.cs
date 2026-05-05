using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 12f;
    public float projectileLifeTime = 3f;

    private void Start()
    {
        // 생성된 후 projectileLifeTime 초가 지나면 자기 자신을 파괴합니다.
        Destroy(gameObject, projectileLifeTime);
    }

    private void Update()
    {
        // Z축(앞) 방향으로 매 프레임 이동합니다.
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
    }
}