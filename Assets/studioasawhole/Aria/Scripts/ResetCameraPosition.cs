using Unity.XR.CoreUtils;
using UnityEngine;

public class ResetCameraPosition : MonoBehaviour
{
    [SerializeField] private CharacterController charController;
    [SerializeField] private XROrigin xrOrigin;

    public void OnResetCamera()
    {
        print("resetting position");
        float yDiff = charController.height - Camera.main.transform.position.y;
        xrOrigin.CameraYOffset += yDiff;

    }
}
