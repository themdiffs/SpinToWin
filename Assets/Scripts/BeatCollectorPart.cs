using UnityEngine;

public class BeatColliderPart : MonoBehaviour
{
    private BeatCollector collector;

    void Awake()
    {
        collector = GetComponentInParent<BeatCollector>();
    }

    void OnTriggerEnter(Collider other)
    {
        HittableBeat beat = other.GetComponent<HittableBeat>();
        if (beat != null)
            collector.HandleBeat(beat);
    }
}