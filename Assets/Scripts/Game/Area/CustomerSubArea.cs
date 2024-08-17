using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSubArea : MonoBehaviour
{
  private ICustomerArea parentArea;

  private void Awake()
  {
    parentArea = GetComponentInParent<ICustomerArea>();
  }
  
  private void OnTriggerEnter(Collider collider)
  {
    //여기 호출이 안됨
    if (collider.TryGetComponent<Customer>(out var customer))
    {
      Debug.Log("Enter Customer");
      if(customer.facilityFlow.Peek().facilityType == parentArea.facilityType)
        parentArea.AddCustomer(customer);
    }
  }

  /*private void OnCollisionExit(Collision other)
  {
    if (other.gameObject.TryGetComponent<Customer>(out var customer))
    {
      parentArea.isPlayerIn = false;
    }
  }*/
}
