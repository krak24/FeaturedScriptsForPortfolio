using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Radio : MonoBehaviour
{
    public AudioMixer audioMixer;

    public List<RadioStation> radioStations;
    public int currentStation;

    public float waitBeforeOff = 15;

    public CircleSlider circleSlider;

    public RectTransform arrowImage;
    public float arrowStartPositionY;
    public float arrowEndPositionY;

    void Start()
    {
        // Add a listener to update the radio station when the slider value changes
        circleSlider.OnValueChanged.AddListener(OnSliderValueChanged);

        PlayRadio();
    }

    // Adjusts the radio volume based on the slider value
    public void AdjustRadioVolume(float sliderValue)
    {
        float volumeInDecibels = SliderValueToDecibels(sliderValue);
        audioMixer.SetFloat("Volume", volumeInDecibels);
    }

    // Converts slider value (0-100) to decibels (-80 to 20)
    private float SliderValueToDecibels(float sliderValue)
    {
        return Mathf.Lerp(-80f, 20f, sliderValue / 100f);
    }

    // Activates the selected radio station and deactivates others
    public void PlayRadio()
    {
        for (int i = 0; i < radioStations.Count; i++)
        {
            if (i != currentStation)
            {
                radioStations[i].audioSource.volume = 0;
                radioStations[i].waitTime = waitBeforeOff;
                radioStations[i].currentRadio = false;
            }
        }
        radioStations[currentStation].audioSource.volume = 1;
        radioStations[currentStation].stopRadio = false;
        radioStations[currentStation].currentRadio = true;
    }

    // Updates the radio station based on the slider value
    public void OnSliderValueChanged(float value)
    {
        int stationCount = radioStations.Count;
        float segmentSize = 100f / stationCount;

        for (int i = 0; i < stationCount; i++)
        {
            if (value >= i * segmentSize && value < (i + 1) * segmentSize)
            {
                currentStation = i;
                break;
            }
        }

        PlayRadio();
        UpdateArrowPosition(value);
    }

    // Updates the arrow position to reflect the slider value
    void UpdateArrowPosition(float sliderValue)
    {
        float normalizedValue = sliderValue / 100f;
        float newYPosition = Mathf.Lerp(arrowStartPositionY, arrowEndPositionY, normalizedValue);

        arrowImage.anchoredPosition = new Vector2(arrowImage.anchoredPosition.x, newYPosition);
    }
}
