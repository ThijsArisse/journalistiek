using System.Collections.Generic;
using UnityEngine;

public class PositionResetter : MonoBehaviour
{

    static PositionResetter instance;

    List<(Transform resetable, Vector3 position, Quaternion rotation)> resetables = new();

    [Header("By default without reset every rigidbody\nbeing enabled it only resets objects\nwith the component PositionResetable.")]
    public bool justResetEveryRigidbody = false;

    private void Awake()
    {
        //to make sure this is the only one in the scene
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple PositionResetter in the scene, please consider using only one.", this);
            Destroy(this);
        }
    }

    private void Start()
    {
        if (justResetEveryRigidbody)
        {
            SetResetables<Rigidbody>();
        }
        else
        {
            SetResetables<PositionResetable>();
        }
    }

    //think I have to do this so I dont have to do much copy paste work
    //uses generic type as parameter where the type must inherit Component, which always has a transform
    private void SetResetables<T>()
        where T : Component
    {
        //add all the objects to the list and save their initial position (tuple is just handy for this dont need a struct)
        var arr = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (T item in arr)
        {
            resetables.Add((item.transform, item.transform.position, item.transform.rotation));
        }
    }

#if DEBUG
    void Update()
    {
        //for debugging be able to press R to reset
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPositions();
        }
    }
#endif

    public void ResetPositions()
    {
        //resets the positions back to their initial position (and rotation)
        foreach (var item in resetables)
        {
            item.resetable.transform.SetPositionAndRotation(item.position, item.rotation);
        }
    }
}
