using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathtubWarehouse : MonoBehaviour, IWarehouse
{
  [SerializeField]private BathItemType bathItemType;
  public BathItemType BathItemType
  {
    get => bathItemType;
    set => bathItemType = value;
  }


  public BathItemType BathItemOut()
  {
    //animation
    return BathItemType;
  }
}