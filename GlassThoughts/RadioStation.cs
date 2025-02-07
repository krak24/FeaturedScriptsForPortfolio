using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RadioStation : MonoBehaviour
{
    [HideInInspector]
    public AudioSource audioSource;

    public List<AudioClip> stationClips;  
    private List<AudioClip> currentStationClips;  

    public AudioClip currentClip;
    public int currentClipNumber;

    public bool currentRadio; 
    public bool stopRadio; 

    [HideInInspector]
    public float waitTime; 


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ShuffleRadio();
    }


    void Update()
    {

        if (!currentRadio && !stopRadio)
        {
            waitTime -= 1 * Time.deltaTime;
            if (waitTime < 0)
            {
                audioSource.Stop();
                ShuffleRadio();
                stopRadio = true;
            }
        }

        // If the station is active, play the next clip when the current one ends
        if (!stopRadio)
        {
            if (!audioSource.isPlaying)
            {
                currentClipNumber += 1;
                if (currentClipNumber >= currentStationClips.Count)
                {
                    currentClipNumber = 0;
                }
                currentClip = currentStationClips[currentClipNumber];
                audioSource.clip = currentClip;
                audioSource.Play();
            }
        }
    }

    // Randomizes the order of audio clips for playback
    public void ShuffleRadio()
    {
        currentStationClips = new List<AudioClip>(stationClips);
        List<int> numbersTaken = new List<int>();

        for (int i = 0; i < stationClips.Count; i++)
        {
            bool next = false;
            while (!next)
            {
                int newNumber = Random.Range(0, stationClips.Count);
                if (!numbersTaken.Contains(newNumber))
                {
                    currentStationClips[i] = stationClips[newNumber];
                    numbersTaken.Add(newNumber);
                    next = true;
                }
            }
        }
    }
}