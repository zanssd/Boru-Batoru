using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public bool isPlayer = false;
    public GameObject barFill;

    public void SetBarColor(bool isForPlayer)
    {
        isPlayer = isForPlayer;
        barFill.GetComponent<Image>().color = isPlayer ? Color.cyan : Color.red;
    }

    public void UpdateBar(float percentage)
    {
        RectTransform rt = barFill.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(GetComponent<RectTransform>().rect.width * percentage, rt.rect.height);

        Image barImage = barFill.GetComponent<Image>();
        Color currentColor = barImage.color;
        barImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, percentage < 1 && percentage > 0 ? 0.5f : 1f);
    }
}
