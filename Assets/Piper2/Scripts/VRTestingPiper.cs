using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VRTestingPiper : MonoBehaviour
{

    public UnityEvent unityEvent;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Testing());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Testing()
    {
        yield return new WaitForSeconds(5);
        unityEvent.Invoke();
    } 
}
