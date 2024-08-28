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

  public void InitializeFacilityFlow()
  {
    facilityFlow = new ObservableQueue<FacilityControlBlock>();
    facilityFlow.ObserveRemove().Subscribe(removeEvent =>
    {
      FindNextDestination();
    });
    
    {
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.Massage, equipmentType = EquipmentType.Towel});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =FacilityType.Bathtub ,itemTypeList = new List<BathItemType>()
          {
              (BathItemType)Random.Range(0,2)
          }, temperature = Random.Range(33,39)});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =FacilityType.ShowerBooth ,itemTypeList = new List<BathItemType>() { (BathItemType)Random.Range(2,4) }, temperature = Random.Range(33,40)});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.HeaterArea});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.PaymentArea});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.ExitArea});
    }
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
    await UniTask.WaitUntil(() => agent.remainingDistance < 0.2f, cancellationToken: moveCancellationSource.Token);
    if(!agent.isStopped) Stop();
  }
  public async UniTask Move_Facility(Vector3 destination)
  {
    Debug.Log("move to "+destination);
    if (facilityFlow.Count == 0) return;
    var fcb = facilityFlow.Peek();
    agent.isStopped = false;
    fcb.isWaiting = false;
    fcb.isMoving = true;
    animator.SetBool("Move", true);
    agent.SetDestination(new Vector3(destination.x, transform.position.y, destination.z));
    await UniTask.WaitUntil(() => agent.remainingDistance < 2f, cancellationToken: moveCancellationSource.Token);
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
