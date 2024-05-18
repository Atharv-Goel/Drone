using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PropellerMovement : MonoBehaviour
{
    public UDictionary<string, float> binds;
    
    public float motorSize;
    public float propSize;
    public float Cdrag;
    public float Clift;
    public float mass;
    public int direction;
    
    public float angVel;
    public float moment;

    void Start()
    {
        moment = mass * propSize * propSize / 12;
    }

    //Magnetic force within the motor
    public float GetMagForce()
    {
        var force = binds.Where(key => Input.GetKey(key.Key)).Sum(key => key.Value);
        return Math.Max(force, 0) * direction;
    }

    public Vector3 GetLiftForce()
    {
        var force = Clift * angVel * angVel * propSize * propSize * propSize * propSize;
        return transform.up * force;
    }

    void Update()
    {
        var torque = GetMagForce() * motorSize;
        torque -= Cdrag * angVel * angVel * propSize * propSize * propSize * propSize * direction;
        var angAcc = torque / moment;
        angVel += angAcc * Time.deltaTime;
    }
}