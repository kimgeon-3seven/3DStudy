using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TurretYawPivot : MonoBehaviour
{ 

    [Header("Yaw rotation Speed")]
    public Vector3 yawRotationSpeed = new Vector3(0f, 90f, 0f);

    [Header("Target")]
    public Transform target;

    private void Update()
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.localRotation = Quaternion.Slerp(transform.rotation, targetRotation, yawRotationSpeed.y * Time.deltaTime);
    }

}
