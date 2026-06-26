using UnityEngine;

public class HittableBeat : MonoBehaviour
{
    public float speed = 1.0f;
    public float hitRadius = 0.0f;

    private Vector3 center;

    void Start()
    {
        center = FindAnyObjectByType<Vinyl>().transform.position;
        transform.LookAt(center);
    }

    void Update()
    {
        float step = speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, center) - step <= hitRadius)
        {
            Destroy(gameObject);
            return;
        }

        transform.position += transform.forward * step;
    }
}