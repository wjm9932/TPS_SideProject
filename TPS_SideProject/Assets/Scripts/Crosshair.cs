using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Image aimPointReticle;
    public Image hitPointReticle;

    public float smoothTime = 0.2f;
    
    private Camera screenCamera;
    private RectTransform crossHairRectTransform;

    private Vector2 currentHitPointVelocity;
    private Vector2 targetPoint;

    private void Awake()
    {
        crossHairRectTransform = hitPointReticle.GetComponent<RectTransform>();
        screenCamera = Camera.main;
    }

    public void SetActiveCrosshair(bool active)
    {
        hitPointReticle.enabled = active;
        aimPointReticle.enabled = active;
    }

    public void UpdatePosition(Vector3 worldPoint)
    {
        targetPoint = screenCamera.WorldToScreenPoint(worldPoint);
    }

    private void Update()
    {
        if(hitPointReticle.enabled == false)
        {
            return;
        }
        crossHairRectTransform.position = targetPoint;//Vector2.SmoothDamp(crossHairRectTransform.position, targetPoint, ref currentHitPointVelocity, smoothTime);
    }
}