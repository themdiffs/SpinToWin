using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Scratcher : MonoBehaviour
{
    //[SerializeField] GameObject raycastPlane;
    //[SerializeField] AudioClip
    Slider slider;
    [SerializeField] AudioSource source;
    float currentTime = 0;
    //AudioClip clip;
    bool scratching = false;
    bool playing = false;
    float lastValue = 0;

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
            StopAllCoroutines();
            //currentTime = Mathf.Lerp(currentTime, slider.value * source.clip.length, 0.01f);
            //source.time = currentTime;
            /*
            float currentValue = slider.value;
            float speed = currentValue - lastValue;
            lastValue = currentValue;

            source.pitch = speed;
            */

            float currentTime = source.time;
            float targetTime = slider.value * source.clip.length;

            float speed = targetTime - currentTime;
            source.pitch = speed;
        }
        else if (!playing)
        {
            StopAllCoroutines();
            playing = true;
            source.pitch = 1;
            //source.Play();
        }
        else
        {
            //StartCoroutine(lerpPitchToOne(1));
            slider.value = source.time / source.clip.length;
        }

    }
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