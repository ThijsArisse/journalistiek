using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class resetpoint : MonoBehaviour
{
    [SerializeField] Transform resetTransform;

    [SerializeField] GameObject player;

    [SerializeField] Camera playerhead;

    [SerializeField] InputActionReference inputRef;

    void Start()
    {
        inputRef.action.started += (_) => OnResetCamera();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        OnResetCamera();
    }

    [ContextMenu("Reset Position")]
    public void OnResetCamera()
    {
        print("reset");
        var distanceDiff = resetTransform.position - playerhead.transform.position;
        player.transform.position += distanceDiff;
    }
}
