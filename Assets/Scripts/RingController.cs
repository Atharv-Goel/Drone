using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RingController : MonoBehaviour
{
    public GameObject ring;
    public GameObject drone;
    private Queue<Vector3[]> beziers;
    private Queue<bool> ready;
    public int count = 2;
    private Queue<GameObject> rings;
    private Vector3[] last;
    void Start()
    {
        beziers = new Queue<Vector3[]>();
        ready = new Queue<bool>();
        rings = new Queue<GameObject>();
        for (var i = 0; i < count; i++)
        {
            ready.Enqueue(false);
            beziers.Enqueue(new[] {Vector3.zero, Vector3.zero, Vector3.zero});
            var copy = Instantiate(ring, drone.transform.position, Quaternion.identity);
            copy.SetActive(false);
            rings.Enqueue(copy);
        }

        last = new[] { new Vector3(0, 0, -10f), Vector3.zero, Vector3.zero };
    }

    Vector3 boundClamp(Vector3 v)
    {
        v.x = Math.Clamp(v.x, -40f, 40f);
        v.y = Math.Clamp(v.y, 3f, 100f);
        v.z = Math.Clamp(v.z, -40f, 40f);
        return v;
    }

    void Generate()
    {
        ready.Dequeue();
        ready.Enqueue(true);
        beziers.Dequeue();
        Destroy(rings.Peek());
        rings.Dequeue();
        var second = boundClamp((last[2] - last[1]) * Random.Range(0.5f, 2f));
        var third = boundClamp(second +
                               new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)));
        last = new[] { last[2], second, third };
        beziers.Enqueue(last);
        rings.Enqueue(Instantiate(ring, last[2], Quaternion.identity));
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!ready.Peek())
            Generate();
    }
}
