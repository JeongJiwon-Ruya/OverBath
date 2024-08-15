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
  public FacilityType FacilityType { get; set; }
  public Customer CurrentCustomer { get; set; }
  public IEnumerator CustomerProgressRoutine { get; set; }
  
  public void ReleaseCustomer();
}
