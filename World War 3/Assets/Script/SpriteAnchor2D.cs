using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteAlways]
public class SpriteAutoFit2D : MonoBehaviour
{
    [Header("Включить автоподгонку под экран")]
    public bool useAutoFit = false;

    private Vector2 vpBL, vpTR;
    private float savedDepth;
    private float initialZ;
    private float savedYPosition;
    private bool inited = false;

    private int lastScreenWidth = -1;
    private int lastScreenHeight = -1;

    private Camera cam;
    private SpriteRenderer sr;

    void Awake()
    {
        cam = Camera.main;
        sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (useAutoFit)
            InitializeFit();
    }

    void Update()
    {
        if (useAutoFit && !inited)
        {
            InitializeFit();
        }
        else if (!useAutoFit)
        {
            // Сохраняем текущую Y-позицию при отключении автоподгонки
            savedYPosition = transform.position.y;
            inited = false;
        }
    }

    void LateUpdate()
    {
        if (!useAutoFit || cam == null || sr.sprite == null || !inited)
            return;

        // Проверка изменения размеров экрана
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;

            RecalculateScaleAndCenter();
        }

        ApplyLockedY();
    }

    private void InitializeFit()
    {
        if (cam == null || sr == null || sr.sprite == null)
            return;

        initialZ = transform.position.z;

        Bounds b = sr.bounds;
        Vector3 worldMin = new Vector3(b.min.x, b.min.y, initialZ);
        Vector3 worldMax = new Vector3(b.max.x, b.max.y, initialZ);

        Vector3 vMin = cam.WorldToViewportPoint(worldMin);
        Vector3 vMax = cam.WorldToViewportPoint(worldMax);
        vpBL = new Vector2(RoundToPrecision(vMin.x, 5), RoundToPrecision(vMin.y, 5));
        vpTR = new Vector2(RoundToPrecision(vMax.x, 5), RoundToPrecision(vMax.y, 5));

        savedDepth = RoundToPrecision(cam.WorldToViewportPoint(transform.position).z, 5);

        savedYPosition = transform.position.y;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        inited = true;

        RecalculateScaleAndCenter();
    }

    private void RecalculateScaleAndCenter()
    {
        Vector3 wBL = cam.ViewportToWorldPoint(new Vector3(vpBL.x, vpBL.y, savedDepth));
        Vector3 wTR = cam.ViewportToWorldPoint(new Vector3(vpTR.x, vpTR.y, savedDepth));

        Vector2 targetSize = new Vector2(wTR.x - wBL.x, wTR.y - wBL.y);
        Vector2 spriteSize = sr.sprite.bounds.size;

        if (spriteSize.x <= float.Epsilon || spriteSize.y <= float.Epsilon)
            return;

        Vector3 ls = transform.localScale;
        ls.x = RoundToPrecision(targetSize.x / spriteSize.x, 5);
        ls.y = RoundToPrecision(targetSize.y / spriteSize.y, 5);
        transform.localScale = ls;

        Vector3 center = (wBL + wTR) * 0.5f;
        transform.position = new Vector3(
            RoundToPrecision(center.x, 5),
            transform.position.y, // Y не трогаем здесь
            initialZ
        );
    }

    private void ApplyLockedY()
    {
        Vector3 pos = transform.position;
        pos.y = savedYPosition;
        transform.position = pos;
    }

    private float RoundToPrecision(float value, int decimals)
    {
        float multiplier = Mathf.Pow(10f, decimals);
        return Mathf.Round(value * multiplier) / multiplier;
    }
}
