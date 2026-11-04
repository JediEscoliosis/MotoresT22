using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timer = 0f;
    public TextMeshProUGUI textTimer;

    public event Action OnFinished;      
    
    bool running;

    void Awake()
    {
        textTimer.enabled = false;       
    }

    public void StartTimer(float seconds)
    {
        timer = seconds;                 
        running = true;
        textTimer.enabled = true;        
        textTimer.text = Mathf.CeilToInt(timer).ToString();
    }

    void Update()
    {
        if (!running) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = 0f;
            running = false;
            textTimer.enabled = false;   
            OnFinished?.Invoke();
            return;
        }

        textTimer.text = Mathf.CeilToInt(timer).ToString();   
    }
}