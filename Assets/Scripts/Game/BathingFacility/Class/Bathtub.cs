using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class Bathtub : MonoBehaviour, IBathingFacility, ITemperatureControl
{
  
  [SerializeField]private int temperature;
  public int Temperature
  {
    get => temperature;
    set
    {
      temperature = value;
      TemperatureText.text = value.ToString();
    }
  }

  [SerializeField]private TextMeshPro temperatureText;
  public TextMeshPro TemperatureText
  {
    get => temperatureText;
    set => temperatureText = value;
  }

  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else Temperature--;
  }
}