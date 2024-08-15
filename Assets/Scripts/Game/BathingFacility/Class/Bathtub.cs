using System.Collections;
using System.Linq;
using ObservableCollections;
using TMPro;
using UnityEngine;

public class Bathtub : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  /// <summary>
  /// 손님이 오는 조건
  /// 1. 온도가 일치해야함
  /// 2. 탕의 종류가 일치해야함
  /// </summary>
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
      StartCoroutine(CustomerProgressRoutine);
    }
  }

  public IEnumerator CustomerProgressRoutine { get; set; }


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


  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public ObservableQueue<BathItemType> BathItems { get; set; }


  private void Start()
  {
    FacilityType = FacilityType.Bathtub;
    CustomerProgressRoutine = StartCustomerProgressRoutine();
    InitializeBathItemFields();
  }

  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 1;
    BathItemTypes = new[] { BathItemType.Water, BathItemType.Aroma };
    BathItems = new ObservableQueue<BathItemType>(BathItemsQueueSize);
  }

  public void ChangeTemperature(TemperatureControlSymbol symbol)
  {
    if (symbol == TemperatureControlSymbol.Plus) Temperature++;
    else if (symbol == TemperatureControlSymbol.Minus) Temperature--;
    GameEventBus.Publish(GameEventType.BathStateChange,
        new BathStateChangeTransportData(FacilityType, symbol, transform.position, Temperature, TryPeekBathItem(),
            TryPeekBathItem())
    );
  }

  public bool TryAddBathItem(BathItemType bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem))
    {
      Debug.Log("Item Type Unmatched!");
      return false;
    }

    var pastBathItem = BathItemType.None;
    if (BathItems.Count == BathItemsQueueSize)
    {
      pastBathItem = BathItems.Dequeue(); // 가장 오래된 항목 제거
      Debug.Log(pastBathItem + " Out!");
    }

    BathItems.Enqueue(bathItem);
    Debug.Log(bathItem + " In!");

    GameEventBus.Publish(GameEventType.BathStateChange,
        new BathStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature,
            bathItem, pastBathItem)
    );

    return true;
  }

  private IEnumerator StartCustomerProgressRoutine()
  {
    var fcb = CurrentCustomer.facilityFlow.Peek();
    var a = new WaitForSeconds(0.1f);

    while (fcb.progress < 100)
    {
      fcb.progress++;
      Debug.Log(fcb.progress);
      yield return a;
    }
  }

  private BathItemType TryPeekBathItem()
  {
    var result = BathItemType.None;
    return BathItems.TryPeek(result) ? result : BathItemType.None;
  }

  public void ReleaseCustomer()
  {
    CurrentCustomer.facilityFlow.Dequeue();
    CurrentCustomer = null;
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
}