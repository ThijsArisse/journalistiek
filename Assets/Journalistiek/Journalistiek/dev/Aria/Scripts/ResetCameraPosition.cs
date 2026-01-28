using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetCameraPosition : MonoBehaviour
{
    [SerializeField] private CharacterController charController;
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private InputActionReference inputRef;

    private void Start() {
        inputRef.action.started += (_) => OnResetCamera();
    }

    public void OnResetCamera()
    {
        print("resetting position");
        float yDiff = charController.height - Camera.main.transform.position.y;
        xrOrigin.CameraYOffset += yDiff;

    }
}
