using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class VRSmartBoardDrawer : MonoBehaviour
{
    [Header("Board")]
    public GameObject boardObject;
    private Collider boardCollider;
    public Camera fallbackCamera;

    [Header("VR Controller")]
    public Transform controllerRayOrigin;
    public XRNode controllerHand = XRNode.RightHand;

    [Header("Drawing")]
    public Material lineMaterial;
    public float lineWidth = 0.012f;
    public float surfaceOffset = 0.004f;
    public float minPointDistance = 0.015f;
    public float maxDrawDistance = 20f;

    [Header("Eraser")]
    public float eraserRadius = 0.08f;

    [Header("Colors")]
    public Color[] penColors =
    {
        Color.white,
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    [Header("Double Tap")]
    public float doubleTapTime = 0.35f;

    private InputDevice controller;
    private LineRenderer currentLine;

    private List<LineRenderer> allLines = new List<LineRenderer>();
    private Stack<LineRenderer> undoneLines = new Stack<LineRenderer>();

    private int currentColorIndex = 0;
    private float lastPrimaryPressTime = -10f;

    private bool previousTriggerPressed = false;
    private bool previousPrimaryPressed = false;
    private bool previousSecondaryPressed = false;
    private bool previousThumbstickUsed = false;

    // 🔥 NEW: Draw Mode
    private bool isDrawingEnabled = false;

    void Start()
    {
        if (boardObject != null)
            boardCollider = boardObject.GetComponent<Collider>();

        if (fallbackCamera == null)
            fallbackCamera = Camera.main;

        TryInitializeController();
    }

    void Update()
    {
        if (!controller.isValid)
            TryInitializeController();

        bool triggerPressed = GetTriggerPressed();
        bool primaryPressed = GetPrimaryButtonPressed();
        bool secondaryPressed = GetSecondaryButtonPressed();
        bool eraserPressed = GetEraserPressed();

        HandlePrimaryButton(primaryPressed);

        // 🔥 DRAW ONLY WHEN ENABLED
        if (isDrawingEnabled)
        {
            if (eraserPressed)
            {
                currentLine = null;
                TryEraseAtPointer();
            }
            else
            {
                HandleDrawing(triggerPressed);
            }
        }
        else
        {
            currentLine = null;
        }

        // 🔥 CLEAR
        if (secondaryPressed && !previousSecondaryPressed)
            ClearBoard();

        HandleThumbstickControls();

        // 🔧 MOUSE TESTING (NO VR)
        HandleMouseFallback();

        previousTriggerPressed = triggerPressed;
        previousPrimaryPressed = primaryPressed;
        previousSecondaryPressed = secondaryPressed;
    }

    void TryInitializeController()
    {
        controller = InputDevices.GetDeviceAtXRNode(controllerHand);
    }

    // ================= INPUT =================

    bool GetTriggerPressed()
    {
        if (controller.isValid &&
            controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool val))
            return val;

        return Input.GetMouseButton(0);
    }

    bool GetPrimaryButtonPressed()
    {
        if (controller.isValid &&
            controller.TryGetFeatureValue(CommonUsages.primaryButton, out bool val))
            return val;

        return Input.GetKeyDown(KeyCode.Space);
    }

    bool GetSecondaryButtonPressed()
    {
        if (controller.isValid &&
            controller.TryGetFeatureValue(CommonUsages.secondaryButton, out bool val))
            return val;

        return false;
    }

    bool GetEraserPressed()
    {
        if (controller.isValid &&
            controller.TryGetFeatureValue(CommonUsages.gripButton, out bool val))
            return val;

        return Input.GetKey(KeyCode.E);
    }

    // ================= CORE LOGIC =================

    void HandlePrimaryButton(bool primaryPressed)
    {
        if (primaryPressed && !previousPrimaryPressed)
        {
            float now = Time.time;

            // DOUBLE TAP → CHANGE COLOR
            if (now - lastPrimaryPressTime <= doubleTapTime)
            {
                ChangeColor();
                lastPrimaryPressTime = -10f;
            }
            else
            {
                lastPrimaryPressTime = now;

                // SINGLE TAP → TOGGLE DRAW MODE
                isDrawingEnabled = !isDrawingEnabled;
                Debug.Log("Draw Mode: " + (isDrawingEnabled ? "ON" : "OFF"));
            }
        }
    }

    void HandleDrawing(bool triggerPressed)
    {
        if (triggerPressed && !previousTriggerPressed)
            StartNewLine();

        if (triggerPressed && TryGetBoardHit(out RaycastHit hit))
        {
            Vector3 drawPoint = hit.point + hit.normal * surfaceOffset;
            AddPoint(drawPoint);
        }

        if (!triggerPressed)
            currentLine = null;
    }

    bool TryGetBoardHit(out RaycastHit hit)
    {
        Ray ray;

        if (controllerRayOrigin != null)
            ray = new Ray(controllerRayOrigin.position, controllerRayOrigin.forward);
        else
            ray = fallbackCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, maxDrawDistance))
            return hit.collider == boardCollider;

        return false;
    }

    void StartNewLine()
    {
        GameObject obj = new GameObject("Line_" + allLines.Count);
        obj.transform.SetParent(transform);

        currentLine = obj.AddComponent<LineRenderer>();
        currentLine.positionCount = 0;
        currentLine.widthMultiplier = lineWidth;
        currentLine.numCapVertices = 8;
        currentLine.numCornerVertices = 8;

        Material mat = new Material(lineMaterial);
        Color col = penColors[currentColorIndex];

        mat.color = col;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", col * 2f);

        currentLine.material = mat;

        allLines.Add(currentLine);
        undoneLines.Clear();
    }

    void AddPoint(Vector3 point)
    {
        if (currentLine == null) return;

        int count = currentLine.positionCount;

        if (count > 0)
        {
            if (Vector3.Distance(currentLine.GetPosition(count - 1), point) < minPointDistance)
                return;
        }

        currentLine.positionCount++;
        currentLine.SetPosition(count, point);
    }

    void TryEraseAtPointer()
    {
        if (!TryGetBoardHit(out RaycastHit hit)) return;

        Vector3 p = hit.point;

        for (int i = allLines.Count - 1; i >= 0; i--)
        {
            var line = allLines[i];

            for (int j = 0; j < line.positionCount; j++)
            {
                if (Vector3.Distance(line.GetPosition(j), p) < eraserRadius)
                {
                    Destroy(line.gameObject);
                    allLines.RemoveAt(i);
                    return;
                }
            }
        }
    }

    void ChangeColor()
    {
        currentColorIndex = (currentColorIndex + 1) % penColors.Length;
        Debug.Log("Color: " + penColors[currentColorIndex]);
    }

    public void ClearBoard()
    {
        foreach (var l in allLines)
            Destroy(l.gameObject);

        allLines.Clear();
        undoneLines.Clear();
    }

    public void Undo()
    {
        if (allLines.Count == 0) return;

        var last = allLines[allLines.Count - 1];
        allLines.RemoveAt(allLines.Count - 1);

        last.gameObject.SetActive(false);
        undoneLines.Push(last);
    }

    public void Redo()
    {
        if (undoneLines.Count == 0) return;

        var line = undoneLines.Pop();
        line.gameObject.SetActive(true);
        allLines.Add(line);
    }

    void HandleThumbstickControls()
    {
        if (!controller.isValid) return;

        if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
        {
            if (!previousThumbstickUsed)
            {
                if (axis.x > 0.75f) { Redo(); previousThumbstickUsed = true; }
                else if (axis.x < -0.75f) { Undo(); previousThumbstickUsed = true; }
                else if (axis.y > 0.75f) { lineWidth += 0.002f; previousThumbstickUsed = true; }
                else if (axis.y < -0.75f) { lineWidth = Mathf.Max(0.004f, lineWidth - 0.002f); previousThumbstickUsed = true; }
            }

            if (Mathf.Abs(axis.x) < 0.2f && Mathf.Abs(axis.y) < 0.2f)
                previousThumbstickUsed = false;
        }
    }

    // 🖱️ TEST WITHOUT VR
    void HandleMouseFallback()
    {
        if (Input.GetMouseButtonDown(1)) ChangeColor();
        if (Input.GetKeyDown(KeyCode.C)) ClearBoard();
        if (Input.GetKeyDown(KeyCode.Z)) Undo();
        if (Input.GetKeyDown(KeyCode.Y)) Redo();
        if (Input.GetKey(KeyCode.E)) TryEraseAtPointer();
        if (Input.GetKeyDown(KeyCode.LeftBracket))
{
    lineWidth = Mathf.Max(0.004f, lineWidth - 0.003f);
    Debug.Log("Brush size decreased: " + lineWidth);
}

if (Input.GetKeyDown(KeyCode.RightBracket))
{
    lineWidth += 0.003f;
    Debug.Log("Brush size increased: " + lineWidth);
}
    }
}