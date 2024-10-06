using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FacilityEnterPoint : MonoBehaviour
{
  private IBathingFacility parentFacility;
  private ITemperatureControl parentTemperature;

  private void Awake()
  {
    parentFacility = GetComponentInParent<IBathingFacility>();
    parentTemperature = GetComponentInParent<ITemperatureControl>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!other.gameObject.TryGetComponent<Customer>(out var customer)) return;
    if (!customer.facilityFlow.TryPeek(out var fcb)) return;
    if (customer == parentFacility.CurrentCustomer) return;
    
    if (fcb.facilityType == parentFacility.FacilityType && fcb.temperature == parentTemperature.Temperature)
    {
      Debug.Log("Customer Enter in FEP");
      //customer.GetNavMeshAgent().speed = 0f;
      /*
       * animation 처리 후 마지막에 facility의 CurrentCustomer에 할당
       */
      customer.GetNavMeshAgent().ResetPath();
      customer.GetNavMeshAgent().enabled = false;
      customer.animator.SetBool("Move", false);
      customer.animator.SetBool("In", true);
      customer.gameObject.transform.DOMove(parentFacility.position, 1.1f).SetEase(Ease.InSine)
          .OnComplete(() => parentFacility.CurrentCustomer = customer);
      
    }
  }
}
