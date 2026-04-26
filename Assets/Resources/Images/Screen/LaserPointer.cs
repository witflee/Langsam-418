using UnityEngine;

public class SmartLaserPointer : MonoBehaviour
{
    public LineRenderer line;

    [Header("Modes")]
    public bool useMouse = true; // true now, false for VR

    [Header("Mouse Mode")]
    public Camera cam;

    [Header("VR Mode")]
    public Transform controller; // assign RightHandAnchor later

    public float maxDistance = 20f;

    void Start()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        UpdateLaser();
    }

    void UpdateLaser()
    {
        Ray ray;

        // 🖱 MOUSE MODE
        if (useMouse)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        // 🎮 VR MODE
        else
        {
            ray = new Ray(controller.position, controller.forward);
        }

        RaycastHit hit;
        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            endPoint = hit.point;
        }
        else
        {
            endPoint = ray.origin + ray.direction * maxDistance;
        }

        line.SetPosition(0, ray.origin);
        line.SetPosition(1, endPoint);
    }
}