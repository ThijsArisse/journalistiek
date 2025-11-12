using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class XRSpawnManager : MonoBehaviour
{
    [Tooltip("Desired world Y position (meters) for the XR Origin after XR initializes.")]
    public float desiredHeight = 1.8f;

    [Tooltip("How many seconds to wait for XR subsystems to initialize before giving up.")]
    public float waitTimeout = 5f;

    [Tooltip("If true, reset any Camera Offset child local Y to 0 after positioning.")]
    public bool zeroCameraOffsetLocalY = true;

    private IEnumerator Start()
    {
        yield return StartCoroutine(WaitForXRAndConfigure());
        ApplySpawnHeight();
    }

    private IEnumerator WaitForXRAndConfigure()
    {
        float t = 0f;
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();

        // Wait until XR Manager is initialized
        while (!XRGeneralSettings.Instance || XRGeneralSettings.Instance.Manager == null || !XRGeneralSettings.Instance.Manager.isInitializationComplete)
        {
            if (t > waitTimeout) break;
            t += Time.deltaTime;
            yield return null;
        }

        // Wait for XRInputSubsystem instances to appear
        t = 0f;
        while (true)
        {
            subsystems.Clear();
            SubsystemManager.GetInstances(subsystems);

            if (subsystems.Count > 0) break;

            if (t > waitTimeout) break;
            t += Time.deltaTime;
            yield return null;
        }

        // Try to set tracking origin mode to Floor and recenter on available subsystems
        foreach (var ss in subsystems)
        {
            if (ss == null) continue;

            // Try set floor tracking origin (preferred)
            ss.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor);

            // Try recenter so whatever standing/sitting offsets are cleared
            ss.TryRecenter();

            // Give the subsystem a frame to apply changes
            yield return null;
        }

        // Extra couple frames to allow XR to finish any internal repositioning
        yield return null;
        yield return null;
    }

    private void ApplySpawnHeight()
    {
        // Temporarily disable CharacterController to avoid collision pushing
        CharacterController cc = GetComponent<CharacterController>();
        bool hadCC = false;
        if (cc != null)
        {
            hadCC = cc.enabled;
            cc.enabled = false;
        }

        // Set world position Y to desiredHeight (keep X/Z)
        Vector3 p = transform.position;
        p.y = desiredHeight;
        transform.position = p;

        // Zero local Y of Camera Offset if present (common XRI structure)
        if (zeroCameraOffsetLocalY)
        {
            Transform camOffset = transform.Find("Camera Offset");
            if (camOffset != null)
            {
                Vector3 local = camOffset.localPosition;
                local.y = 0f;
                camOffset.localPosition = local;
            }
        }

        // Re-enable CharacterController if it was enabled before
        if (cc != null && hadCC)
        {
            cc.enabled = true;
        }
    }

    // Optional public method to force reapply at runtime
    public void ReapplySpawnHeight()
    {
        StartCoroutine(ReapplyCoroutine());
    }

    private IEnumerator ReapplyCoroutine()
    {
        yield return WaitForXRFrame();
        ApplySpawnHeight();
        yield return null;
    }

    private IEnumerator WaitForXRFrame()
    {
        yield return null;
        yield return null;
    }
}
