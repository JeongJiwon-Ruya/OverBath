using System.Collections;
using UnityEngine;

public enum TemperatureControlSymbol
{
  Plus,
  Minus,
  Keep
}
public enum FacilityType {Bathtub, ShowerBooth, Massage, Sauna, PaymentArea, HeaterArea, ExitArea}

public interface IBathingFacility
{
  public FacilityEnterPoint enterPoint { get; set; }
  public Vector3 usingPosition { get; set; }
  public FacilityType FacilityType { get; set; }
  public Customer CurrentCustomer { get; set; }
  public Coroutine CustomerProgressRoutine { get; set; }

  public IEnumerator StartCustomerProgressRoutine();
  public void ReleaseCustomer();
}

