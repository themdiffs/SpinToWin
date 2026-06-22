using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using System;

public class Scratcher : MonoBehaviour
{
    [SerializeField] AudioSource source;
    [SerializeField] GameObject record;

    bool scratching = false;
    float targetTimeProgress = 0f; // normalized 0-1
    float timeProgress => source.time / source.clip.length; // normalized 0-1
    float secondsDiff => (targetTimeProgress - timeProgress) * source.clip.length;

    float angleDelta = 0f;


    void Start()
    {
        //slider = GetComponent<Slider>();
        source.Play();
    }

    /*
    void Update()
    {
        UpdateNeedle();

        //close. Main problems are:
        //that it's not looping correctly. When you do a full rotation it seems to lerp backwards through the whole song and it doesn't go to the end of the song when you go backwards from the start
        //One of these equations is desyncing the sound from the record visual when you grab it
        if (scratching)
        {
            inputAngle = GetAngle();
            float angleDelta = inputAngle - angleReference;
            angleReference = inputAngle;

            //lotta bullshit math here that's probably causing the desync. I know the normal rotation matches a pitch of 1 so im trying to reverse match the pitch to a certain rotation. the 32 was just brute forced until it felt right, probably an actually correct value to put there
            source.pitch = Mathf.Lerp(source.pitch, angleDelta / Time.deltaTime * 32, 1 - Mathf.Pow(10, -Time.deltaTime));

            //record.transform.Rotate(0, 0, -(angleDelta * 360));
            targetTimeProgress += angleDelta;
            var timeProgress = source.time / source.clip.length;
            record.transform.SetLocalPositionAndRotation(record.transform.localPosition, Quaternion.Euler(0, 0, -timeProgress * 360f));
        }
        else
        {
            source.pitch = 1;

            //I know this is right
            //record.transform.Rotate(0, 0, -(Time.deltaTime * (360f)) / source.clip.length);
            targetTimeProgress += Time.deltaTime;

            var timeProgress = source.time / source.clip.length;
            record.transform.SetLocalPositionAndRotation(record.transform.localPosition, Quaternion.Euler(0, 0, -timeProgress * 360f));
        }

        logTimeline();
    }
    */

    private bool wasBehind = true;

    private void Update()
    {
        // Update targetTimeProgress...
        if (scratching)
        {
            // either by rotating the disk manually,
            inputAngle = GetAngle();
            angleDelta = inputAngle - angleReference;
            angleReference = inputAngle;
            targetTimeProgress += angleDelta;
        }
        else if (!IsAudioBehind())
            // or simply playing forward
            targetTimeProgress = timeProgress;

        // Update source.time & source.pitch...
        if (scratching || IsAudioBehind())
        {
            // either by lerping to target,
            LerpAudio(angleDelta, 0.5f);
        }
        if (wasBehind != IsAudioBehind())
        {
            // or snapping to be fully caught up
            if (wasBehind && !scratching)
            {
                source.pitch = 1f;
                source.time = targetTimeProgress * source.clip.length;
            }
            wasBehind = !wasBehind;
        }


        //Debug.Log(IsAudioBehind());
        LogTimeline();

        UpdateRotation(true);
    }

    private void UpdateRotation(bool debugMode = false)
    {
        float progress = debugMode ? timeProgress : targetTimeProgress;
        record.transform.SetLocalPositionAndRotation(record.transform.localPosition, Quaternion.Euler(0, 0, -progress * 360f));
    }

    private void LerpAudio(float angleDelta, float maxDelaySeconds = 0f)
    {
        //source.pitch = Mathf.Lerp(source.pitch, angleDelta / Time.deltaTime * 32, 1 - Mathf.Pow(10, -Time.deltaTime));
        float maxSpeed = secondsDiff / maxDelaySeconds;
        source.pitch = maxSpeed;
    }

    private float thresholdSeconds = 1f;
    private bool IsAudioBehind()
    {
        return Mathf.Abs(secondsDiff) > thresholdSeconds;
    }


    float inputAngle = 0;
    float angleReference = 0;
    private void ComputeAngleDelta()
    {
        inputAngle = GetAngle();
        float d = inputAngle - angleReference;
        angleReference = inputAngle;
        angleDelta = d;
    }

    private void LogTimeline()
    {
        var timeProgress = source.time / source.clip.length;
        var rotationProgess = 1 - (record.transform.localRotation.eulerAngles.z / 360f);
        Debug.Log($"Scratcher: timeProgress={timeProgress}, targetTimeProgress={targetTimeProgress}, rotations: {rotationProgess}" +
            $" Diff = {secondsDiff}, IsBehind={IsAudioBehind()}, WasBehind={wasBehind}");
    }

    private void OnMouseDown()
    {
        scratching = true;
        angleReference = GetAngle();
    }

    private void OnMouseUp()
    {
        scratching = false;
    }

    //returns the delta change in angle
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