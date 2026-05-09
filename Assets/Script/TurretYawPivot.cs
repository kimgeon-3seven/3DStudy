using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretYawPivot : MonoBehaviour
{ 

    [Header("Yaw rotation Speed")]
    public Vector3 yawRotationSpeed = new Vector3(0f, 90f, 0f);

    [Header("Target")]
    public TurretShooter myShooter;

    public Transform target;

    private void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            if (myShooter != null)
            {
                target = myShooter.target;
            }
            if (target == null) return; // 타겟이 여전히 없으면 종료
        }

            Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, yawRotationSpeed.y * Time.deltaTime);
    }

}
