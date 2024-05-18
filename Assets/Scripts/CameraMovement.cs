using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject target;
    
    public float attraction = 10f;
    public float damp = 0.99f;
    Vector3 velocity = Vector3.zero;
    Vector3 acceleration = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Transform targetTransform = target.transform;
        Vector3 originPos = transform.position;
        Vector3 dir = targetTransform.GetComponent<DroneMovement>().linVel;
        dir.y = 0;
        if (dir.magnitude == 0)
            dir = targetTransform.forward;
        dir.y = 0;
        Vector3 targetPos = targetTransform.position - 20 * dir.normalized + new Vector3(0, 4, 0);

        acceleration = (targetPos - originPos) * attraction;

        velocity = damp * velocity + acceleration * Time.deltaTime;
        originPos += velocity * Time.deltaTime;
        transform.position = originPos;

        Quaternion rotation = Quaternion.LookRotation(targetTransform.position - originPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10); 
    }
}
