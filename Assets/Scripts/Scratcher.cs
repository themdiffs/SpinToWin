using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Scratcher : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] GameObject record;
    [SerializeField] float followLag = 0.08f;
    [SerializeField] float catchUpSeconds = 0.1f;
    [SerializeField] float maxPitch = 10f;

    float targetTimeProgress = 0f;
    float timeProgress => source.time / source.clip.length;

    float audioProgress = 0f;
    float lastWrapped = 0f;

    float secondsDiff => (targetTimeProgress - audioProgress) * source.clip.length;

    bool scratching = false;
    bool catchingUp = false;
    float catchUpTimer = 0f;
    float deltaAngle = 0f;

    float inputAngle = 0;
    float angleReference = 0;

    void Start()
    {
        source.loop = true;
        source.Play();
    }

    private void Update()
    {
        UpdateNeedle();

        float wrapped = timeProgress;
        float step = wrapped - lastWrapped;
        if (step > 0.5f) step -= 1f;
        else if (step < -0.5f) step += 1f;
        audioProgress += step;
        lastWrapped = wrapped;

        if (scratching)
        {
            inputAngle = GetAngle();
            deltaAngle = inputAngle - angleReference;
            deltaAngle -= Mathf.Round(deltaAngle);
            angleReference = inputAngle;
            targetTimeProgress += deltaAngle;

            source.pitch = secondsDiff / followLag;
        }
        else
        {
            CatchUp();
        }

        UpdateRotation(false);
    }

    // After release
    private void CatchUp()
    {
        if (!catchingUp)
        {
            // Caught up
            targetTimeProgress = audioProgress;
            source.pitch = 1f;
            return;
        }

        catchUpTimer -= Time.deltaTime;
        float gap = secondsDiff;

        if (catchUpTimer <= 0f || Mathf.Abs(gap) <= 0.001f)
        {
            SetAudioProgress(targetTimeProgress); // land exactly
            source.pitch = 1f;
            catchingUp = false;
            return;
        }

        float playable = maxPitch * catchUpTimer;
        if (Mathf.Abs(gap) > playable)
        {
            SetAudioProgress(targetTimeProgress - Mathf.Sign(gap) * playable / source.clip.length);
            gap = secondsDiff;
        }

        source.pitch = gap / catchUpTimer;
    }

    private void SetAudioProgress(float progress)
    {
        audioProgress = progress;
        float w = Mathf.Repeat(progress, 1f);
        source.time = w * source.clip.length;
        lastWrapped = w;
    }

    private void UpdateRotation(bool debugMode = false)
    {
        float progress = debugMode ? timeProgress : targetTimeProgress;
        record.transform.SetLocalPositionAndRotation(record.transform.localPosition, Quaternion.Euler(0, 0, -progress * 360f));
    }
    private void OnMouseDown()
    {
        scratching = true;
        angleReference = GetAngle();
    }

    private void OnMouseUp()
    {
        scratching = false;
        catchingUp = true;
        catchUpTimer = catchUpSeconds;
    }

    //returns the angle under the cursor, normalized 0-1
    float GetAngle()
    {
        RaycastHit hit;
        float angle = 0f;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity))
        {
            angle = Vector3.SignedAngle(new Vector3(0, 1, 0), new Vector3(hit.point.x, hit.point.y, 0), new Vector3(0, 0, -1));
            if (angle <= 0)
            {
                angle += 360f;
            }
        }
        return angle / 360f;
    }

    void UpdateNeedle()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            source.volume = 0;
        }
        else
        {
            source.volume = 1;
        }
    }
}