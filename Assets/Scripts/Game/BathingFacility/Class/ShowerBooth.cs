using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShowerBooth : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
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

  [SerializeField] private TextMeshPro temperatureText;
  public TextMeshPro TemperatureText { get => temperatureText; set => temperatureText = value; }
  
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public Queue<BathItem> BathItems { get; set; }

  private void Start()
  {
    InitializeBathItemFields();
  }
  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 3;
    BathItemTypes = new[] { BathItemType.BodyWash, BathItemType.Shampoo };
    BathItems = new Queue<BathItem>(BathItemsQueueSize);
  }
  
  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else Temperature--;
    GameEventBus.Publish(GameEventType.TemperatureChange, new TemperatureChangeTransportData(symbol, transform.position, Temperature));
  }

  public bool TryAddBathItem(BathItem bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem.Type))
    {
      Debug.Log("Item Type Unmatched!");
      return false;
    }

    if (BathItems.Count == BathItemsQueueSize)
    {
      var item = BathItems.Dequeue(); // 가장 오래된 항목 제거
      Debug.Log(item.Type + " Out!");
    }
    BathItems.Enqueue(bathItem);
    Debug.Log(bathItem.Type + " In!");
    return true;
  }
}