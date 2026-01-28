using UnityEngine;
using UnityEngine.XR;

public class HandModelSwitcher : MonoBehaviour
{
    public GameObject defaultHandModel;
    public GameObject alternateHandModel;

    public XRNode inputSource; // LeftHand or RightHand
    private InputDevice device;
    private bool triggerPressedLastFrame = false;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
        SetModel(false); // Start with default
    }

    void Update()
    {
        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(inputSource);
        }

        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed))
        {
            if (triggerPressed && !triggerPressedLastFrame)
            {
                // Trigger just pressed
                SetModel(true); // Switch to alternate
            }
            else if (!triggerPressed && triggerPressedLastFrame)
            {
                // Trigger just released
                SetModel(false); // Switch back to default
            }

            triggerPressedLastFrame = triggerPressed;
        }
    }

    void SetModel(bool useAlternate)
    {
        if (defaultHandModel) defaultHandModel.SetActive(!useAlternate);
        if (alternateHandModel) alternateHandModel.SetActive(useAlternate);
    }
}
