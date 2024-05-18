using System;
using System.Linq;
using UnityEngine;

public class DroneMovement2 : MonoBehaviour
{
    public GameObject[] propellers;
    private Vector3 moment;
    public Vector3 angVel;
    public Vector3 latVel;
    public float angDrag;
    public float latDrag;
    public float mass;

    void Start()
    {
        var size = GetComponent<MeshRenderer>().bounds.size;
        moment.x = 1 / 12f * mass * (size.y * size.y + size.z * size.z);
        moment.y = 1 / 12f * mass * (size.x * size.x + size.z * size.z);
        moment.z = 1 / 12f * mass * (size.x * size.x + size.y * size.y);
        
        foreach (var prop in propellers)
        {
            var propeller = prop.GetComponent<PropellerMovement>();
            if (propeller == null)
                continue;
            
            var dist = propeller.transform.localPosition;
            moment.y += propeller.moment + propeller.mass * (dist.x * dist.x + dist.z * dist.z);
        }
    }
    float GetMotorTorque()
    {
        return propellers.Sum(prop => prop.GetComponent<PropellerMovement>().GetMagForce() * prop.transform.localPosition.magnitude);
    }
    
    void Update()
    {
        var torqueY = GetMotorTorque();

        var torque = new Vector3();
        var liftForce = new Vector3();
        
        foreach (var prop in propellers)
        {
            var force = prop.GetComponent<PropellerMovement>().GetLiftForce();
            torque += Vector3.Cross(force, (prop.transform.position - transform.position));
            liftForce += force;
        }

        liftForce -= latDrag * latVel.magnitude * latVel.magnitude * latVel;
        
        if (transform.position.y < 0.25f)
        {
            latVel.y = 0;
            liftForce.y = Mathf.Max(liftForce.y, 0);
        }
        else
            liftForce.y -= 9.81f;

        var torqueX = Vector3.Dot(torque, transform.right);
        var torqueZ = Vector3.Dot(torque, transform.forward);

        torqueX += angDrag * angVel.x * angVel.x * Math.Sign(angVel.x);
        torqueY += angDrag * angVel.y * angVel.y * Math.Sign(angVel.y);
        torqueZ += angDrag * angVel.z * angVel.z * Math.Sign(angVel.z);
        
        var angAccX = torqueX / moment.x;
        var angAccY = torqueY / moment.y;
        var angAccZ = torqueZ / moment.z;

        angVel += new Vector3(angAccX, angAccY, angAccZ) * -Time.deltaTime;
        latVel += liftForce * Time.deltaTime;

        transform.position += latVel * Time.deltaTime;
        transform.Rotate(angVel.x * Time.deltaTime, angVel.y * Time.deltaTime, angVel.z * Time.deltaTime, Space.Self);
    }
}