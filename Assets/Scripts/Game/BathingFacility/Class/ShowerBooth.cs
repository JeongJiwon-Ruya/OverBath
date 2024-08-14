using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShowerBooth : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  /// <summary>
  /// 손님이 오는 조건
  /// 1. 온도가 일치해야함
  ///
  /// Progress 차는 조건
  /// 1. BathItem이 모두 갖춰져야함.
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

  [SerializeField] private TextMeshPro temperatureText;
  public TextMeshPro TemperatureText { get => temperatureText; set => temperatureText = value; }
  
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public Queue<BathItemType> BathItems { get; set; }

  private void Start()
  {
    facilityType = FacilityType.ShowerBooth;
    routine = CheckBathItemIsFitCurrentCustomer();
    InitializeBathItemFields();
  }
  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 3;
    BathItemTypes = new[] { BathItemType.BodyWash, BathItemType.Shampoo };
    BathItems = new Queue<BathItemType>(BathItemsQueueSize);
  }
  
  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else Temperature--;
    GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange, new TransportData(facilityType, symbol, transform.position, Temperature));
  }

  public bool TryAddBathItem(BathItemType bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem))
    {
      Debug.Log("Item Type Unmatched!");
      return false;
    }

    if (BathItems.Count == BathItemsQueueSize)
    {
      var item = BathItems.Dequeue(); // 가장 오래된 항목 제거
      Debug.Log(item + " Out!");
    }
    BathItems.Enqueue(bathItem);
    Debug.Log(bathItem + " In!");
    /*
     * 여기서 손님의 존재 여부에 따라 액션을 달리함
     */
    StopCoroutine(routine);
    StartCoroutine(routine);
    return true;
  }

  private void OnCollisionEnter(Collision other)
  {
    if (!other.gameObject.TryGetComponent<Customer>(out var customer)) return;
    if (!customer.facilityFlow.TryPeek(out var fcb)) return;
    
    if (fcb.facilityType == facilityType && fcb.temperature == Temperature)
    {
      Debug.Log("Customer Lock in");
      currentCustomer = customer;
    }
  }

  private IEnumerator routine;
  
  private IEnumerator CheckBathItemIsFitCurrentCustomer()
  {
    if (!currentCustomer) yield break;
    var fcb = currentCustomer.facilityFlow.Peek();
    if (fcb.itemTypeList.All(x => BathItems.Contains(x)))
    {
      while (fcb.progress <= 100)
      {
        fcb.progress++;
        Debug.Log(fcb.progress);
        yield return new WaitForSeconds(0.01f);
      }
    }
  }
}