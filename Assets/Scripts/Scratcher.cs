using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Scratcher : MonoBehaviour
{
    //[SerializeField] GameObject raycastPlane;
    //[SerializeField] AudioClip
    Slider slider;
    [SerializeField] AudioSource source;
    bool playing = false;
    float currentTarget = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
        source.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.IsPressed())
        {
            //this lerps it and works okayyy but if you grab it far and hold it there it slows down as it reaches its goal. If we just have it force to its goal if its on the target note they probably won't notice it, but it is something.
            source.pitch = Mathf.Lerp(source.pitch, (slider.value * source.clip.length) - source.time, 1 - Mathf.Pow(5000, -Time.deltaTime));
            playing = false;

            //tried to have it directly tied to the mouse. honestly sounded pretty good but the scale wasn't matching up. I tried adjusting it based on the width of the slider but wasn't working out.
            //source.pitch = Mouse.current.delta.ReadValue().x;

            
            //was thinking each frame I could set it to the pitch necessary to reach the slider value by the next frame, since sound is played between frames. The math checked out on my paper but it definitely doesn't behave correctly.
            //source.pitch = ((slider.value * source.clip.length) - source.time) / Time.deltaTime;
        }
        else if (!playing)
        {
            playing = true;
            source.pitch = 1;
            source.time = slider.value * source.clip.length;
        }
        else
        {
            //StartCoroutine(lerpPitchToOne(1));
            source.pitch = 1;
            slider.value = source.time / source.clip.length;
        }

    }

    //was thinking I could have it go in steps. When the mouse grabs it it gets a new target and sets the pitch and only changes it once it reaches it. This would stop the slow down when the gap is large but it didnt really behave right. I may not have implemented it correctly.
    bool checkIfReached()
    {
        //we've gone past the target
        if (source.pitch > 0 && source.time > currentTarget)
        {
            return true;
        }
        //we've gone past the target negatively
        else if (source.pitch < 0 && source.time < currentTarget)
        {
            return true;
        }
        else if (source.time - currentTarget < 0.2f && source.time - currentTarget > -0.2f)
        {
            return true;
        }


        return false;
    }

    //was going to have this go when you let go to smooth out the jump but it doesn't really solve the problem
    IEnumerator lerpPitchToOne(float time)
    {
        float elapsed = 0;

        while (source.pitch != 1)
        {
            elapsed += Time.deltaTime;
            source.pitch = Mathf.Lerp(source.pitch, 1, elapsed / time);
            yield return null;
        }

    }
}