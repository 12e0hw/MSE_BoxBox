using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StaminaManager2 : MonoBehaviour
{
    public Slider staminaGauage;

    [Header("Settings")]
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
            running = Keyboard.current.pKey.isPressed;
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

        staminaGauage.value = currentStamina;
    }
}
