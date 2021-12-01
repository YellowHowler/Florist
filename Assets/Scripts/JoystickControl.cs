﻿using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoystickControl : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform _thisTransform;  // 현재 조이스틱
    private RectTransform _pointTransform;  // 조이스틱 손잡이
    
    private Vector2 _pointPosition;  // point 원래 자리
    private float _pointRadius;  // point 반지름
    private float _posToDir;  // 입력 위치 > 입력 방향으로 변환할때 사용하는 값 (1 / _pointRange)

    private const float MinRange = 0.3f;  // 입력 반지름 최소 범위
    private const float MaxRange = 1.3f;  // 입력 반지름 최대 범위
    
    public readonly Subject<Vector2> joystickDirection = new Subject<Vector2>();
    public readonly BoolReactiveProperty isJoystickDown = new BoolReactiveProperty();

    public void OnDrag(PointerEventData eventData)
    {
        var inputPosition = eventData.position - _pointPosition;
        _pointTransform.anchoredPosition = inputPosition.magnitude > _pointRadius * MinRange
            ? inputPosition.normalized * _pointRadius * MinRange : inputPosition;

        joystickDirection.OnNext(Position2Direction(inputPosition));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isJoystickDown.Value = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointTransform.anchoredPosition = Vector2.zero;
        isJoystickDown.Value = false;
    }

    private Vector2 Position2Direction(Vector2 position)
    {
        position *= _posToDir;

        if (position.magnitude > MaxRange)
        {
            position = position.normalized * MaxRange;
        }
        else if(position.magnitude < MinRange)
        {
            position = Vector2.zero;
        }
        
        return position.normalized * (position.magnitude - MinRange);
    }

    private void Start()
    {
        _thisTransform = GetComponent<RectTransform>();
        _pointTransform = transform.GetChild(0).GetComponent<RectTransform>();

        _pointPosition = _pointTransform.position;
        _pointRadius = _thisTransform.sizeDelta.x * 0.3f;
        _posToDir = 1 / _pointRadius;
    }
}
