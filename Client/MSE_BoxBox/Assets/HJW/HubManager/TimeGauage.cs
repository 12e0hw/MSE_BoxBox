using UnityEngine;
using UnityEngine.UI;

public class TimeGauage : MonoBehaviour
{
    
    public Slider timeGuage;
    public float maxTime = 12f;
    public float currentTime;
    public static bool timeOver = false;
    void Start()
    {
        timeOver = false;
        currentTime = maxTime;
        if(timeGuage != null)
        {
            timeGuage.maxValue = maxTime;
            timeGuage.value = currentTime;
        }
    }

    void Update()
    {
        if(timeOver) return;
        currentTime -= Time.deltaTime;

        if(currentTime <= 0)
        {
            timeOver = true;
            Debug.Log("Game Over");
        }

        if(timeGuage != null)
        {
            timeGuage.value = currentTime;
        }
    }
}
