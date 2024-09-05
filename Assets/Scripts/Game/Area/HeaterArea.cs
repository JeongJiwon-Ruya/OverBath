using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class HeaterArea : MonoBehaviour, ICustomerArea
{
  private Vector2 areaMin;
  private Vector2 areaMax;
  
  #region Event Function

  private void Awake()
  {
    FacilityType = FacilityType.HeaterArea;
    customers = new ObservableList<Customer>();
    customers.ObserveAdd().Subscribe(x =>
    {
      StartCoroutine(DehydrateCustomer(x.Value));
      PlaceNewCustomer(x.Value);
    });
    customers.ObserveRemove().Subscribe(x =>
    {
      x.Value.facilityFlow.Dequeue();
      ReleaseAndReplaceCustomer(x.Value);
    });

    (areaMin, areaMax) = transform.GetMinMax();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.TryGetComponent<Customer>(out var customer))
    {
      if(customer.facilityFlow.Peek().facilityType == FacilityType)
        AddCustomer(customer);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (!other.TryGetComponent<Customer>(out var customer)) return;
    if (customers.Contains(customer))
    {
      (customers as ObservableList<Customer>)?.Remove(customer);
    }
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.HeaterArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(FacilityType, transform.position));
      }
    });
  }
  
  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.HeaterArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(FacilityType, transform.position));
      }
    });
  }

  #endregion
  
  #region ICustomerArea
  public FacilityType FacilityType { get; set; }
  public IObservableCollection<Customer> customers { get; set; }
  public void AddCustomer(Customer customer)
  {
    (customers as ObservableList<Customer>)?.Add(customer);
  }

  public void RemoveCustomer(Customer customer = null)
  {
    (customers as ObservableList<Customer>)?.Remove(customer);
  }

  public void PlaceNewCustomer(Customer customer)
  {
    _ = customer.Move_Waiting(GetRandomPosition());
  }

  public void ReleaseAndReplaceCustomer(Customer releasedCustomer)
  {
    
  }
  #endregion

  private Vector3 GetRandomPosition()
  {
    return new Vector3(Random.Range(areaMin.x, areaMax.x), 0, Random.Range(areaMin.y, areaMax.y));
  }

  private IEnumerator DehydrateCustomer(Customer customer)
  {
    customer.facilityFlow.Peek().isUsingNow = true;
    while (customer.moisture > 0)
    {
        customer.moisture--;
        Debug.Log(customer.moisture);
        yield return StaticData.areaProgress;
    }
    customer.facilityFlow.Peek().isUsingNow = false;
    RemoveCustomer(customer);
  }
}
