using UnityEngine;

public enum BoxColor { Red, Blue, Green }
public enum BoxSize { Small, Big }

public class BoxController : MonoBehaviour
{
    public BoxColor boxColor;
    public BoxSize boxSize;
    public int scoreValue;

    private void Start()
    {
        scoreValue = (boxSize == BoxSize.Big) ? 5 : 2;
    }

    // 박스 에셋 색 구분 어려울 때 사용
    // private void ApplyVisualColor()
    // {
    //     SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
    //     if (sr == null) sr = GetComponent<SpriteRenderer>();

    //     if (sr != null)
    //     {
    //         switch (boxColor)
    //         {
    //             case BoxColor.Red: sr.color = Color.red; break;
    //             case BoxColor.Blue: sr.color = Color.blue; break;
    //             case BoxColor.Green: sr.color = Color.green; break;
    //         }
    //     }
    // }
}