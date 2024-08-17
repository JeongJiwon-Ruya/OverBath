using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using UnityEngine;
using R3;

public class PaymentArea : MonoBehaviour, ICustomerArea, IPlayerArea
{
  /// <summary>
  /// 1. 손님 온 순서대로 줄세우기
  /// 
  /// </summary>

  private Coroutine paymentCoroutine;
  [SerializeField]private Transform[] customersSeat;

  //private Coroutine newCustomerRoutine;
  
  //IPlayerArea
  #region IPlayerArea

  [SerializeField] private bool isPlayerIn;
  public bool IsPlayerIn
  {
    get => isPlayerIn;
    set
    {
      if (value)
      {
        paymentCoroutine = StartCoroutine(PaymentRoutine());
      }
      else
      {
        StopCoroutine(paymentCoroutine);
      }
      isPlayerIn = value;
    }
  }

  #endregion
  
  // Unity Event Function
  #region Event Function

  private void Awake()
  {
    facilityType = FacilityType.PaymentArea;
    customers = new ObservableQueue<Customer>();
  }

  private void Start()
  {
    customers.ObserveAdd().Subscribe(newCustomer =>
    {
      //newCustomerRoutine = StartCoroutine(PlaceNewCustomer(newCustomer.Value));
      PlaceNewCustomer(newCustomer.Value);
    });
    customers.ObserveRemove().Subscribe(replaceCustomer =>
    {
      ReleaseAndReplaceCustomer(replaceCustomer.Value);
    });
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.PaymentArea, request: true })
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
      if (arg0 is { facilityType: FacilityType.PaymentArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(facilityType, transform.position));
      }
    });
  }

  #endregion
  
  //ICustomerArea
  #region ICustomerArea

  public FacilityType facilityType { get; set; }
  public IObservableCollection<Customer> customers { get; set; }
  public void AddCustomer(Customer customer)
  {
    Debug.Log("add " + customer.name);
    (customers as ObservableQueue<Customer>)?.Enqueue(customer);
  }

  public void RemoveCustomer()
  {
    Debug.Log("Remove Customer");
    (customers as ObservableQueue<Customer>)?.Dequeue();
  }

  public void PlaceNewCustomer(Customer customer)
  {
    //추가된 후 호출
    //제일 뒤에 배치해야함.
    customer.facilityFlow.Peek().isUsingNow = true;
    customer.Move(customersSeat[customers.Count - 1].position);
    /*yield return new WaitForSeconds(1f);
    customer.Stop();*/
  }
  public void ReleaseAndReplaceCustomer(Customer releasedCustomer)
  {
    Debug.Log("ReleaseNReplaceCustomer " + releasedCustomer.name);
    //두번째 손님부터 정상 처리 X
    //시설 이용이 끝났을때 & 최초 손님 생성시 시설상태 업데이트가 안되고있음
    
    //제거된 뒤 호출
    //인자로 받은 손님은 알아서 다른곳 가겠고,
    //뒤에 서있던 손님들 한포지션씩 앞으로 이동
    releasedCustomer.facilityFlow.Dequeue();
    var index = 0;
    foreach (var customer in customers)
    {
      customer.Move(customersSeat[index].position);
      index++;
    }
  }
  
  #endregion

  private IEnumerator PaymentRoutine()
  {
    while (true)
    {
      if(customers.Count != 0 && customers.First().transform.position.IsNear(customersSeat[0].position, 1f)) RemoveCustomer();
      yield return new WaitForSeconds(1f);
    }
  } 
}