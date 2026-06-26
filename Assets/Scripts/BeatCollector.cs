using UnityEngine;

public class BeatCollector : MonoBehaviour
{
    public void HandleBeat(HittableBeat beat)
    {
        if (beat == null) return;
        ScoreTracker.Instance.AddScore(1);
        Destroy(beat.gameObject);
    }
}