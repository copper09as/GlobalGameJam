using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using UnityEngine;

public class GlobalUiSystem : IGameSystem
{
    public int Priority => 0;
    private GameObject globalSettingPanel;
    private Vector2 showPos;
    private Vector2 hidePos;
    private RectTransform rect;
    private CanvasGroup canvasGroup;

    public void OnInit()
    {
        globalSettingPanel = GameObject.Instantiate
        (Resources.Load<GameObject>("Prefabs/Ui/GlobalSettingPanel"),GameEntry.Instance.transform);
        rect = globalSettingPanel.GetComponent<RectTransform>();

        canvasGroup = globalSettingPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = globalSettingPanel.AddComponent<CanvasGroup>();

        showPos = rect.anchoredPosition;
        hidePos = showPos + new Vector2(0, -200f); // 向下偏移
        HideGlobalSettingPanel();

    }
    public void ToggleGlobalSettingPanel()
    {
        if(globalSettingPanel.activeSelf)
        {
            HideGlobalSettingPanel();
        }
        else
        {
            ShowGlobalSettingPanel();
        }
    }
    public void ShowGlobalSettingPanel()
    {
        if(globalSettingPanel.activeSelf)
        {
            return;
        }
        globalSettingPanel.SetActive(true);

        rect.anchoredPosition = hidePos;
        rect.localScale = Vector3.one * 0.9f;
        canvasGroup.alpha = 0;

        DOTween.Kill(rect);

        rect.DOAnchorPos(showPos, 0.3f).SetEase(Ease.OutCubic);
        rect.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, 0.25f);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideGlobalSettingPanel()
    {
        DOTween.Kill(rect);

        rect.DOAnchorPos(hidePos, 0.25f).SetEase(Ease.InCubic);
        rect.DOScale(0.9f, 0.25f);
        canvasGroup.DOFade(0f, 0.2f)
            .OnComplete(() =>
            {
                globalSettingPanel.SetActive(false);
            });
    }

    public void OnShutdown()
    {
    }

    public void OnUpdate(float deltaTime)
    {
    }
}
