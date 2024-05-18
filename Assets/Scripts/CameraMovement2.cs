using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement2 : MonoBehaviour
{
    public GameObject target;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Transform targetTransform = target.transform;
        Vector3 originPos = transform.position;
        Vector3 dir = targetTransform.forward;
        dir.y = 0;
        if (Vector3.Dot(targetTransform.forward, Vector3.forward) < 0 && Vector3.Dot(targetTransform.up, Vector3.down) > 0)
        {
            dir.z = -dir.z;
            Debug.Log("switch");
        }
        Vector3 targetPos = targetTransform.position - 20 * dir.normalized + new Vector3(0, 4, 0);

        transform.position = targetPos;

        Quaternion rotation = Quaternion.LookRotation(targetTransform.position - originPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10); 
    }
}