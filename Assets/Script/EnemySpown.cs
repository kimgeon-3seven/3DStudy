using UnityEngine;
using System.Collections;
using UnityEngine.Pool; // 풀링 네임스페이스 추가

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPlanes;
    public float spawnInterval = 3f;
    public int maxEnemies = 20;

    // 오브젝트 풀 선언
    private IObjectPool<GameObject> _enemyPool;
    private WaitForSeconds _spawnDelay;

    private void Awake()
    {
        // 1. 적 오브젝트 풀 세팅 (TurretShooter.cs와 동일한 방식)
        _enemyPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject newEnemy = Instantiate(enemyPrefab);
                if (newEnemy.TryGetComponent<EnemyHit>(out EnemyHit hitScript))
                {
                    hitScript.SetPool(_enemyPool); // EnemyHit 스크립트에 풀 참조 전달
                }
                return newEnemy;
            },
        actionOnGet: enemy => enemy.SetActive(true),
            actionOnRelease: enemy => enemy.SetActive(false),
            actionOnDestroy: enemy => Destroy(enemy),
            maxSize: maxEnemies // 맵 최대 적 수 
        );
    }

    private void Start()
    {
        _spawnDelay = new WaitForSeconds(spawnInterval);
        StartCoroutine(SpawnWaveRoutine());
    }

    private IEnumerator SpawnWaveRoutine()
    {
        while (true)
        {
            yield return _spawnDelay;

            if (_enemyPool.CountInactive < maxEnemies)
            {
                int randomIndex = Random.Range(0, spawnPlanes.Length);
                Transform selectedSpawnPlane = spawnPlanes[randomIndex];


                // 풀에서 꺼내오는 과정이 포함된 연출 코루틴 실행
                StartCoroutine(SpawnSequenceRoutine(selectedSpawnPlane));
            }
        }
    }

    private IEnumerator SpawnSequenceRoutine(Transform spawnPlane)
    {
        // --- [연출 1단계: 경고 (Warning)] ---
        Renderer planeRenderer = spawnPlane.GetComponent<Renderer>();
        Color originalColor = planeRenderer.material.color;

        planeRenderer.material.color = Color.red;
        yield return new WaitForSeconds(1.0f);
        planeRenderer.material.color = originalColor;

        // --- [연출 2단계: 풀링된 객체 가져오기 및 초기화] ---
        // Instantiate 대신 풀에서 객체를 꺼내옴
        GameObject enemy = _enemyPool.Get();

        // 위치 설정
        Vector3 spawnPos = spawnPlane.position + Vector3.up * 1f;
        enemy.transform.position = spawnPos;
        enemy.transform.rotation = spawnPlane.rotation;

        // [핵심] 재사용된 객체이므로 코루틴 시작 전 스케일을 0으로 확실히 덮어씌움!
        enemy.transform.localScale = Vector3.zero;

        // --- [연출 3단계: 스케일 업 (Scale Up)] ---
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            enemy.transform.localScale = Vector3.one * progress;
            yield return null;
        }

        enemy.transform.localScale = Vector3.one;
    }
}
