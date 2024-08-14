using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacilityControlBlock
{
  public FacilityType facilityType;
  
  public bool isMoving;
  public bool isUsingNow;
  public bool isWaiting = true;
  
  public int progress;
  public int temperature;
  public List<BathItemType> itemTypeList;
}
