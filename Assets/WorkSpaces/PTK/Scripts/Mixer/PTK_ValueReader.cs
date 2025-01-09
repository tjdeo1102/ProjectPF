using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class PTK_ValueReader : MonoBehaviour
{
    [SerializeField] private PTK_HandleElec handleElec;

    [SerializeField] private XRKnob targetKnob;

    void Update()
    {
        float sourceValue = handleElec.GetKnobValue();
        float sourceMaxValue = handleElec.knobMaxValue;

        float percentage = sourceValue / sourceMaxValue;

        targetKnob.value = percentage;
    }
}