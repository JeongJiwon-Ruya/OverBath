using System.Collections.Generic;

public class FacilityControlBlock
{
  public FacilityType facilityType;
  
  public bool isMoving;
  public bool isUsingNow;
  public bool isWaiting = true;
  
  public int progress;
  public int temperature;
  public List<BathItemType> itemTypeList;

  public bool IsNextDestinationIsArea()
  {
    return facilityType is FacilityType.PaymentArea or FacilityType.HeaterArea or FacilityType.ExitArea;
  }
}
