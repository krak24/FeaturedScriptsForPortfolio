using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CircleSlider : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IInitializePotentialDragHandler
{
    public enum Direction { CW, CCW };
    public Direction direction = Direction.CW;
    public float maxValue = 0;
    public int loops = 1;
    public bool clampOutput01 = false;
    public bool snapToPosition = false;
    public int snapStepsPerLoop = 10;
    public UnityEvent<float> OnValueChanged;

    public bool hasLight = false; 
    public GameObject objectToActivate; 

    private float knobValue;
    private float _currentLoops = 0;
    private float _previousValue = 0;
    private float _initAngle;
    private float _currentAngle;
    private Vector2 _currentVector;
    private Quaternion _initRotation;
    private bool _canDrag = false;
    private bool _screenSpaceOverlay;
    private float sliderValue;

    private void Start()
    {
        _screenSpaceOverlay = GetComponentInParent<Canvas>().rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _canDrag = true;
        _initRotation = transform.rotation;

        if (_screenSpaceOverlay)
        {
            _currentVector = eventData.position - (Vector2)transform.position;
        }
        else
        {
            _currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(transform.position);
        }
        _initAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _canDrag = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canDrag)
        {
            return;
        }

        if (_screenSpaceOverlay)
        {
            _currentVector = eventData.position - (Vector2)transform.position;
        }
        else
        {
            _currentVector = eventData.position - (Vector2)Camera.main.WorldToScreenPoint(transform.position);
        }
        _currentAngle = Mathf.Atan2(_currentVector.y, _currentVector.x) * Mathf.Rad2Deg;

        Quaternion addRotation = Quaternion.AngleAxis(_currentAngle - _initAngle, transform.forward);
        addRotation.eulerAngles = new Vector3(0, 0, addRotation.eulerAngles.z);

        Quaternion finalRotation = _initRotation * addRotation;

        if (direction == Direction.CW)
        {
            knobValue = 1 - (finalRotation.eulerAngles.z / 360f);

            if (snapToPosition)
            {
                SnapToPosition(ref knobValue);
                finalRotation.eulerAngles = new Vector3(0, 0, 360 - 360 * knobValue);
            }
        }
        else
        {
            knobValue = (finalRotation.eulerAngles.z / 360f);

            if (snapToPosition)
            {
                SnapToPosition(ref knobValue);
                finalRotation.eulerAngles = new Vector3(0, 0, 360 * knobValue);
            }
        }

        // Prevent over-rotation
        if (Mathf.Abs(knobValue - _previousValue) > 0.5f)
        {
            if (knobValue < 0.5f && loops > 1 && _currentLoops < loops - 1)
            {
                _currentLoops++;
            }
            else if (knobValue > 0.5f && _currentLoops >= 1)
            {
                _currentLoops--;
            }
            else
            {
                if (knobValue > 0.5f && _currentLoops == 0)
                {
                    knobValue = 0;
                    transform.localEulerAngles = Vector3.zero;
                    InvokeEvents(knobValue + _currentLoops);
                    return;
                }
                else if (knobValue < 0.5f && _currentLoops == loops - 1)
                {
                    knobValue = 1;
                    transform.localEulerAngles = Vector3.zero;
                    InvokeEvents(knobValue + _currentLoops);
                    return;
                }
            }
        }

        // Check max value
        if (maxValue > 0)
        {
            if (knobValue + _currentLoops > maxValue)
            {
                knobValue = maxValue;
                float maxAngle = direction == Direction.CW ? 360f - 360f * maxValue : 360f * maxValue;
                transform.localEulerAngles = new Vector3(0, 0, maxAngle);
                InvokeEvents(knobValue);
                return;
            }
        }

        transform.rotation = finalRotation;
        InvokeEvents(knobValue + _currentLoops);

        _previousValue = knobValue;
    }

    private void SnapToPosition(ref float knobValue)
    {
        float snapStep = 1f / snapStepsPerLoop;
        knobValue = Mathf.Round(knobValue / snapStep) * snapStep;
    }

    private void InvokeEvents(float value)
    {
        if (clampOutput01)
            value /= loops;

        // Convert knob value to a scale from 0 to 100
        sliderValue = value * 100f;

        OnValueChanged?.Invoke(sliderValue);

        // Activate object if 'hasLight' is enabled and sliderValue is greater than 10
        if (hasLight && sliderValue > 10f && objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        if (hasLight && sliderValue < 10f && objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    public float GetSliderValue()
    {
        return sliderValue;
    }
}
