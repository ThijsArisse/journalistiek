using System.Collections;
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
    private void SetResetables<T>()
        where T : Component
    {
        var arr = FindObjectsByType<T>(FindObjectsSortMode.None);
        foreach (var item in arr)
        {
            resetables.Add((item.transform, item.transform.position, item.transform.rotation));
        }
    }

#if DEBUG
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPositions();
        }
    }
#endif

    public void ResetPositions()
    {
        foreach (var item in resetables)
        {
            item.resetable.transform.SetPositionAndRotation(item.position, item.rotation);
        }
    }
}
