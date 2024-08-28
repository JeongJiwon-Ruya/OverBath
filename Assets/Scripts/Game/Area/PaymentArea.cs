using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using UnityEngine;
using R3;

public class PaymentArea : MonoBehaviour, ICustomerArea, IPlayerInteractionArea
{
  /// <summary>
  /// 1. 손님 온 순서대로 줄세우기
  /// 
  /// </summary>

  private Coroutine paymentCoroutine;
  [SerializeField]private Transform[] customersSeat;
  
  // Unity Event Function
  #region Event Function

  private void Awake()
  {
    FacilityType = FacilityType.PaymentArea;
    customers = new ObservableQueue<Customer>();
    customers.ObserveAdd().Subscribe(newCustomer =>
    {
      PlaceNewCustomer(newCustomer.Value);
    });
    customers.ObserveRemove().Subscribe(replaceCustomer =>
    {
      replaceCustomer.Value.facilityFlow.Dequeue();
      ReleaseAndReplaceCustomer(replaceCustomer.Value);
    });
  }

  private void Start()
  {
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.PaymentArea, request: true })
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
      if (arg0 is { facilityType: FacilityType.PaymentArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(FacilityType, transform.position));
      }
    });
  }

  #endregion
  
  //IPlayerArea
  #region IPlayerArea

  [SerializeField] private bool isPlayerIn;
  public Player CurrentPlayer { get; set; }

  public bool IsPlayerIn
  {
    get => isPlayerIn;
    set
    {
      isPlayerIn = value;
      if (value)
      {
        paymentCoroutine = StartCoroutine(PaymentRoutine());
      }
      else
      {
        StopCoroutine(paymentCoroutine);
      }
    }
  }

  #endregion
  
  //ICustomerArea
  #region ICustomerArea

  public FacilityType FacilityType { get; set; }
  public IObservableCollection<Customer> customers { get; set; }
  public void AddCustomer(Customer customer)
  {
    Debug.Log("add " + customer.name);
    (customers as ObservableQueue<Customer>)?.Enqueue(customer);
  }

  public void RemoveCustomer(Customer customer = null)
  {
    Debug.Log("Remove Customer");
    (customers as ObservableQueue<Customer>)?.Dequeue();
  }

  public void PlaceNewCustomer(Customer customer)
  {
    //추가된 후 호출
    //제일 뒤에 배치해야함.
    customer.facilityFlow.Peek().isUsingNow = true;
    _ = customer.Move_Waiting(customersSeat[customers.Count - 1].position);
    /*yield return new WaitForSeconds(1f);
    customer.Stop();*/
  }
  public void ReleaseAndReplaceCustomer(Customer releasedCustomer)
  {
    var index = 0;
    foreach (var customer in customers)
    {
      _ = customer.Move_Waiting(customersSeat[index].position);
      index++;
    }
  }
  
  #endregion

  private IEnumerator PaymentRoutine()
  {
    while (isPlayerIn)
    {
      if(customers.Count != 0 && customers.First().transform.position.IsNear(customersSeat[0].position)) RemoveCustomer();
      yield return new WaitForSeconds(1f);
    }
  } 
}