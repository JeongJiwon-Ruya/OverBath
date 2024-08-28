using System;
using System.Collections;
using System.Collections.Generic;
using ObservableCollections;
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
      currentCustomer = value;
      if (value)
      {
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
    yield return null;
  }

  public void ReleaseCustomer()
  {
    throw new System.NotImplementedException();
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
      currentPlayer = value;
      if (value)
      {
        if (CheckReadyToProgress())
        {
          CustomerProgressRoutine = StartCoroutine(StartCustomerProgressRoutine());
        }
      }
    }
  }

  public bool TrySetPlayer(Player player)
  {
    //플레이어 정보 저장.
    CurrentPlayer = player;
    player.gameObject.SetActive(false);
    return true;
  }

  #endregion

  private bool CheckReadyToProgress()
  {
    if (!currentPlayer && !currentCustomer) return false;
    Debug.Log("Ready!");
    
    /*
     * 플레이어 / 손님 객체 할당되었는지 확인
     * 플레이어가 올바를 장비를 들고 왔는지 확인
     */
    return true;
  }
}