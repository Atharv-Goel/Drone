using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DroneMovement : MonoBehaviour
{

    public TMP_Text speedometer;
    public Slider angDragSlider;
    public Slider dragSlider;
    
    public float mass = 5;
    private float _inertiaX;
    private float _inertiaZ;
    public Vector3 linVel;
    private float _angVelX;
    private float _angVelZ;
    public float angDrag = 0.1f;
    public float drag = 0.15f;
    public float friction = 0.5f;
    private Vector3 _size;

    void Start()
    {
        _size = GetComponent<MeshRenderer>().bounds.size;
        _inertiaX = 1 / 12f * mass * (_size.y * _size.y + _size.z * _size.z);
        _inertiaZ = 1 / 12f * mass * (_size.x * _size.x + _size.y * _size.y);
        foreach (Transform child in transform)
        {
            var propeller = child.GetComponent<PropMovement>();
            if (propeller == null)
                continue;
            
            var dist = propeller.localPos;
            _inertiaX += propeller.inertiaX + propeller.mass * (dist.y * dist.y + dist.z * dist.z);
            _inertiaZ += propeller.inertiaZ + propeller.mass * (dist.x * dist.x + dist.y * dist.y);
        }
    }

    float Collision(Vector3 acc)
    {
        Vector3[] cornerOp = new[]
        {
            new Vector3(1, 1, 1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, 1),
            new Vector3(-1, 1, -1)
        };
        Vector3 corner = new Vector3(_size.x / 2, _size.y / 2, _size.z / 2);
        foreach (var op in cornerOp)
        {
            //Vector3
        }
        
        var angle = Math.Atan(_size.y / _size.z) + transform.eulerAngles.x;
        var k = Math.Sqrt(0.25f * _size.y * _size.y + 0.25f * _size.z * _size.z);
        var a = Math.Sin(angle) * k + linVel.y * Time.deltaTime +
                0.5 * Time.deltaTime * Time.deltaTime * acc.y;
        var b = 0.5 * Time.deltaTime * Time.deltaTime / mass;
        var c = angle + _angVelX * Time.deltaTime;
        var d = 0.25 * _size.z / _inertiaX;
        
        double fn = 8 * mass;
        for (var i = 0; i < 5; i++)
        {
            fn -= (k * Math.Sin(c + d * fn) - a - b * fn) / (d * k * Math.Cos(c + d * fn) - b);
        }

        return (float)fn;
    }

    // Update is called once per frame
    void Update()
    {
        speedometer.text = $"Speed\n{linVel.magnitude:N2}";
        angDrag = angDragSlider.value;
        drag = dragSlider.value;

        float torqueX = 0; 
        float torqueZ = 0;
        var linAcc = Vector3.zero;
        
        foreach (Transform child in transform)
        {
            var propeller = child.GetComponent<PropMovement>();
            if (propeller == null)
                continue;
            
            var (force, torX, torZ) = propeller.addTorque();
            linAcc += force / mass;
            torqueX += torX;
            torqueZ += torZ;
        }

        float resistance = drag;
        if (transform.position.y < 0.25f)
        {
            linVel.y = 0;
            linAcc.y = Mathf.Max(linAcc.y, 0);
            resistance = friction;
        }
        else
            linAcc.y -= 9.81f;

        linAcc -= (resistance * linVel.magnitude * linVel.magnitude) * linVel.normalized;

        float colForce = Collision(linAcc);
        linAcc.y += colForce / mass;
        torqueX += Vector3.Dot(Vector3.Cross(new Vector3(0, -_size.y, _size.z), new Vector3(0, colForce, 0)), transform.right);
        
        linVel += linAcc * Time.deltaTime;
        transform.position += linVel * Time.deltaTime;
        
        var angAccX = torqueX / _inertiaX;
        var angAccZ = torqueZ / _inertiaZ;
        angAccX -= angDrag * _angVelX * _angVelX * Mathf.Sign(_angVelX);
        angAccZ -= angDrag * _angVelZ * _angVelZ * Mathf.Sign(_angVelZ);
        _angVelX += angAccX * Time.deltaTime;
        _angVelZ += angAccZ * Time.deltaTime;
        transform.RotateAround(transform.position, transform.right, _angVelX * Time.deltaTime);
        transform.RotateAround(transform.position, transform.forward, _angVelZ * Time.deltaTime);
    }
}
