using UnityEngine;

public enum BathItemType 
{
  Water, 
  Aroma, 
  BodyWash, 
  Shampoo,
  Ocher,
  None = -99
}
public class BathItem : MonoBehaviour
{
  [SerializeField]private BathItemType type;
  public BathItemType Type { get => type; set => type = value; }
}

public class BathBomb : BathItem
{
  public BathBomb()
  {
  }
}

public class Cleanser : BathItem
{
  public Cleanser()
  {
  }
}