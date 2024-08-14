using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class Bathtub : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  /// <summary>
  /// 손님이 오는 조건
  /// 1. 온도가 일치해야함
  /// 2. 탕의 종류가 일치해야함
  /// </summary>
  public FacilityType facilityType { get; set; }

  public Customer currentCustomer { get; set; }

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
  
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public Queue<BathItemType> BathItems { get; set; }
  
  private void Start()
  {
    facilityType = FacilityType.Bathtub;
    InitializeBathItemFields();
  }

  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 1;
    BathItemTypes = new[] { BathItemType.Water, BathItemType.Aroma };
    BathItems = new Queue<BathItemType>(BathItemsQueueSize);
    /*BathItems.Enqueue(new BathItem());*/
  }

  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else if(symbol == TemperatureControlSymbol.Minus) Temperature--;
    GameEventBus.Publish(GameEventType.BathStateChange, 
        new BathStateChangeTransportData(facilityType, symbol, transform.position, Temperature, TryPeekBathItem(), TryPeekBathItem())
        );
  }
  
  public bool TryAddBathItem(BathItemType bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem))
    {
      Debug.Log("Item Type Unmatched!");
      return false;
    }

    BathItemType pastBathItem = BathItemType.None;
    if (BathItems.Count == BathItemsQueueSize)
    {
      pastBathItem = BathItems.Dequeue(); // 가장 오래된 항목 제거
      Debug.Log(pastBathItem + " Out!");
    }
    BathItems.Enqueue(bathItem);
    Debug.Log(bathItem + " In!");
    
    GameEventBus.Publish(GameEventType.BathStateChange, 
        new BathStateChangeTransportData(facilityType, TemperatureControlSymbol.Keep, transform.position, Temperature, bathItem, pastBathItem)
        );
    
    return true;
  }

  private BathItemType TryPeekBathItem()
  {
    return BathItems.TryPeek(out var item) ? item : BathItemType.None;
  }
}