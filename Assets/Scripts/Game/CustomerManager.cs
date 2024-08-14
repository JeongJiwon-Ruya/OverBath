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
    var newCustomer = Instantiate(customerPrefab);
    newCustomer.transform.position = enterArea.transform.position;
    customers.Add(newCustomer.GetComponent<Customer>());
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.TemperatureChange, CheckFitCustomer);
  }

  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.TemperatureChange, CheckFitCustomer);
  }

  private void CheckFitCustomer(TransportData data)
  {
    if (data is TemperatureChangeTransportData temperatureChangeTransportData)
    {
      var pastFittedCustomer = customers.FirstOrDefault(x =>
      {
        if (x.facilityFlow.TryPeek(out var fcb))
        {
          var pastTemp = temperatureChangeTransportData.symbol == TemperatureControlSymbol.Plus
              ? temperatureChangeTransportData.temperature - 1
              : temperatureChangeTransportData.temperature + 1;
          return fcb.isMoving && fcb.temperature == pastTemp;
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
          return fcb.isWaiting && fcb.temperature == temperatureChangeTransportData.temperature;
        }
        return false;
        });
      if (fitCustomer != null)
      {
        fitCustomer.Move(temperatureChangeTransportData.facilityPosition);
      }
    }
  }
}
