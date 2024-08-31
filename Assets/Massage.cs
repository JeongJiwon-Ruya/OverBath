using System;
using System.Collections;
using System.Collections.Generic;
using ObservableCollections;
using TMPro;
using UnityEngine;

public class Massage : MonoBehaviour, IBathingFacility, IPlayerDocking
{
  /*
   * 1. 손님은 일단 와서 들어가있음
   * 2. 플레이어도 가서 들어가야 progress 올라갈 준비가 됨.
   * (손님보다 플레이어가 먼저 들어와있어도 상관없음)
   * 3. 타월 종류 맞춰야 progress올라감.
   * (종류가 맞지 않으면 progress시작 안함)
   */
  
  #region Event Function
  private void Awake()
  {
    FacilityType = FacilityType.Massage;
  }
  
  private void Start()
  {
  }

  private void OnCollisionEnter(Collision other)
  {
    if (!other.gameObject.TryGetComponent<Customer>(out var customer)) return;
    if (!customer.facilityFlow.TryPeek(out var fcb)) return;

    if (fcb.facilityType == FacilityType)
    {
      CurrentCustomer = customer;
    }
  }

  private void OnEnable()
  {
    GameEventBus.Subscribe(GameEventType.Request_Area, arg0 =>
    {
      if (arg0 is { facilityType: FacilityType.Massage, request: true })
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
      if (arg0 is { facilityType: FacilityType.HeaterArea, request: true })
      {
        GameEventBus.Publish(GameEventType.SendAreaPosition,
            new AreaInfoTransportData(FacilityType, transform.position));
      }
    });
  }
  
  #endregion

  #region IBathingFacility

  public FacilityType FacilityType { get; set; }

  private Customer currentCustomer;

  public Customer CurrentCustomer
  {
    get
    {
      return currentCustomer;
    } 
    set
    {
      var tempCustomer = currentCustomer;
      currentCustomer = value;
      if (!value)
      {
        tempCustomer.gameObject.SetActive(true);
      }
      else
      {
        if (currentCustomer.facilityFlow.TryPeek(out var fcb)) currentCustomerFCB = fcb;
        currentCustomer.gameObject.SetActive(false);
        if (CheckReadyToProgress())
        {
          CustomerProgressRoutine = StartCoroutine(StartCustomerProgressRoutine());
        }
      }
    }
  }

  public Coroutine CustomerProgressRoutine { get; set; }

  public IEnumerator StartCustomerProgressRoutine()
  {
    /*
     * 때밀이 액션
     */
    Debug.Log("Start Routine!");
    yield return new WaitUntil(() => currentCustomerFCB.progress >= 100);
    ReleaseCustomer();
  }

  public void ReleaseCustomer()
  {
    CurrentCustomer.facilityFlow.Dequeue();
    CurrentCustomer = null;
    CurrentPlayer = null;
    //player가 꺼지면 input 못받음
  }

  #endregion

  #region IPlayerDocking

  public bool IsPlayerIn { get; set; }
  private Player currentPlayer;
  public Player CurrentPlayer
  {
    get => currentPlayer;
    set
    {
      var tempPlayer = currentPlayer;
      currentPlayer = value;
      if (value)
      {
        if (CheckReadyToProgress())
        {
          CustomerProgressRoutine = StartCoroutine(StartCustomerProgressRoutine());
        }
      }
      else
      {
        tempPlayer.ReleaseDocking();
      }
    }
  }

  public bool TrySetPlayer(Player player)
  {
    /*
     * currentCustomer가 있어야함.
     * customer가 원하는 장비를 차고와야함.
     */
    if (!CurrentCustomer) return false;
    if (currentCustomerFCB.equipmentType != player.currentEquipment) return false;
    //플레이어 정보 저장.
    CurrentPlayer = player;
    //player.gameObject.SetActive(false);
    return true;
  }

  public void ActionInput()
  {
    currentCustomerFCB.progress += 5;
    progressText.text = currentCustomerFCB.progress.ToString();
  }

  #endregion

  [SerializeField] private TextMeshPro progressText;
  private FacilityControlBlock currentCustomerFCB;
  
  private bool CheckReadyToProgress()
  {
    if (currentPlayer)
    {
      if (currentCustomer)
      {
        Debug.Log("Ready!");
        currentCustomer.gameObject.SetActive(false);
        return true;
      }
    }
    
    /*
     * 플레이어 / 손님 객체 할당되었는지 확인
     * 플레이어가 올바를 장비를 들고 왔는지 확인
     */
    return false;
  }
}