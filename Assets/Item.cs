using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTag  {Towel}

public class Item : MonoBehaviour
{
  [SerializeField]private Collider itemCollider;
  public ItemTag itemTag { get; set; }
  
  private void Start()
  {
    if (!itemCollider) itemCollider = GetComponent<Collider>();
  } 
}
