using System.Linq;
using UnityEngine;

public class PropMovement : MonoBehaviour
{
    [Header("Movement Weights")]
    public UDictionary<string, float> positive;
    public UDictionary<string, float> negative;
    public float inertiaX;
    public float inertiaZ;
    public float mass = 1;
    public Vector3 localPos;
    private Transform _parent;

    void Start()
    {
        Vector3 size = GetComponent<MeshRenderer>().bounds.size;
        float radius = size.x / 2;
        float height = size.y;
        float inertia = 1/4f * mass * radius * radius + 1/12f * mass * height * height;
        inertiaX = inertiaZ = inertia;

        localPos = transform.position - transform.parent.transform.position;
    }

    public (Vector3, float, float) addTorque()
    {
        localPos = transform.position - transform.parent.transform.position;
        
        var power = positive.Where(key => Input.GetKey(key.Key)).Sum(key => key.Value) - 
                        negative.Where(key => Input.GetKey(key.Key)).Sum(key => key.Value);
        var force = transform.up * power;
        var torque = Vector3.Cross(localPos, force);
        return (force, Vector3.Dot(torque, transform.right), Vector3.Dot(torque, transform.forward));
    }
}
