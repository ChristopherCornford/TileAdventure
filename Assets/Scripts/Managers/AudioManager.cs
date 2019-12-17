using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip mainTheme;

    public GameObject cameraAudio;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = cameraAudio.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playMusic()
    {
        audioSource.PlayOneShot(mainTheme, 1.0f);
        StartCoroutine(loopMusic());
    }

    IEnumerator loopMusic()
    {
        yield return new WaitForSecondsRealtime(129.384f);
        audioSource.PlayOneShot(mainTheme, 1.0f);
        StartCoroutine(loopMusic());
    }
}
