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

//public Slider stationSlider;
    public CircleSlider circleSlider;

    public RectTransform arrowImage;
    public float arrowStartPositionY;
    public float arrowEndPositionY;

    void Start()
    {
        //stationSlider.onValueChanged.AddListener(delegate { OnSliderValueChanged(stationSlider.value); });
        circleSlider.OnValueChanged.AddListener(OnSliderValueChanged);
        //stationSlider.onValueChanged.AddListener(AdjustRadioVolume); // Dodane
        PlayRadio();
      //  UpdateArrowPosition(stationSlider.value);
    }

    public void AdjustRadioVolume(float sliderValue)
    {
        float volumeInDecibels = SliderValueToDecibels(sliderValue);
        audioMixer.SetFloat("Volume", volumeInDecibels);
    }

    private float SliderValueToDecibels(float sliderValue)
    {
        // Interpolacja liniowa miêdzy -80 dB a 20 dB
        return Mathf.Lerp(-80f, 20f, sliderValue / 100f);
    }

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

    void UpdateArrowPosition(float sliderValue)
    {
        float normalizedValue = sliderValue / 100f;
        float newYPosition = Mathf.Lerp(arrowStartPositionY, arrowEndPositionY, normalizedValue);

        arrowImage.anchoredPosition = new Vector2(arrowImage.anchoredPosition.x, newYPosition);
    }
}
