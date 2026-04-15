using UnityEngine;
using UnityEngine.UI;

public class TimeGauage : MonoBehaviour
{
    public Slider timeGuage;
    public float maxTime = 120f;
    public float currentTime;
    public bool timeOver = false;
    void Start()
    {
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

        if(currentTime == 0)
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
