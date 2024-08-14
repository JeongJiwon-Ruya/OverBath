using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
  /// <summary>
  /// customer들은 GameManager 또는 다른 Generate 클래스에서 생성하여 참조 유지.
  /// 손님 우선순위에 따라 관리해야 하기 때문에 별도의 자료구조에 저장 필요.
  /// </summary>
  public List<Customer> customers;
  public GameObject customerPrefab;

  [SerializeField] private GameObject enterArea;

  private void Start()
  {
    customers = new List<Customer>();
    TestMakeCustomer();
    TestMakeCustomer();
  }

  private void TestMakeCustomer()
  {
    var newCustomer = Instantiate(customerPrefab);
    newCustomer.transform.position = enterArea.transform.position;
    customers.Add(newCustomer.GetComponent<Customer>());
  }
  
  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.BathStateChange, CheckFitCustomer_Bathtub);
    GameEventBus.Subscribe(GameEventType.ShowerBoothTempStateChange, CheckFitCustomer_ShowerBooth);
  }

  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.BathStateChange, CheckFitCustomer_Bathtub);
    GameEventBus.UnSubscribe(GameEventType.ShowerBoothTempStateChange, CheckFitCustomer_ShowerBooth);
  }

  private void CheckFitCustomer_Bathtub(TransportData data)
  {
    /*
     * 온도 체크
     * 현재 탕 확인(bathitemtype)
     */
    if (data is BathStateChangeTransportData temperatureChangeTransportData)
    {
      var pastFittedCustomer = customers.FirstOrDefault(x =>
      {
        if (x.facilityFlow.TryPeek(out var fcb))
        {
          var pastTemp = temperatureChangeTransportData.symbol switch
          {
              TemperatureControlSymbol.Plus => temperatureChangeTransportData.temperature - 1,
              TemperatureControlSymbol.Minus => temperatureChangeTransportData.temperature + 1,
              TemperatureControlSymbol.Keep => temperatureChangeTransportData.temperature,
              _ => 0
          };

          return fcb.isMoving 
              && fcb.facilityType == temperatureChangeTransportData.facilityType 
              && fcb.temperature == pastTemp 
              && fcb.itemTypeList.FirstOrDefault() == temperatureChangeTransportData.pastBathItemType;
        }
        return false;
      });
      if (pastFittedCustomer != null)
      {
        pastFittedCustomer.Stop();
      }
      
      var fitCustomer = customers.FirstOrDefault(x =>
      {
        if (x.facilityFlow.TryPeek(out var fcb))
        {
          return fcb.isWaiting 
              && fcb.facilityType == temperatureChangeTransportData.facilityType 
              && fcb.temperature == temperatureChangeTransportData.temperature
              && fcb.itemTypeList.FirstOrDefault() == temperatureChangeTransportData.bathItemType;
        }
        return false;
        });
      if (fitCustomer != null)
      {
        fitCustomer.Move(temperatureChangeTransportData.facilityPosition);
      }
    }
  }

  private void CheckFitCustomer_ShowerBooth(TransportData data)
  {
    var pastFittedCustomer = customers.FirstOrDefault(x =>
    {
      if (x.facilityFlow.TryPeek(out var fcb))
      {
        var pastTemp = data.symbol switch
        {
            TemperatureControlSymbol.Plus => data.temperature - 1,
            TemperatureControlSymbol.Minus => data.temperature + 1,
            TemperatureControlSymbol.Keep => data.temperature,
            _ => 0
        };

        return fcb.isMoving
            && fcb.facilityType == data.facilityType
            && fcb.temperature == pastTemp;
      }
      return false;
    });
    if (pastFittedCustomer != null)
    {
      pastFittedCustomer.Stop();
    }
    var fitCustomer = customers.FirstOrDefault(x =>
    {
      if (x.facilityFlow.TryPeek(out var fcb))
      {
        return fcb.isWaiting
            && fcb.facilityType == data.facilityType
            && fcb.temperature == data.temperature;
      }
      return false;
    });
    if (fitCustomer != null)
    {
      fitCustomer.Move(data.facilityPosition);
    }
  }
}
