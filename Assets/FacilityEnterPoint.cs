using DG.Tweening;
using UnityEngine;

public class FacilityEnterPoint : MonoBehaviour
{
  [SerializeField] private float enterDuration = 1f;
  [SerializeField] private Transform focusTransform;
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
    
    if (fcb.facilityType == parentFacility.FacilityType)
    {
      if (parentTemperature == null || fcb.temperature == parentTemperature.Temperature)
      {
        HandleCustomerEnter(parentFacility, customer);
      }
    }
  }
  void HandleCustomerEnter(IBathingFacility parentFacility, Customer customer)
  {
    Debug.Log("Customer Enter in FEP");
    //customer.GetNavMeshAgent().speed = 0f;
    /*
     * animation 처리 후 마지막에 facility의 CurrentCustomer에 할당
     */
    //customer.GetNavMeshAgent().ResetPath();
    customer.GetNavMeshAgent().enabled = false;
    customer.transform.LookAt(parentFacility.usingPosition);
    customer.animator.SetBool($"Move", false);
    customer.animator.SetBool($"In_{parentFacility.FacilityType}", true);
    customer.gameObject.transform.DOMove(parentFacility.usingPosition, enterDuration).SetEase(Ease.InSine)
        .OnComplete(() =>
        {
          customer.transform.DOLookAt(focusTransform.position, 0.2f).OnComplete(
                () =>
                {
                  customer.animator.SetTrigger($"Use_{parentFacility.FacilityType}");
                  parentFacility.CurrentCustomer = customer;
                });
        });
  }
}
