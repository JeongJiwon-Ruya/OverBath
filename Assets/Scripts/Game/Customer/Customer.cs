using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using R3;

public class Customer : MonoBehaviour
{
  public int stress;
  public int moisture = 100;
  public ObservableQueue<FacilityControlBlock> facilityFlow;
  public Animator animator;

  private CancellationTokenSource lifeCycleCancellationSource = new ();
  private CancellationTokenSource moveCancellationSource = new ();
  
  [SerializeField]private NavMeshAgent agent;
  public int useFacilityCount;
  private void Awake()
  {
    if (!animator) animator = GetComponent<Animator>();
    if (!agent) agent = GetComponent<NavMeshAgent>();
    InitializeFacilityFlow();
  }

  private void Start()
  {
    PublishRequestEventToFacility();
  }

  private void OnEnable()
  {
    moveCancellationSource = new CancellationTokenSource();
    lifeCycleCancellationSource = new CancellationTokenSource();
  }

  private void OnDisable()
  {
    moveCancellationSource.Cancel();
    lifeCycleCancellationSource.Cancel();
  }

  private void OnDestroy()
  {
    moveCancellationSource.Dispose();
    lifeCycleCancellationSource.Dispose();
  }

  public NavMeshAgent GetNavMeshAgent()
  {
    return agent;
  }
  
  public void InitializeFacilityFlow()
  {
    facilityFlow = new ObservableQueue<FacilityControlBlock>();
    facilityFlow.ObserveRemove().Subscribe(removeEvent =>
    {
      useFacilityCount++;
      FindNextDestination();
    });

    var temperatureInfo = StageInfoReader.currentStageInfo.Temperature.Split('/');
    (var min, var max) = (int.Parse(temperatureInfo[0]), int.Parse(temperatureInfo[1]));
    
    if (StageInfoReader.currentStageInfo.Bathtub)
    {
      var itemType = StageInfoReader.currentStageInfo.BathtubType.Split('/');
      var itemTypeInit = (BathItemType)Random.Range(0, itemType.Length);
      facilityFlow.Enqueue(new FacilityControlBlock {facilityType = FacilityType.Bathtub, itemTypeList = new List<BathItemType> {itemTypeInit}, temperature = Random.Range(min, max)});
    }

    if (StageInfoReader.currentStageInfo.ShowerBooth)
    {
      var itemType = StageInfoReader.currentStageInfo.ShowerBoothType.Split('/');
      var itemTypeInit = (BathItemType)((int)BathItemType.BodyWash + Random.Range(0, itemType.Length));
      facilityFlow.Enqueue(new FacilityControlBlock {facilityType = FacilityType.ShowerBooth, itemTypeList = new List<BathItemType> {itemTypeInit}, temperature = Random.Range(min, max)});
    }
    if (StageInfoReader.currentStageInfo.Massage)
    {
      var itemType = StageInfoReader.currentStageInfo.MassageType.Split('/');
      var itemTypeInit = (EquipmentType)Random.Range(0, itemType.Length);
      facilityFlow.Enqueue(new FacilityControlBlock {facilityType = FacilityType.Massage, equipmentType = itemTypeInit});
    }
    if (StageInfoReader.currentStageInfo.Sauna)
    {
      var itemType = StageInfoReader.currentStageInfo.SaunaType.Split('/');
      var itemTypeInit = (BathItemType)((int)BathItemType.Ocher + Random.Range(0, itemType.Length));
      facilityFlow.Enqueue(new FacilityControlBlock {facilityType = FacilityType.Sauna, itemTypeList = new List<BathItemType> {itemTypeInit}});
    }
    
    facilityFlow.Enqueue(new FacilityControlBlock {facilityType = FacilityType.PaymentArea});
    facilityFlow.Enqueue(new FacilityControlBlock {facilityType = FacilityType.ExitArea});
  }

