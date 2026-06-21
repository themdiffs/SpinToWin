using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Scratcher : MonoBehaviour
{
    [SerializeField] AudioSource source;
    bool scratching = false;
    [SerializeField] GameObject record;
    float inputAngle = 0;
    float angleReference = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //slider = GetComponent<Slider>();
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        updateNeedle();

        //close. Main problems are:
        //that it's not looping correctly. When you do a full rotation it seems to lerp backwards through the whole song and it doesn't go to the end of the song when you go backwards from the start
        //One of these equations is desyncing the sound from the record visual when you grab it
        if (scratching)
        {
            scratching = true;

            inputAngle = getRotation();
            float angleDifference = inputAngle - angleReference;
            angleReference = inputAngle;

            //lotta bullshit math here that's probably causing the desync. I know the normal rotation matches a pitch of 1 so im trying to reverse match the pitch to a certain rotation. the 32 was just brute forced until it felt right, probably an actually correct value to put there
            source.pitch = Mathf.Lerp(source.pitch, angleDifference / Time.deltaTime * 32, 1 - Mathf.Pow(10, -Time.deltaTime));

            record.transform.Rotate(0, 0, -(angleDifference * 360));
        }
        else
        {
            source.pitch = 1;

            //I know this is right
            record.transform.Rotate(0, 0, -(Time.deltaTime * (360f * 4)) / source.clip.length);
        }

    }
    private void OnMouseDown()
    {
        scratching = true;
        angleReference = getRotation();
    }

    private void OnMouseUp()
    {
        scratching = false;
    }

    //returns the delta change in angle
    float getRotation()
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

    void updateNeedle()
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