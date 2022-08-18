using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ButtonBar : MonoBehaviour
{
    public VideoPlayer video;
    public TextMeshProUGUI progressTime, totalTime;
    public Slider slider;
    public bool CanCounting;

    private void Start()
    {
        CanCounting = true;
    }

    private void Update()
    {
        if (video.isPaused && Time.time > 2)
            CanCounting = false;
        if (CanCounting)
            SetTime(video.frame);
    }

    public void SetTime(long frame)
    {
        var progress = (float) frame / video.frameCount;
        slider.value = progress;
        var second = video.frameCount / video.frameRate;
        
        progressTime.text = SecondToTime(progress * second);
        totalTime.text = SecondToTime(second);
    }

    private static string SecondToTime(float rawSecond)
    {
        var minutes =  TimeSpan.FromSeconds(rawSecond).TotalMinutes;
        var seconds = TimeSpan.FromSeconds(rawSecond).Seconds;
        return $"{minutes:00}:{seconds:00}";
    }
}