  private void FindNextDestination()
  {
    Debug.Log($"{name} Find nextDestination");
    if (facilityFlow.Count == 0) return;
    var fcb = facilityFlow.Peek();
    if (fcb.IsNextDestinationIsArea())
    {
      Debug.Log($"{name} NextIsArea : {fcb.facilityType}");
      GameEventBus.Publish(GameEventType.Request_Area, new RequestTransportData(fcb.facilityType));
    }
    else
    {
      
      PublishRequestEventToFacility();
    }
  }
  
  private void PublishRequestEventToFacility()
  {
    if (!facilityFlow.TryPeek(out var fcb)) return;
    if(fcb.itemTypeList != null)
      Debug.Log($"Customer : {fcb.facilityType} & {fcb.temperature} & {fcb.itemTypeList[0]}");
    else 
      Debug.Log($"Customer : {fcb.facilityType} & {fcb.temperature} & {fcb.equipmentType}");
    switch (fcb.facilityType)
    {
      case FacilityType.Bathtub:
        GameEventBus.Publish(GameEventType.Request_Bathtub, new RequestTransportData(fcb.facilityType));
        break;
      case FacilityType.ShowerBooth:
        GameEventBus.Publish(GameEventType.Request_ShowerBooth, new RequestTransportData(fcb.facilityType));
        break;
      case FacilityType.Massage:
        GameEventBus.Publish(GameEventType.Request_Area, new RequestTransportData(fcb.facilityType));
        break;
      case FacilityType.Sauna:
        GameEventBus.Publish(GameEventType.Request_Sauna, new RequestTransportData(fcb.facilityType));
        break;
      case FacilityType.HeaterArea:
        GameEventBus.Publish(GameEventType.Request_Area, new RequestTransportData(fcb.facilityType));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }
  
  public async UniTask Move_Area(Vector3 destination)
  {
    Debug.Log("move to "+destination);
    if (facilityFlow.Count == 0) return;
    var fcb = facilityFlow.Peek();
    agent.isStopped = false;
    fcb.isWaiting = false;
    fcb.isMoving = true;
    animator.SetBool("Move", true);
    agent.SetDestination(new Vector3(destination.x, transform.position.y, destination.z));
    await UniTask.WaitUntil(() => agent.remainingDistance < 0.1f, cancellationToken: moveCancellationSource.Token);
    if(!agent.isStopped) Stop();
  }
  public async UniTask Move_Facility(Vector3 destination)
  {
    Debug.Log("move to "+destination);
    if (!facilityFlow.TryPeek(out var fcb)) return;
    await UniTask.WaitUntil(() => gameObject.activeSelf);
    agent.isStopped = false;
    fcb.isWaiting = false;
    fcb.isMoving = true;
    animator.SetBool("Move", true);
    agent.SetDestination(new Vector3(destination.x, transform.position.y, destination.z));
    await UniTask.WaitUntil(() => agent.remainingDistance < 0.1f, cancellationToken: moveCancellationSource.Token);
    await UniTask.WaitWhile(() => gameObject.activeSelf);
  }

  public async UniTask Move_Waiting(Vector3 destination)
  {
    moveCancellationSource.Cancel();
    agent.isStopped = false;
    animator.SetBool("Move", true);
    agent.SetDestination(destination);
    await UniTask.WaitUntil(() => agent.remainingDistance < 0.2f, cancellationToken: lifeCycleCancellationSource.Token);
    if(!agent.isStopped) Stop();
    moveCancellationSource = new CancellationTokenSource();
  }
  
  public void Stop()
  {
    if (facilityFlow.Count == 0) return;
    Debug.Log("Stop!");
    var fcb = facilityFlow.Peek();
    /*if (!facilityFlow.TryPeek(out var fcb)) return;*/
    agent.isStopped = true;
    agent.velocity = Vector3.zero;
    fcb.isMoving = false;
    fcb.isWaiting = true;
    animator.SetBool("Move", false);
  }

  public void Die()
  {
    gameObject.SetActive(false);
  }
}
