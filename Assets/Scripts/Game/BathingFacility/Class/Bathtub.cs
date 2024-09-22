using System.Collections;
using System.Linq;
using ObservableCollections;
using TMPro;
using UnityEngine;
using R3;
using UnityEngine.Serialization;

public class Bathtub : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  /// <summary>
  /// 손님이 오는 조건
  /// 1. 온도가 일치해야함
  /// 2. 탕의 종류가 일치해야함
  /// </summary>
  
  #region Event Function
  private void Awake()
  {
    FacilityType = FacilityType.Bathtub;
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
  
  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Bathtub, arg =>
    {
      if (!CurrentCustomer && arg.request )
      {
        GameEventBus.Publish(GameEventType.BathStateChange,
            new BathStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature, TryPeekBathItem(), TryPeekBathItem()));
      }
    });
  }

  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.Request_Bathtub, arg =>
    {
      if (!CurrentCustomer && arg.request)
      {
        GameEventBus.Publish(GameEventType.BathStateChange,
            new BathStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature, TryPeekBathItem(), TryPeekBathItem()));
      }
    });
  }
  #endregion
  
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

      currentCustomer = value;
      if (value)
      {
        value.gameObject.SetActive(false);
        CustomerProgressRoutine = StartCoroutine(StartCustomerProgressRoutine());
      }
      else
      {
        GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
            new ShowerBoothStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature));
      }
    }
  }
  public Coroutine CustomerProgressRoutine { get; set; }
  
  public IEnumerator StartCustomerProgressRoutine()
  {
    if (CurrentCustomer.facilityFlow.TryPeek(out var fcb))
    {
      while (fcb.progress < 100)
      {
        fcb.progress++;
        progressText.text = fcb.progress.ToString();
        yield return StaticData.progress;
      }

      progressText.text = "";
      ReleaseCustomer();
    }
    else
    {
      Debug.Log("Current customer didn't have fcb");
    }
  }
  public void ReleaseCustomer()
  {
    SpawnBucketManager.SpawnObject(transform.position);
    CurrentCustomer.facilityFlow.Dequeue();
    CurrentCustomer = null;
    
    /*GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
        new ShowerBoothStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature));*/
  }
  #endregion
  
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
    if (CurrentCustomer)
    {
      //currentCustomer의 스트레스 증가
      Debug.Log("stress up");
    }
    else
    {
      if (symbol == TemperatureControlSymbol.Plus) Temperature++;
      else if (symbol == TemperatureControlSymbol.Minus) Temperature--;
      GameEventBus.Publish(GameEventType.BathStateChange,
          new BathStateChangeTransportData(FacilityType, symbol, transform.position, Temperature, TryPeekBathItem(),
              TryPeekBathItem()));
    }
  }
  #endregion
  
  #region IBathItemHandler
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public IObservableCollection<BathItemType> BathItems { get; set; }
  
  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 1;
    BathItemTypes = new[] { BathItemType.Water, BathItemType.Aroma };
    BathItems = new ObservableQueue<BathItemType>(BathItemsQueueSize);
    BathItems.ObserveCountChanged().Subscribe(x => bathItemText.text = BathItems.First().ToString());
    (BathItems as ObservableQueue<BathItemType>)?.Enqueue(BathItemType.Water);
  }
  public bool TryAddBathItem(BathItemType bathItem)
  {
    if (BathItemTypes.All(x => x != bathItem))
    {
      Debug.Log("Item Type Unmatched!");
      return false;
    }
    
    if (CurrentCustomer)
    {
      //currentcustomer의 stress증가
      Debug.Log("stress up");
    }
    else
    {
      BathItemType? pastBathItem = BathItemType.None;
      if (BathItems.Count == BathItemsQueueSize)
      {
        pastBathItem = (BathItems as ObservableQueue<BathItemType>)?.Dequeue(); // 가장 오래된 항목 제거
        Debug.Log(pastBathItem + " Out!");
      }

      (BathItems as ObservableQueue<BathItemType>)?.Enqueue(bathItem);
      Debug.Log(bathItem + " In!");
      GameEventBus.Publish(GameEventType.BathStateChange,
          new BathStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature,
              bathItem, pastBathItem));
    }

    return true;
  }
  #endregion

  [SerializeField] private TextMeshPro bathItemText;
  [SerializeField] private TextMeshPro progressText;
  
  private BathItemType? TryPeekBathItem()
  {
    return (BathItems as ObservableQueue<BathItemType>)?.Peek();
  }
}