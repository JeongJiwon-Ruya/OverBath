using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Sauna : MonoBehaviour, IBathingFacility, IPlayerDocking
{
  [SerializeField] private Transform usingPositionTransform;
  [SerializeField] private BathItemType bathItemType;
  
  #region Unity Event
  private void Awake()
  {
    enterPoint = GetComponentInChildren<FacilityEnterPoint>();
    FacilityType = FacilityType.Sauna;
    usingPosition = usingPositionTransform.position;
    bathItemType = BathItemType.Ocher;
  }
  
  
  private void OnCollisionEnter(Collision other)
  {
    if (!other.gameObject.TryGetComponent<Customer>(out var customer)) return;
    if (!customer.facilityFlow.TryPeek(out var fcb)) return;

    if (fcb.facilityType == FacilityType && fcb.itemTypeList.First() == bathItemType)
    {
      Debug.Log("Customer Lock in");
      CurrentCustomer = customer;
    }
  }
  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Sauna, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.Sauna, request: true })
      {
        GameEventBus.Publish(GameEventType.SaunaTypePosition,
            new SaunaTransportData(FacilityType, bathItemType, enterPoint.transform.position));
      }
    });
  }
  
  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.Request_Sauna, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.Sauna, request: true })
      {
        GameEventBus.Publish(GameEventType.SaunaTypePosition,
            new SaunaTransportData(FacilityType, bathItemType, enterPoint.transform.position));
      }
    });
  }
  #endregion
  
  #region IBathingFacility

  public FacilityEnterPoint enterPoint { get; set; }
  public Vector3 usingPosition { get; set; }
  public FacilityType FacilityType { get; set; }

  private Customer currentCustomer;
  public Customer CurrentCustomer { 
    get => currentCustomer;
    set
    {
      if (value)
      {
        currentCustomer = value;
        //CurrentCustomer.gameObject.SetActive(false);
        CustomerProgressRoutine = StartCoroutine(StartCustomerProgressRoutine());
      }
      else
      {
        CurrentCustomer.gameObject.SetActive(true);
        currentCustomer = value;
        GameEventBus.Publish(GameEventType.SaunaTypePosition,
            new SaunaTransportData(FacilityType, bathItemType, enterPoint.transform.position));
      }
    } 
  }
  public Coroutine CustomerProgressRoutine { get; set; }
  
  public IEnumerator StartCustomerProgressRoutine()
  {
    if (!CurrentCustomer.facilityFlow.TryPeek(out var fcb)) yield break;

    while (fcb.progress <= 100)
    {
      Debug.Log(fcb.progress);
      fcb.progress += 5;
      yield return StaticData.progress;
    }

    while (CurrentCustomer)
    {
      Debug.Log(CurrentCustomer.stress);
      CurrentCustomer.stress += 1;
      yield return StaticData.stressProgress;
    }
    
    /*
     * 0.5초당 10씩 참.
     * progress가 100이 되어도 손님을 자동으로 release하지 않음.
     * 그 이후부터는 progress의 속도보다 느리게 stress가 쌓임.
     * 플레이어가 인터랙션해야 나올수 있음.
     */
  }

  public void ReleaseCustomer()
  {
    SpawnBucketManager.SpawnObject(transform.position);
    CurrentCustomer.transform.LookAt(enterPoint.transform.position);
    CurrentCustomer.animator.SetBool($"In_{FacilityType}", false);
    CurrentCustomer.animator.SetBool("Out", true);
    CurrentCustomer.gameObject.transform.DOMove(enterPoint.transform.position, 1.5f).SetEase(Ease.InSine)
        .OnComplete(() =>
        {
          CurrentCustomer.animator.SetBool("Out", false);
          CurrentCustomer.GetNavMeshAgent().enabled = true;
          CurrentCustomer.animator.SetTrigger("OutIdle");
          CurrentCustomer.facilityFlow.Dequeue();
          CurrentCustomer = null;
        });
  }
  #endregion

  #region IPlayerDocking
  public bool IsPlayerIn { get; set; }
  public Player CurrentPlayer { get; set; }
  public bool TrySetPlayer(Player player)
  {
    Debug.Log("Knock");
    ActionInput();
    return false; //player가 도킹상태를 유지할 필요가 없기 때문에 항상 false를 리턴.
  }

  public void ActionInput()
  {
    if (CurrentCustomer)
    {
      ReleaseCustomer();
    }
  }
  #endregion
}