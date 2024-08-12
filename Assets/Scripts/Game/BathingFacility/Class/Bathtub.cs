using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class Bathtub : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  public BathItemType[] BathItemTypes { get; set; }
  
  private Queue<BathItem> bathItems;
  public Queue<BathItem> BathItems { get => bathItems; set => bathItems = value; }
  
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

  private void Start()
  {
    BathItemTypes = new[] { BathItemType.Water, BathItemType.Aroma };
    BathItems = new Queue<BathItem>(1);
  }

  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else Temperature--;
  }
  
  public bool TryAddBathItem(BathItem bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem.Type))
    {
      return false;
    } 
    
    if (BathItems.TryDequeue(out var result))
    {
      Debug.Log(result.Type.ToString() + " Out!");
    }
    BathItems.Enqueue(bathItem);
    Debug.Log(bathItem.Type.ToString() + " In!");
    return true;
  }
}