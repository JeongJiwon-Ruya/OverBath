using System;
using System.Collections;
using System.Linq;
using ObservableCollections;
using TMPro;
using UnityEngine;
using R3;

public class ShowerBooth : MonoBehaviour, IBathingFacility, ITemperatureControl, IBathItemHandler
{
  /// <summary>
  /// 손님이 오는 조건
  /// 1. 온도가 일치해야함
  ///
  /// Progress 차는 조건
  /// 1. BathItem이 모두 갖춰져야함.
  /// </summary>

  private ISynchronizedView<BathItemType, bool> bathItemsQueueView;
  
  
  #region Event Function
  private void Awake()
  {
    FacilityType = FacilityType.ShowerBooth;
    InitializeBathItemFields();
  }
  private void OnCollisionEnter(Collision other)
  {
    if (!other.gameObject.TryGetComponent<Customer>(out var customer)) return;
    /*if (!customer.facilityFlow.TryPeek(out var fcb)) return;*/
    if (customer.facilityFlow.Count == 0) return;
    var fcb = customer.facilityFlow.Peek();
    
    if (fcb.facilityType == FacilityType && fcb.temperature == Temperature)
    {
      Debug.Log("Customer Lock in");
      CurrentCustomer = customer;
    }
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_ShowerBooth, arg =>
    {
      if (!CurrentCustomer && arg.request)
      {
        GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
            new ShowerBoothStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature));
      }
    });
  }

  private void OnDisable()
  {
    GameEventBus.UnSubscribe(GameEventType.Request_ShowerBooth, arg =>
    {
      if (!CurrentCustomer && arg.request)
      {
        GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
            new ShowerBoothStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature));
      }
    });
  }

  private void OnDestroy()
  {
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

      if (value) value.gameObject.SetActive(false);
      else
      {
        GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
            new ShowerBoothStateChangeTransportData(FacilityType, TemperatureControlSymbol.Keep, transform.position, Temperature));
      }
      currentCustomer = value;
      CheckBathItemIsFitCurrentCustomer();
    }
  }
  public Coroutine CustomerProgressRoutine { get; set; }
  
  public IEnumerator StartCustomerProgressRoutine()
  {
    var fcb = CurrentCustomer.facilityFlow.Peek();
    fcb.isUsingNow = true;
    while (fcb.progress < 100)
    {
      fcb.progress++;
      progressText.text = fcb.progress.ToString();
      yield return new WaitForSeconds(0.05f);
    }

    progressText.text = "";
    fcb.isUsingNow = false;
    ReleaseCustomer();
  }
  
  public void ReleaseCustomer()
  {
    CurrentCustomer.facilityFlow.Dequeue();
    CurrentCustomer = null;
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
      //customer의 stress 증가
      Debug.Log("stress up");
    }
    else
    {
      if (symbol == TemperatureControlSymbol.Plus) Temperature++;
      else Temperature--;
      GameEventBus.Publish(GameEventType.ShowerBoothTempStateChange,
          new ShowerBoothStateChangeTransportData(FacilityType, symbol, transform.position, Temperature));
    }
  }
  #endregion
  
  #region IBathItemHandler
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  
  public IObservableCollection<BathItemType> BathItems { get; set; }

  public void InitializeBathItemFields()
  {
    BathItemsQueueSize = 3;
    BathItemTypes = new[] { BathItemType.BodyWash, BathItemType.Shampoo };
    BathItems = new ObservableList<BathItemType>(BathItemsQueueSize);

    BathItems.ObserveCountChanged().Subscribe(_ =>
    {
      bathItemText.text = "";
      foreach (var VARIABLE in BathItems)
      {
        bathItemText.text += $"\n{VARIABLE}";
      }
    });
    BathItems.ObserveAdd().Subscribe(_ =>
    {
      CheckBathItemIsFitCurrentCustomer();
    });
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
      var item = (BathItems as ObservableList<BathItemType>)?.Remove(BathItems.Last()); // 가장 오래된 항목 제거
      Debug.Log(item + " Out!");
    }

    Debug.Log(bathItem + " In!");
    (BathItems as ObservableList<BathItemType>)?.Add(bathItem);
    /*
     * 여기서 손님의 존재 여부에 따라 액션을 달리함
     */
    return true;
  }
  

  #endregion

  [SerializeField] private TextMeshPro bathItemText;
  [SerializeField] private TextMeshPro progressText;
  
  private void CheckBathItemIsFitCurrentCustomer()
  {
    if (!CurrentCustomer) return;
    var fcb = CurrentCustomer.facilityFlow.Peek();
    if (fcb.isUsingNow) return;
    if (!fcb.itemTypeList.All(x => BathItems.Contains(x))) return;
    var sb = "";
    fcb.itemTypeList.ForEach(x =>
    {
      sb += " " + x;
      (BathItems as ObservableList<BathItemType>)?.Remove(x);
    });
    Debug.Log($"{sb} is using now!");
    CustomerProgressRoutine = StartCoroutine(StartCustomerProgressRoutine());
  }
  
}