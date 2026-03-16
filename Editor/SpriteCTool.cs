using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

#if (UNITY_EDITOR) 
public class SpriteCombinerTool
{
    const int TEMP_LAYER = 31;

    [MenuItem("Tools/Combine Selected Sprites")]
    static void CombineSprites()
    {
        GameObject[] selection = Selection.gameObjects;
        if (selection.Length == 0) return;

        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        foreach (var go in selection)
            renderers.AddRange(go.GetComponentsInChildren<SpriteRenderer>());

        if (renderers.Count == 0) return;

        float masterPPU = renderers[0].sprite.pixelsPerUnit;
        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers) bounds.Encapsulate(r.bounds);

        // Pixel Snapping
        Vector3 min = bounds.min;
        Vector3 max = bounds.max;
        min.x = Mathf.Floor(min.x * masterPPU) / masterPPU;
        min.y = Mathf.Floor(min.y * masterPPU) / masterPPU;
        max.x = Mathf.Ceil(max.x * masterPPU) / masterPPU;
        max.y = Mathf.Ceil(max.y * masterPPU) / masterPPU;
        bounds.SetMinMax(min, max);

        int width = Mathf.RoundToInt(bounds.size.x * masterPPU);
        int height = Mathf.RoundToInt(bounds.size.y * masterPPU);

        // Setup Camera
        GameObject camObj = new GameObject("TempBakeCam");
        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = bounds.size.y / 2f;
        cam.transform.position = bounds.center + Vector3.back * 10;
        cam.backgroundColor = new Color(0, 0, 0, 0); 
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.cullingMask = 1 << TEMP_LAYER;

        Material unlitMat = new Material(Shader.Find("Unlit/Transparent"));
        Dictionary<SpriteRenderer, Material> oldMats = new Dictionary<SpriteRenderer, Material>();
        Dictionary<GameObject, int> oldLayers = new Dictionary<GameObject, int>();
        Dictionary<Transform, Vector3> oldScales = new Dictionary<Transform, Vector3>();

        foreach (var r in renderers)
        {
            oldMats[r] = r.sharedMaterial;
            oldLayers[r.gameObject] = r.gameObject.layer;
            oldScales[r.transform] = r.transform.localScale;

            float ppuScale = r.sprite.pixelsPerUnit / masterPPU;
            r.transform.localScale = oldScales[r.transform] * ppuScale;

            MaterialPropertyBlock block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            block.SetTexture("_MainTex", r.sprite.texture);
            r.SetPropertyBlock(block);

            r.sharedMaterial = unlitMat; 
            r.gameObject.layer = TEMP_LAYER;
        }

        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        RenderTexture.active = rt;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        cam.targetTexture = rt;
        cam.Render();

        Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        Object.DestroyImmediate(camObj);
        Object.DestroyImmediate(unlitMat);

        foreach (var r in renderers)
        {
            if (r != null)
            {
                r.sharedMaterial = oldMats[r];
                r.gameObject.layer = oldLayers[r.gameObject];
                r.transform.localScale = oldScales[r.transform];
                r.SetPropertyBlock(null);
            }
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string path = $"Assets/Combined_{selection[0].name}_{timestamp}.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        AssetDatabase.ImportAsset(path);
        
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = masterPPU;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        // --- CREATE RESULT ---
        GameObject combinedGo = new GameObject("Combined_" + selection[0].name);
        combinedGo.transform.position = bounds.center; 
        
        var resultSR = combinedGo.AddComponent<SpriteRenderer>();
        resultSR.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        resultSR.sortingLayerID = renderers[0].sortingLayerID;
        resultSR.sortingOrder = renderers[0].sortingOrder;

        // --- COLLIDER LOGIC ---
        // 1. Add Rigidbody (Required for Composite)
        Rigidbody2D rb = combinedGo.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        
        // 2. Add Composite
        CompositeCollider2D composite = combinedGo.AddComponent<CompositeCollider2D>();
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;

        // 3. Clone child colliders directly onto the main object (or helpers)
        foreach (var originalGo in selection)
        {
            foreach (var childCol in originalGo.GetComponentsInChildren<Collider2D>())
            {
                // Create a temporary child to hold the individual collider shape
                GameObject colContainer = new GameObject("ColHolder");
                colContainer.transform.SetParent(combinedGo.transform);
                colContainer.transform.position = childCol.transform.position;
                colContainer.transform.rotation = childCol.transform.rotation;
                colContainer.transform.localScale = childCol.transform.lossyScale;

                // Copy Collider Data
                if (childCol is BoxCollider2D b) {
                    var n = colContainer.AddComponent<BoxCollider2D>();
                    n.size = b.size; n.offset = b.offset; n.usedByComposite = true;
                }
                else if (childCol is PolygonCollider2D p) {
                    var n = colContainer.AddComponent<PolygonCollider2D>();
                    n.points = p.points; n.offset = p.offset; n.usedByComposite = true;
                }
                else if (childCol is CircleCollider2D c) {
                    var n = colContainer.AddComponent<CircleCollider2D>();
                    n.radius = c.radius; n.offset = c.offset; n.usedByComposite = true;
                }
                else if (childCol is CapsuleCollider2D cap) {
                    var n = colContainer.AddComponent<CapsuleCollider2D>();
                    n.size = cap.size; n.offset = cap.offset; n.direction = cap.direction; n.usedByComposite = true;
                }
            }
        }

        // 4. Force the composite to bake in the Editor
        composite.GenerateGeometry();
        
        // Auto-select the new object
        Selection.activeGameObject = combinedGo;

        Debug.Log("Combined Sprite and Merged Collider Created.");
    }
}
#endif