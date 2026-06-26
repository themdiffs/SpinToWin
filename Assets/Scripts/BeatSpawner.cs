using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BeatNote
{
    public float angle;
    public float hitTime;
    public int type;

    [System.NonSerialized] public float spawnTime;
}

public class BeatSpawner : MonoBehaviour
{
    [Header("References")]
    public Vinyl vinyl;
    public GameObject[] beatPrefabs; // one per type, each with its own speed & size

    [Header("Geometry")]
    public float spawnRadius = 2.6f;
    public float collectorRadius = 4f;

    [Header("Chart")]
    public List<BeatNote> beatmap = new List<BeatNote>();

    private int next;
    private List<BeatNote> ordered;

    void Awake()
    {
        if (vinyl == null) vinyl = FindAnyObjectByType<Vinyl>();

        // calculate each beat's release time from when it must arrive
        foreach (var note in beatmap)
        {
            float speed = beatPrefabs[note.type].GetComponent<HittableBeat>().speed;
            note.spawnTime = note.hitTime - (spawnRadius - collectorRadius) / speed;
        }

        // sort by spawn time
        ordered = new List<BeatNote>(beatmap);
        ordered.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }

    void Update()
    {
        float elapsed = ScoreTracker.Instance.elapsed;
        while (next < ordered.Count && elapsed >= ordered[next].spawnTime)
        {
            Spawn(ordered[next]);
            next++;
        }
    }

    void Spawn(BeatNote note)
    {
        Vector3 pos = PointOnVinyl(note.angle, spawnRadius);
        Instantiate(beatPrefabs[note.type], pos, Quaternion.identity);
    }

    Vector3 PointOnVinyl(float angleDeg, float radius)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        return vinyl.transform.position + dir * radius;
    }

    void OnDrawGizmos()
    {
        if (vinyl == null) return;

        Gizmos.color = Color.cyan;
        DrawCircle(spawnRadius);
        Gizmos.color = Color.yellow;
        DrawCircle(collectorRadius);

        Gizmos.color = Color.red;
        foreach (var note in beatmap)
            Gizmos.DrawSphere(PointOnVinyl(note.angle, spawnRadius), 0.15f);
    }

    void DrawCircle(float radius, int segments = 64)
    {
        Vector3 prev = PointOnVinyl(0f, radius);
        for (int i = 1; i <= segments; i++)
        {
            Vector3 cur = PointOnVinyl(360f / segments * i, radius);
            Gizmos.DrawLine(prev, cur);
            prev = cur;
        }
    }
}