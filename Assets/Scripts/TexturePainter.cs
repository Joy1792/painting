using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePainter : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Camera paintCanvasCamera;

    [SerializeField]
    private GameObject brushCursor;

    [SerializeField]
    private Transform brushPool;

    [SerializeField]
    private LayerMask paintableLayerMask;

    [SerializeField]
    private RenderTexture paintRenderTexture;

    [SerializeField]
    private Material baseMaterial;

    private float brushScaleFactor = 1.0f;

    private Texture2D bufferTexture;

    private int brushCounter = 0, MAX_BRUSH_COUNT = 250;

    private bool textureIsSaving;

    private void Awake()
    {
        bufferTexture = new Texture2D(paintRenderTexture.width, paintRenderTexture.height, TextureFormat.RGB24, false);
    }
    private void Update()
    {
        //brushColor = ColorSelector.GetColor();  //Updates our painted color with the selected color
        if (Input.GetMouseButton(0))
        {
            Paint();
        }
       //UpdateBrushCursor();
    }

    void Paint()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitUVPosition(ref uvWorldPosition))
        {
            GameObject brushObject = Pool.Instance.GetObject();
            brushObject.GetComponent<SpriteRenderer>().color = Color.green;

            brushObject.transform.SetParent(brushPool);
            brushObject.transform.localPosition = uvWorldPosition;
        }

        brushCounter++;
        if (brushCounter >= MAX_BRUSH_COUNT)
        {
            brushCursor.SetActive(false);
        }
    }

    private bool HitUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hitInfo;
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = mainCamera.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hitInfo, 100.0F, paintableLayerMask))
        {
            Paintable paintable = hitInfo.transform.parent.GetComponent<Paintable>();
            if (paintable.MeshCollider == null || paintable.MeshCollider.sharedMesh == null) return false;

            Vector2 pixelUV = hitInfo.textureCoord;
            uvWorldPosition.x = pixelUV.x - paintCanvasCamera.orthographicSize; //To center the UV on X
            uvWorldPosition.y = pixelUV.y - paintCanvasCamera.orthographicSize; //To center the UV on Y
            uvWorldPosition.z = 0.0f;
            return true;
        }

        return false;
    }
    void UpdateBrushCursor()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (!textureIsSaving && HitUVPosition(ref uvWorldPosition))
        {
            brushCursor.SetActive(true);
            brushCursor.transform.position = uvWorldPosition + brushPool.transform.position;
        }
        else
        {
            brushCursor.SetActive(false);
        }
    }

    public void SaveTexture()
    {
        brushCounter = 0;
        
        RenderTexture.active = paintRenderTexture;
        bufferTexture.ReadPixels(new Rect(0, 0, paintRenderTexture.width, paintRenderTexture.height), 0, 0);
        bufferTexture.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = bufferTexture;

        foreach (Transform child in brushPool)
            Pool.Instance.AddObject(child.gameObject);
            

        //Invoke("ShowCursor", .1f);
    }

    void ShowCursor()
    {
        textureIsSaving = false;
    }
}
