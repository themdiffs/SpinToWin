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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.IsPressed())
        {
            currentTime = Mathf.Lerp(currentTime, slider.value * source.clip.length, 0.01f);
            source.time = currentTime;
        }
        else if (!playing)
        {
            playing = true;
            source.Play();
        }
        else
        {
            slider.value = source.time / source.clip.length;
        }

    }
}
