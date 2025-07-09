using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ButtonTest : MonoBehaviour
{
    private UI_EventHandler eventHandler;

    [SerializeField] private float hoverDuration = 0.2f;  // 호버 애니메이션 시간
    [SerializeField] private float hoverScale = 1.1f;     // 호버시 커지는 크기

    private Vector3 originalScale;
    private Tweener currentTween;

    protected virtual void Awake()
    {
        eventHandler = GetComponent<UI_EventHandler>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // 버튼이 활성화될 때마다 크기 초기화
        transform.localScale = originalScale;
        
        // 기존 Tween이 있다면 제거
        currentTween?.Kill();
        
        eventHandler.OnPointerEnterHandler += OnHoverEnter;
        eventHandler.OnPointerExitHandler += OnHoverExit;
        eventHandler.OnClickHandler += OnClick;
    }

    private void OnDisable()
    {
        eventHandler.OnPointerEnterHandler -= OnHoverEnter;
        eventHandler.OnPointerExitHandler -= OnHoverExit;
        eventHandler.OnClickHandler -= OnClick;
    }

    private void OnHoverEnter()
    {
        SoundManager.Instance.Play("SFX_UI_Button_Keyboard_Enter_Thick_2", SoundManager.Sound.Effect);
        
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * hoverScale, hoverDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnHoverExit()
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, hoverDuration)
            .SetEase(Ease.OutQuad);
    }

    private void OnClick()
    {
        // 사운드 직접 재생
        SoundManager.Instance.Play("SFX_UI_Button_Keyboard_Enter_Thick_1", SoundManager.Sound.Effect, 1.0f, false);
        
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, hoverDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject, LinkBehaviour.CompleteAndKillOnDisable);
    }
}