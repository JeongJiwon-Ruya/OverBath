using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TemperatureControlSymbol
{
  Plus,
  Minus,
  Keep
}
public enum FacilityType {Bathtub, ShowerBooth}

public interface IBathingFacility
{
  public FacilityType facilityType { get; set; }
  
  public Customer currentCustomer { get; set; }
}
