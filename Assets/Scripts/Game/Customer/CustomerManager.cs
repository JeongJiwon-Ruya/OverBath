using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
  /// <summary>
  /// customer들은 GameManager 또는 다른 Generate 클래스에서 생성하여 참조 유지.
  /// 손님 우선순위에 따라 관리해야 하기 때문에 별도의 자료구조에 저장 필요.
  /// </summary>
  [SerializeField]private GameManger gameManger;
  
  public ObservableList<Customer> customers;
  
  public GameObject customerPrefab;

  [SerializeField] private GameObject enterArea;


  #region EventFunction
  private void Awake()
  {
    customers = new ObservableList<Customer>();
    customers.ObserveAdd().Subscribe(addEvent =>
    {
      addEvent.Value.facilityFlow.ObserveRemove().Subscribe(removeEvent =>
      {
        if (removeEvent.Value.facilityType == FacilityType.ExitArea)
        {
          Debug.Log("EXIT CUSTOMER");
          Debug.Log(addEvent.Value.name);
          customers.Remove(addEvent.Value);
        }
        else
        {
          gameManger.UpdateCustomerInfoUI(addEvent.Value);
        }
      });
    });
    customers.ObserveRemove().Subscribe(removeEvent =>
    {
      gameManger.RemoveCustomerInfoUI(removeEvent.Value);
      MakeCustomer();
    });
  }

  private void Start()
  {
    MakeCustomer();
    MakeCustomer();
  }
  
  
  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.SendAreaPosition, CheckFitCustomer_Area);
    GameEventBus.Subscribe(GameEventType.BathStateChange, CheckFitCustomer_Bathtub);
    GameEventBus.Subscribe(GameEventType.ShowerBoothTempStateChange, CheckFitCustomer_ShowerBooth);
  }

  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.SendAreaPosition, CheckFitCustomer_Area);
    GameEventBus.UnSubscribe(GameEventType.BathStateChange, CheckFitCustomer_Bathtub);
    GameEventBus.UnSubscribe(GameEventType.ShowerBoothTempStateChange, CheckFitCustomer_ShowerBooth);
  }
  #endregion
  
  private void CheckFitCustomer_Area(AreaInfoTransportData data)
  {
    
    var fitCustomer = customers.FirstOrDefault(x =>
    {
      if (x.facilityFlow.Count == 0) return false;
      var pcb = x.facilityFlow.Peek();
      return pcb.facilityType == data.facilityType
          && !pcb.isMoving
          && !pcb.isUsingNow;
    });
    if (fitCustomer != null)
    {
      if(!fitCustomer.gameObject.activeSelf) fitCustomer.gameObject.SetActive(true);
      _ = fitCustomer.Move_Area(data.position);
    }
  }
  
  private void CheckFitCustomer_Bathtub(BathStateChangeTransportData data)
  {
    /*
     * 온도 체크
     * 현재 탕 확인(bathitemtype)
     */
    var currentComingCustomer = customers.FirstOrDefault(x =>
    {
      if(x.facilityFlow.Count != 0)
          /*if (x.facilityFlow.TryPeek(out var fcb))*/
      {
        var fcb = x.facilityFlow.Peek();
        var pastTemp = data.symbol switch
        {
            TemperatureControlSymbol.Plus => data.temperature - 1,
            TemperatureControlSymbol.Minus => data.temperature + 1,
            TemperatureControlSymbol.Keep => data.temperature,
            _ => 0
        };

        return fcb.isMoving 
            && fcb.facilityType == data.facilityType 
            && fcb.temperature == pastTemp 
            && fcb.itemTypeList.FirstOrDefault() == data.pastBathItemType;
      }
      return false;
    });
    if (currentComingCustomer != null)
    {
      currentComingCustomer.Stop();
    }
    
    var fitCustomer = customers.FirstOrDefault(x =>
    {
      if(x.facilityFlow.Count != 0)
      /*if (x.facilityFlow.TryPeek(out var fcb))*/
      {
        var fcb = x.facilityFlow.Peek();
        return fcb.isWaiting 
            && fcb.facilityType == data.facilityType 
            && fcb.temperature == data.temperature
            && fcb.itemTypeList.FirstOrDefault() == data.bathItemType;
      }
      return false;
      });
    if (fitCustomer != null)
    {
      _ = fitCustomer.Move_Facility(data.facilityPosition);
    }
  }

  private void CheckFitCustomer_ShowerBooth(ShowerBoothStateChangeTransportData data)
  {
    var pastFittedCustomer = customers.FirstOrDefault(x =>
    {
      if(x.facilityFlow.Count != 0)
          /*if (x.facilityFlow.TryPeek(out var fcb))*/
      {
        var fcb = x.facilityFlow.Peek();
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
      if(x.facilityFlow.Count != 0)
          /*if (x.facilityFlow.TryPeek(out var fcb))*/
      {
        var fcb = x.facilityFlow.Peek();
        return fcb.isWaiting
            && fcb.facilityType == data.facilityType
            && fcb.temperature == data.temperature;
      }
      return false;
    });
    if (fitCustomer != null)
    {
      if(!fitCustomer.gameObject.activeSelf) fitCustomer.gameObject.SetActive(true);
      _ = fitCustomer.Move_Facility(data.facilityPosition);
    }
  }
  
  
  private void TestMakeCustomer(int name)
  {
    var newCustomer = Instantiate(customerPrefab);
    newCustomer.name =name.ToString();
    newCustomer.transform.position = enterArea.transform.position;
    customers.Add(newCustomer.GetComponent<Customer>());
  }

  public void MakeCustomer()
  {
    var newCustomer = Instantiate(customerPrefab);
    newCustomer.transform.position = enterArea.transform.position;
    var component = newCustomer.GetComponent<Customer>();
    customers.Add(component);
    gameManger.MakeCustomerInfoUI(component);
  }
}
