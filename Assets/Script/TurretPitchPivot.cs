using UnityEngine;

public class TurretPitchPivot : MonoBehaviour
{
    [Header("Pitch Rotation Speed")]
    public float pitchSpeed = 90f; 

    [Header("Pitch Limits")]
    public float minPitch = -45f;
    public float maxPitch = 20f;

    [Header("Target")]
    public TurretShooter myShooter;

    public Transform target;

    private float currentPitch = 0f;

    private void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
           if(myShooter != null)
            {
                target = myShooter.target;
            }
            if (target == null) return; // 타겟이 여전히 없으면 종료
        }

        
        Vector3 localTargetPos = transform.parent.InverseTransformPoint(target.position);

       
        float targetPitch = -Mathf.Atan2(localTargetPos.y, localTargetPos.z) * Mathf.Rad2Deg;

       
        currentPitch = Mathf.MoveTowards(currentPitch, targetPitch, pitchSpeed * Time.deltaTime);

        
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        
        transform.localEulerAngles = new Vector3(currentPitch, 0f, 0f);
    }
}