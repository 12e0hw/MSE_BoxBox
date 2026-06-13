using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CityItemUI : MonoBehaviour
{
    [SerializeField] private Image flagImage;
    [SerializeField] private TMP_Text cityNameText;

    // Set the city name and flag image for this city item.
    public void SetCity(string cityName, Sprite flagSprite)
    {
        if (cityNameText != null)
        {
            cityNameText.text = cityName;
        }

        if (flagImage != null)
        {
            flagImage.sprite = flagSprite;
            flagImage.enabled = flagSprite != null;
            flagImage.preserveAspect = true;
        }
    }
}