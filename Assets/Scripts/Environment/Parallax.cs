using UnityEngine;

/// <summary>
/// Simple, robust parallax effect for 2D (side-scrollers, menus, backgrounds).
/// Attach to each background layer and assign the camera (or leave empty to use Camera.main).
///
/// parallaxFactor:
///   0   = layer moves with the camera (appears fixed on screen, e.g. sky / sun)
///   1   = layer stays fixed in the world (appears to move at full speed, e.g. foreground)
///   0.5 = layer moves at half the camera speed (mid-distance)
///
/// Optional infinite horizontal tiling: enable "infiniteHorizontal" and make sure the
/// object has a SpriteRenderer whose sprite is seamlessly tileable.
/// </summary>
[DisallowMultipleComponent]
public class Parallax : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Camera to follow. Defaults to Camera.main.")]
    [SerializeField] private Transform cameraTransform;

    [Header("Parallax")]
    [Tooltip("0 = moves with camera (far away), 1 = stays in world (close).")]
    [SerializeField, Range(0f, 1f)] private float parallaxFactorX = 0.5f;
    [SerializeField, Range(0f, 1f)] private float parallaxFactorY = 0.2f;

    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;

    [Header("Infinite Horizontal Scrolling")]
    [SerializeField] private bool infiniteHorizontal = false;

    private Vector3 startPosition;
    private Vector3 lastCameraPosition;
    private float spriteWidth;

    private void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
        {
            Debug.LogWarning($"{nameof(Parallax)}: no camera found on {name}. Disabling.", this);
            enabled = false;
            return;
        }

        startPosition = transform.position;
        lastCameraPosition = cameraTransform.position;

        if (infiniteHorizontal)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                Vector2 size = sr.sprite.bounds.size;
                spriteWidth = size.x * transform.lossyScale.x; // one tile, in world units

                sr.drawMode = SpriteDrawMode.Tiled;
                sr.size = new Vector2(size.x * 5f, size.y);   // 5 tiles wide, centered on the object
            }
            else
            {
                infiniteHorizontal = false;
            }
        }
    }

    // LateUpdate so the camera has already moved this frame (avoids jitter).
    private void LateUpdate()
    {
        Vector3 camPos = cameraTransform.position;
        Vector3 camDelta = camPos - lastCameraPosition;

        Vector3 pos = transform.position;
        if (!lockX) pos.x += camDelta.x * parallaxFactorX;
        if (!lockY) pos.y += camDelta.y * parallaxFactorY;
        transform.position = pos;

        lastCameraPosition = camPos;

        if (infiniteHorizontal && spriteWidth > 0f)
            WrapHorizontally(camPos.x);
    }

    private void WrapHorizontally(float camX)
    {
        // How far the camera is from this layer's centre.
        float distance = camX - transform.position.x;

        if (Mathf.Abs(distance) >= spriteWidth)
        {
            float offset = Mathf.Sign(distance) * spriteWidth * Mathf.Floor(Mathf.Abs(distance) / spriteWidth);
            Vector3 pos = transform.position;
            pos.x += offset;
            transform.position = pos;
        }
    }
}
