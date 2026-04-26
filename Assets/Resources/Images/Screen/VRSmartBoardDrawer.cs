using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SimpleVRDrawer : MonoBehaviour
{
    public Transform controller;   // RightHandAnchor
    public GameObject board;       // SCREEN
    public Material lineMaterial;

    private LineRenderer currentLine;
    private InputDevice device;
    private Collider boardCollider;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        boardCollider = board.GetComponent<Collider>();
    }

    void Update()
    {
        if (!device.isValid)
            device = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        bool drawPressed = false;
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out drawPressed); // 👈 B button

        Ray ray = new Ray(controller.position, controller.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == boardCollider)
            {
                if (drawPressed)
                {
                    if (currentLine == null)
                        StartLine();

                    AddPoint(hit.point);
                }
                else
                {
                    currentLine = null;
                }
            }
        }
    }

    void StartLine()
    {
        GameObject obj = new GameObject("Line");
        obj.AddComponent<BoxCollider>();
        currentLine = obj.AddComponent<LineRenderer>();

        currentLine.material = lineMaterial;
        currentLine.widthMultiplier = 0.01f;
        currentLine.positionCount = 0;
    }

    void AddPoint(Vector3 point)
    {
        int count = currentLine.positionCount;

        if (count > 0)
        {
            if (Vector3.Distance(currentLine.GetPosition(count - 1), point) < 0.01f)
                return;
        }

        currentLine.positionCount++;
        currentLine.SetPosition(count, point);
    }
}