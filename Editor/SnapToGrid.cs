using UnityEngine;
using UnityEditor;

#if (UNITY_EDITOR) 
public class SnapToGrid : EditorWindow
{
    private float gridSize = 1f; // Default grid size

    [MenuItem("Tools/Snap Selected To Grid")]
    public static void ShowWindow()
    {
        GetWindow<SnapToGrid>("Snap To Grid");
    }

    private void OnGUI()
    {
        GUILayout.Label("Snap Settings", EditorStyles.boldLabel);
        gridSize = EditorGUILayout.FloatField("Grid Size", gridSize);

        if (GUILayout.Button("Snap Selected Objects"))
        {
            SnapSelectedObjects();
        }
    }

    private void SnapSelectedObjects()
    {
        if (gridSize <= 0)
        {
            Debug.LogWarning("Grid size must be greater than zero.");
            return;
        }

        Undo.RecordObjects(Selection.transforms, "Snap To Grid");

        foreach (Transform t in Selection.transforms)
        {
            Vector3 pos = t.position;

            // Snap each axis to nearest grid point
            pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
            pos.y = Mathf.Round(pos.y / gridSize) * gridSize;
            pos.z = Mathf.Round(pos.z / gridSize) * gridSize;

            t.position = pos;
        }

        Debug.Log($"Snapped {Selection.transforms.Length} object(s) to grid size {gridSize}.");
    }
}
#endif