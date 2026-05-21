using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StaminaManager : MonoBehaviour
{
    public Slider staminaGauage;

    public Key runKey = Key.B;
    public float maxStamina = 10f;
    public float minusStamina = 2f;
    public float plusStamina = 2f;

    public float currentStamina;

    public bool running = false;

    void Start()
    {
        currentStamina = maxStamina;
        staminaGauage.maxValue = maxStamina;
    }

    void Update()
    {
       if(Keyboard.current != null)
        {
            running = Keyboard.current[runKey].isPressed;
        }

        if (running)
        {
            currentStamina -= minusStamina * Time.deltaTime;
        }
        else
        {
            currentStamina += plusStamina * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0 , maxStamina);

        if(staminaGauage != null)
        {
             staminaGauage.value = currentStamina;
        }
        
    }
}
