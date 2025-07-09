using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ShowDamage : MonoBehaviour
{
    private TextMeshPro _damageText;
    private float moveDistance = 0.3f;  
    private float duration = 0.5f;      

    private void Awake()
    {
        _damageText = GetComponent<TextMeshPro>();
        if (_damageText == null)
        {
            _damageText = gameObject.AddComponent<TextMeshPro>();
        }
        
        _damageText.alignment = TextAlignmentOptions.Center;
        _damageText.fontSize = 3;
    }

    public void SetInfo(Vector2 pos, float damage, Transform parent = null, bool isCritical = false)
    {
        transform.position = pos;

        if (isCritical)
        {
            transform.localScale = Vector3.one;
            _damageText.color = Color.red;
            _damageText.text = $"{Mathf.RoundToInt(damage)}!";
            _damageText.fontSize = 3.5f;
        }
        else
        {
            transform.localScale = Vector3.one;
            _damageText.color = Color.white;
            _damageText.text = Mathf.RoundToInt(damage).ToString();
            _damageText.fontSize = 3f;
        }

        _damageText.alpha = 1f;
        
        if (parent != null)
        {
            _damageText.sortingOrder = 123;
        }

        DoAnimation();
    }

    private void DoAnimation()
    {
        Sequence seq = DOTween.Sequence();

        transform.localScale = Vector3.one * 0.8f;
        seq.Append(transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack))  
           .Join(transform.DOMoveY(transform.position.y + moveDistance, duration).SetEase(Ease.OutQuad)) 
           .Join(_damageText.DOFade(0f, duration).SetEase(Ease.InQuad)) 
           //TODO : 풀링 여기서 처리하세용
           .OnComplete(() => Destroy(gameObject));
    }
}
