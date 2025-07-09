using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private void LateUpdate()
    {
        if (UnitManager.Instance.GetPlayer() != null)
        {
            Vector3 offsetPosition = UnitManager.Instance.GetPlayer().transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, offsetPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
    }
}
