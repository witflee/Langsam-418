using UnityEngine;

public class eraser : MonoBehaviour
{
    public float eraseRadius = 0.08f;

    void Update()
    {
        LineRenderer[] lines = FindObjectsOfType<LineRenderer>();

        foreach (LineRenderer line in lines)
        {
            for (int i = 0; i < line.positionCount; i++)
            {
                Vector3 point = line.GetPosition(i);

                if (Vector3.Distance(transform.position, point) < eraseRadius)
                {
                    Destroy(line.gameObject);
                    break;
                }
            }
        }
    }
}
