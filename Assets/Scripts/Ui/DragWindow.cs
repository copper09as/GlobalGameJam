using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween

public class DragWindow : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition;
    private Tween moveTween;

    [Header("拖动平滑")]
    public float dragSmoothTime = 0.2f; // 拖动平滑时间

    [Header("拖拽结束行为")]
    public bool resetOnEndDrag = false; // 如果为true，拖拽结束回到初始位置

    private Vector3 initialPosition; // 记录初始位置

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("DragWindow: UI must be a child of a Canvas");
        }

        initialPosition = rectTransform.localPosition; // 记录初始位置
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        originalPanelLocalPosition = rectTransform.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);

        moveTween?.Kill();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null)
            return;

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out localPointerPosition))
        {
            Vector3 offsetToOriginal = localPointerPosition - (Vector2)originalLocalPointerPosition;
            Vector3 targetPosition = originalPanelLocalPosition + offsetToOriginal;

            moveTween?.Kill();
            moveTween = rectTransform.DOLocalMove(targetPosition, dragSmoothTime).SetEase(Ease.OutQuad);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        moveTween?.Kill();

        if (resetOnEndDrag)
        {
            // 拖拽结束回到初始位置
            moveTween = rectTransform.DOLocalMove(initialPosition, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            // 限制在屏幕边界
            Vector3 pos = rectTransform.localPosition;

            Vector3 minPosition = new Vector3(-canvas.pixelRect.width / 2 + rectTransform.rect.width / 2,
                                              -canvas.pixelRect.height / 2 + rectTransform.rect.height / 2, pos.z);
            Vector3 maxPosition = new Vector3(canvas.pixelRect.width / 2 - rectTransform.rect.width / 2,
                                              canvas.pixelRect.height / 2 - rectTransform.rect.height / 2, pos.z);

            Vector3 clampedPos = new Vector3(
                Mathf.Clamp(pos.x, minPosition.x, maxPosition.x),
                Mathf.Clamp(pos.y, minPosition.y, maxPosition.y),
                pos.z
            );

            moveTween = rectTransform.DOLocalMove(clampedPos, 0.3f).SetEase(Ease.OutBack);
        }
    }
}
