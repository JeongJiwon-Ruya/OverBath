using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface ITemperatureControl
{

  int Temperature { get; set; }
  TextMeshPro TemperatureText { get; set; }

  
  public void ChangeTemperature(TemperatureControlSymbol symbol);
}
