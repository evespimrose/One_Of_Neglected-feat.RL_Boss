using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    [Header("시간 설정")]
    public float gameTime = 0f;
    public float GameTime => gameTime;

    [Header("디버그 설정")]
    [SerializeField] private bool isDebugMode = false;
    [SerializeField, Range(0f, 660f)] private float debugTime = 0f;

    // 마지막 이벤트 발생 시간 저장
    private float lastThirtySecEvent = -30f;  // 30초 이벤트
    private float lastOneMinEvent = -60f;     // 1분 이벤트
    private float lastOneMinThirtyEvent = -90f;  // 1분 30초 이벤트
    private float lastOneMinFiftyEvent = -110f;  // 1분 50초 이벤트

    public event Action OnThirtySecondsPassed;
    public event Action OnMinutePassed;
    public event Action OnOneMinThirtySecondsPassed;
    public event Action OnOneMinFiftySecondsPassed;

    private void Update()
    {
        if (GameManager.Instance.isPaused || !GameManager.Instance.isGameStarted) return;

        UpdateGameTime();
        CheckTimeBasedEvents();
    }

    private void UpdateGameTime()
    {
        if (isDebugMode)
        {
            gameTime = debugTime;
            // Debug.Log($"[Debug] Game Time: {GetFormattedTime()} ({gameTime:F1}초)");
        }
        else
        {
            gameTime += Time.deltaTime;
        }
    }

    private void CheckTimeBasedEvents()
    {
        float currentTime = gameTime;

        // 30초 이벤트 체크 (30초 이상일 때만)
        if (currentTime >= 30f && currentTime >= lastThirtySecEvent + 30f)
        {
            lastThirtySecEvent = Mathf.Floor(currentTime / 30f) * 30f;
            // Debug.Log($"[TimeManager] 30초 이벤트 발생: {currentTime}초");
            OnThirtySecondsPassed?.Invoke();
        }

        // 1분 이벤트 체크 (60초 이상일 때만)
        if (currentTime >= 60f && currentTime >= lastOneMinEvent + 60f)
        {
            lastOneMinEvent = Mathf.Floor(currentTime / 60f) * 60f;
            // Debug.Log($"[TimeManager] 1분 이벤트 발생: {currentTime}초");
            OnMinutePassed?.Invoke();
        }

        // 1분 30초 이벤트 체크 (90초 이상일 때만)
        if (currentTime >= 90f && currentTime >= lastOneMinThirtyEvent + 90f)
        {
            lastOneMinThirtyEvent = Mathf.Floor(currentTime / 90f) * 90f;
            // Debug.Log($"[TimeManager] 1분 30초 이벤트 발생: {currentTime}초");
            OnOneMinThirtySecondsPassed?.Invoke();
        }

        // 1분 50초 이벤트 체크 (110초 이상일 때만)
        if (currentTime >= 110f && currentTime >= lastOneMinFiftyEvent + 110f)
        {
            lastOneMinFiftyEvent = Mathf.Floor(currentTime / 110f) * 110f;
            // Debug.Log($"[TimeManager] 1분 50초 이벤트 발생: {currentTime}초");
            OnOneMinFiftySecondsPassed?.Invoke();
        }
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        return $"{minutes:D2}:{seconds:D2}";
    }

    public void ResetTime()
    {
        gameTime = 0f;
        lastThirtySecEvent = -30f;
        lastOneMinEvent = -60f;
        lastOneMinThirtyEvent = -90f;
        lastOneMinFiftyEvent = -110f;
    }

    public void SetDebugTime(float time)
    {
        if (!isDebugMode) return;
        debugTime = Mathf.Clamp(time, 0f, 660f);
    }
}