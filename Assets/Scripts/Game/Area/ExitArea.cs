using System;
using System.Collections;
using System.Collections.Generic;
using ObservableCollections;
using UnityEngine;

public class ExitArea : MonoBehaviour, ICustomerArea
{
  // Start is called before the first frame update
  private void Awake()
  {
    facilityType = FacilityType.ExitArea;
    customers = new ObservableQueue<Customer>();
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.ExitArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(facilityType, transform.position));
      }
    });
  }

  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.ExitArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(facilityType, transform.position));
      }
    });
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.TryGetComponent<Customer>(out var customer))
    {
      if(customer.facilityFlow.Peek().facilityType == facilityType)
        AddCustomer(customer);
    }
  }

  #region ICustomerArea
  public FacilityType facilityType { get; set; }
  public IObservableCollection<Customer> customers { get; set; }

  public void AddCustomer(Customer customer)
  {
    customer.facilityFlow.Dequeue();
    customer.Die();
  }

  public void RemoveCustomer(Customer customer = null)
  {
    throw new System.NotImplementedException();
  }

  public void PlaceNewCustomer(Customer customer)
  {
    throw new System.NotImplementedException();
  }

  public void ReleaseAndReplaceCustomer(Customer releasedCustomer)
  {
    throw new System.NotImplementedException();
  }
  #endregion  
}