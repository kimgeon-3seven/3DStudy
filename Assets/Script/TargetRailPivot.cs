using UnityEngine;

public class TargetRailPivot : MonoBehaviour
{
    [Header("Target Speed")]
    public Vector3 targeSpeed = new Vector3(0f, 90f, 0f);

    [Header("Rotation Space")]
    public Space rotationSpace = Space.Self;

    private void Update()
    { 

     transform.Rotate(targeSpeed * Time.deltaTime, rotationSpace);

    }
}
