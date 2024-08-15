using TMPro;

public interface ITemperatureControl
{

  int Temperature { get; set; }
  
  TextMeshPro TemperatureText { get; set; }

  
  
  public void ChangeTemperature(TemperatureControlSymbol symbol);
}
