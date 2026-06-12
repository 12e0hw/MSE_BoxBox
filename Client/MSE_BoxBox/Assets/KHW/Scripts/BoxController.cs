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
    public int scoreValue => Size == BoxSize.Big ? 5 : 2;
    public bool IsDelivered { get; private set; }

    public void MarkDelivered()
    {
        IsDelivered = true;
    }
}
