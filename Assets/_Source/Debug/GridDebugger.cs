using System.Diagnostics;
using UnityEngine;

public class GridDebugger : MonoBehaviour
{
    public bool drawing = true;
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public float cellSize = 1f;
    public Color gridColor = Color.green;

    [Conditional("UNITY_EDITOR")]
    private void OnDrawGizmos()
    {
        if (drawing)
        {
            DrawGrid();
        }
    }

    private void DrawGrid()
    {
        Gizmos.color = gridColor;

        // Calculate offset to center the grid
        Vector3 offset = new Vector3(-gridSizeX * cellSize / 2f, -gridSizeY * cellSize / 2f, 0f);

        // Draw horizontal lines
        for (float y = 0; y <= gridSizeY; y++)
        {
            Vector3 startPoint = new Vector3(0, y * cellSize, 0) + offset;
            Vector3 endPoint = new Vector3(gridSizeX * cellSize, y * cellSize, 0) + offset;
            Gizmos.DrawLine(startPoint, endPoint);
        }

        // Draw vertical lines
        for (float x = 0; x <= gridSizeX; x++)
        {
            Vector3 startPoint = new Vector3(x * cellSize, 0, 0) + offset;
            Vector3 endPoint = new Vector3(x * cellSize, gridSizeY * cellSize, 0) + offset;
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}
