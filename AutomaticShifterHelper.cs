using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutomaticShifterHelper : MonoBehaviour, IDragHandler
{
    public RectTransform background;
    public RectTransform knobRect;
    public RCCP_UIController controller;

    private int currentGearIndex = 3;
    private bool isKnobMoving = false;
    private RCCP_Gearbox gearbox;
    private RCCP_Input input;
    public void OnDrag(PointerEventData eventData)
    {
        if (!isKnobMoving)
        {
            float dragDistance = eventData.delta.y;
            int direction = Mathf.RoundToInt(Mathf.Sign(dragDistance)); // Round to -1, 0, or 1

            if (direction == -1 && knobRect.localPosition.y != -50f)
            {
                if (knobRect.localPosition.y <= 70f && knobRect.localPosition.y > 50f)
                {
                    currentGearIndex++;
                    MoveKnobSmoothly(20f);
                }
                else if (knobRect.localPosition.y <= 20f && knobRect.localPosition.y > 0f)
                {
                    currentGearIndex++;
                    MoveKnobSmoothly(-15f);
                }
                else if (knobRect.localPosition.y <= -15f && knobRect.localPosition.y > -30f)
                {
                    currentGearIndex++;
                    MoveKnobSmoothly(-50f);
                }
            }
            else if (direction == 1 && knobRect.localPosition.y != 70f)
            {
                if (knobRect.localPosition.y >= -50f && knobRect.localPosition.y < -30f)
                {
                    currentGearIndex--;
                    MoveKnobSmoothly(-15f);
                }
                else if (knobRect.localPosition.y >= -15f && knobRect.localPosition.y < 10f)
                {
                    currentGearIndex--;
                    MoveKnobSmoothly(20f);
                }
                else if (knobRect.localPosition.y >= 20f && knobRect.localPosition.y < 30f)
                {
                    currentGearIndex--;
                    MoveKnobSmoothly(70f);
                }
            }
        }
    }

    private void MoveKnobSmoothly(float targetY)
    {
        isKnobMoving = true;
        StartCoroutine(LerpKnob(targetY));
    }

    private IEnumerator LerpKnob(float targetY)
    {
        Vector3 targetPosition = new Vector3(knobRect.localPosition.x, targetY, knobRect.localPosition.z);
        while (Vector3.Distance(knobRect.localPosition, targetPosition) > 0.1f)
        {
            knobRect.localPosition = Vector3.Lerp(knobRect.localPosition, targetPosition, Time.deltaTime * 30f);
            yield return null;
        }

        knobRect.localPosition = targetPosition;
        gearbox = RCCP_SceneManager.Instance.activePlayerVehicle.GetComponentInChildren<RCCP_Gearbox>();
        input = RCCP_SceneManager.Instance.activePlayerVehicle.GetComponentInChildren<RCCP_Input>();
        
        if (currentGearIndex == 0)
        {
            gearbox.ShiftToN();
            controller.isPressing = true;
        }
        else if (currentGearIndex == 1)
        {
            controller.isPressing = false;
            gearbox.ShiftReverse();
        }
        else if (currentGearIndex == 2)
        {
            controller.isPressing = false;
            gearbox.ShiftToN();
        }
        else if (currentGearIndex == 3)
        {
            controller.isPressing = false;
            gearbox.ShiftToGear(0);
        }
        isKnobMoving = false;
    }
}
