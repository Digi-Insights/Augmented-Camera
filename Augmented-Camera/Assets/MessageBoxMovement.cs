using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MessageBoxMovement : MonoBehaviour
{
    private Vector3 _iniPos;
    private Vector3 _tempPos;
    private CanvasGroup _canvasGroup;
    public Vector3 offSet;
    public float duration = 0.5f;
    
    
    private void Awake()
    {
        _iniPos = transform.localPosition;
        _tempPos = _iniPos;
        _tempPos += offSet;
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    
    private void OnEnable()
    {
        KillAllTween();
        transform.localPosition = _tempPos;
        transform.DOLocalMove(_iniPos, duration).SetEase(Ease.OutFlash);
        _canvasGroup.DOFade(1, duration);
    }

    private void OnDisable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public void MoveBackToDisable()
    {
        KillAllTween();
        transform.DOLocalMove(_tempPos, duration).SetEase(Ease.OutFlash);
        _canvasGroup.DOFade(0, duration).OnComplete(() => gameObject.SetActive(false));
    }

    public bool IsTweening()
    {
        return DOTween.IsTweening(transform);
    }

    private void KillAllTween()
    {
        DOTween.Kill(transform);
        DOTween.Kill(_canvasGroup);
    }
}
