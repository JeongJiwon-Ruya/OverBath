using System.Collections;
public enum TemperatureControlSymbol
{
  Plus,
  Minus,
  Keep
}
public enum FacilityType {Bathtub, ShowerBooth}

public interface IBathingFacility
{
  /// <summary>
  /// Unity Event Function==================================
  /// </summary>
  /// <summary>
  /// ==================================Unity Event Function End
  /// </summary>
  
  /// <summary>
  /// IBathingFacility==================================
  /// </summary>
  /// <summary>
  /// ==================================IBathingFacility End
  /// </summary>
  
  /// <summary>
  /// ITemperatureControl==================================
  /// </summary>
  /// <summary>
  /// ==================================ITemperatureControl End
  /// </summary>
  
  /// <summary>
  /// IBathItemHandler==================================
  /// </summary>
  /// <summary>
  /// ==================================IBathItemHandler End
  /// </summary>
  
  public FacilityType FacilityType { get; set; }
  public Customer CurrentCustomer { get; set; }
  public IEnumerator CustomerProgressRoutine { get; set; }

  public IEnumerator StartCustomerProgressRoutine();
  public void ReleaseCustomer();
}

