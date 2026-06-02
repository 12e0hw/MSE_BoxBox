using UnityEngine;

public enum BoxColor { Red, Blue, Green }
public enum BoxSize { Small, Big }

public class BoxController : MonoBehaviour
{
    [SerializeField] private BoxColor boxColor;
    [SerializeField] private BoxSize boxSize;


    public BoxSize Size => boxSize;
    public BoxColor Color => boxColor;
    public bool IsSmall => Size == BoxSize.Small;
    public bool IsBig => Size == BoxSize.Big;
    public int scoreValue => Size ==  BoxSize.Big ? 5 : 2;
    public bool IsDelivered { get; private set; }
    
    // 박스 크기에 따라 몇 명의 플레이어가 들어야 하는지 인원 수 할당
    //public int CarriedPlayer => Size == BoxSize.Big ? 2 : 1;

    // public bool IsCorrectTruck(BoxColor truckColor)
    // {
    //     return Color == truckColor;
    // }

    public void MarkDelivered()
    {
        IsDelivered = true;
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
