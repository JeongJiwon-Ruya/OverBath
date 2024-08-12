using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathtubWarehouse : MonoBehaviour, IWarehouse
{
  [SerializeField] private BathItem bathItem;
  public BathItem BathItem { get => bathItem; set => bathItem = value; }
  
  [SerializeField]private BathItemType bathItemType;
  public BathItemType BathItemType
  {
    get => bathItemType;
    set => bathItemType = value;
  }

  private void Start()
  {
    bathItem.Type = bathItemType;
  }


  public BathItemType BathItemOut()
  {
    //animation
    return BathItemType;
  }
}