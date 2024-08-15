using System.Collections;
using System.Linq;
using ObservableCollections;
using TMPro;
using UnityEngine;

public class ShowerBooth : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  /// <summary>
  /// 손님이 오는 조건
  /// 1. 온도가 일치해야함
  ///
  /// Progress 차는 조건
  /// 1. BathItem이 모두 갖춰져야함.
  /// </summary>

  /// <summary>
  /// Unity Event Function==================================
  /// </summary>
  #region Event Function
  private void Start()
  {
    FacilityType = FacilityType.ShowerBooth;
    CustomerProgressRoutine = StartCustomerProgressRoutine();
    InitializeBathItemFields();
  }
  private void OnCollisionEnter(Collision other)
  {
    if (!other.gameObject.TryGetComponent<Customer>(out var customer)) return;
    if (!customer.facilityFlow.TryPeek(out var fcb)) return;

    if (fcb.facilityType == FacilityType && fcb.temperature == Temperature)
    {
      Debug.Log("Customer Lock in");
      CurrentCustomer = customer;
    }
  }
  
  private void OnDestroy()
  {
    bathItemsQueueView.Dispose();
  }
  #endregion
  /// <summary>
  /// ==================================Unity Event Function End
  /// </summary>
  
  /// <summary>
  /// IBathingFacility==================================
  /// </summary>
  #region IBathingFacility
  public FacilityType FacilityType { get; set; }
  
  private Customer currentCustomer;

  public Customer CurrentCustomer
  {
    get => currentCustomer;
    set
    {
      if (currentCustomer)
      {
        currentCustomer.gameObject.SetActive(true);
        StopCoroutine(CustomerProgressRoutine);
      }

      if (value) value.gameObject.SetActive(false);
      currentCustomer = value;
      CheckBathItemIsFitCurrentCustomer();
    }
  }
  public IEnumerator CustomerProgressRoutine { get; set; }
  
  public IEnumerator StartCustomerProgressRoutine()
  {
    var fcb = CurrentCustomer.facilityFlow.Peek();
    while (fcb.progress < 100)
    {
      fcb.progress++;
      Debug.Log(fcb.progress);
      yield return new WaitForSeconds(0.05f);
    }

    ReleaseCustomer();
  }
  
  public void ReleaseCustomer()
  {
    CurrentCustomer.facilityFlow.Dequeue();
    CurrentCustomer = null;
  }
  #endregion
  /// <summary>
  /// ==================================IBathingFacility End
  /// </summary>
  
  /// <summary>
  /// ITemperatureControl==================================
  /// </summary>
  #region ITemperatureControl
  [SerializeField] private int temperature;
  public int Temperature
  {
    get => temperature;
    set
    {
      temperature = value;
      TemperatureText.text = value.ToString();
    }
  }

  [SerializeField] private TextMeshPro temperatureText;

  public TextMeshPro TemperatureText
  {
    get => temperatureText;
    set => temperatureText = value;
  }
  
  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else Temperature--;
    GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
        new TransportData(FacilityType, symbol, transform.position, Temperature));
  }
  #endregion
  /// <summary>
  /// ==================================ITemperatureControl End
  /// </summary>
  
  /// <summary>
  /// IBathItemHandler==================================
  /// </summary>
  #region IBathItemHandler
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public ObservableQueue<BathItemType> BathItems { get; set; }
  private ISynchronizedView<BathItemType, bool> bathItemsQueueView;


 

  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 3;
    BathItemTypes = new[] { BathItemType.BodyWash, BathItemType.Shampoo };
    BathItems = new ObservableQueue<BathItemType>(BathItemsQueueSize);
    bathItemsQueueView = BathItems.CreateView(_ => CheckBathItemIsFitCurrentCustomer());
  }

  public bool TryAddBathItem(BathItemType bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem))
    {
      Debug.Log("Item Type Unmatched!");
      return false;
    }

    if (BathItems.Count == BathItemsQueueSize)
    {
      var item = BathItems.Dequeue(); // 가장 오래된 항목 제거
      Debug.Log(item + " Out!");
    }

    BathItems.Enqueue(bathItem);
    Debug.Log(bathItem + " In!");
    /*
     * 여기서 손님의 존재 여부에 따라 액션을 달리함
     */
    return true;
  }
  

  #endregion
  /// <summary>
  /// ==================================IBathItemHandler End
  /// </summary>
  
  private bool CheckBathItemIsFitCurrentCustomer()
  {
    if (!CurrentCustomer) return false;
    var fcb = CurrentCustomer.facilityFlow.Peek();
    if (!fcb.itemTypeList.All(x => BathItems.Contains(x))) return false;

    StopCoroutine(CustomerProgressRoutine);
    StartCoroutine(CustomerProgressRoutine);
    return true;
  }
}