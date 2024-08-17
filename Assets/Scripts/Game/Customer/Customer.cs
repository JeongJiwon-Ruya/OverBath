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
  public int moisture;
  public ObservableQueue<FacilityControlBlock> facilityFlow;
  public Animator animator;

  
  [SerializeField]private NavMeshAgent agent;

  private void Awake()
  {
    if (!animator) animator = GetComponent<Animator>();
    if (!agent) agent = GetComponent<NavMeshAgent>();
    InitializeFacilityFlow();
  }

  public void InitializeFacilityFlow()
  {
    facilityFlow = new ObservableQueue<FacilityControlBlock>();
    facilityFlow.ObserveRemove().Subscribe(removeEvent =>
    {
      FindNextDestination();
    });
    
    //TestCode
    /*if (gameObject.name.Contains("1"))*/
    {
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =FacilityType.Bathtub ,itemTypeList = new List<BathItemType>() { BathItemType.Water }, temperature = Random.Range(37,38)});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =FacilityType.ShowerBooth ,itemTypeList = new List<BathItemType>() { BathItemType.Shampoo }, temperature = Random.Range(36,37)});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.PaymentArea});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.ExitArea});
    }
    /*else
    {
      //facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =FacilityType.Bathtub ,itemTypeList = new List<BathItemType>() { BathItemType.Water }, temperature = Random.Range(38,39)});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =FacilityType.ShowerBooth, itemTypeList = new List<BathItemType> {BathItemType.Shampoo}, temperature = Random.Range(36, 37)});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.PaymentArea});
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType = FacilityType.ExitArea});
    }*/
    PublishRequestEventToFacility();
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
    if (facilityFlow.Count == 0) return;
    var fcb = facilityFlow.Peek();
    Debug.Log($"Customer : {fcb.facilityType} & {fcb.temperature} & {fcb.itemTypeList[0]}");
    switch (fcb.facilityType)
    {
      case FacilityType.Bathtub:
        GameEventBus.Publish(GameEventType.Request_Bathtub, new RequestTransportData(fcb.facilityType));
        break;
      case FacilityType.ShowerBooth:
        GameEventBus.Publish(GameEventType.Request_ShowerBooth, new RequestTransportData(fcb.facilityType));
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private float epsilon = 0.01f;
  
  public async UniTask Move(Vector3 destination)
  {
    if (facilityFlow.Count == 0) return;
    var fcb = facilityFlow.Peek();
    agent.isStopped = false;
    fcb.isWaiting = false;
    fcb.isMoving = true;
    animator.SetBool("Move", true);
    agent.SetDestination(destination);
    await UniTask.WaitUntil(() => transform.position.IsNear(destination, epsilon), cancellationToken: this.GetCancellationTokenOnDestroy());
    if(!agent.isStopped) Stop();
  }
  
  public void Stop()
  {
    if (facilityFlow.Count == 0) return;
    var fcb = facilityFlow.Peek();
    /*if (!facilityFlow.TryPeek(out var fcb)) return;*/
    agent.isStopped = true;
    agent.velocity = Vector3.zero;
    fcb.isMoving = false;
    fcb.isWaiting = true;
    Debug.Log("Stop!");
    animator.SetBool("Move", false);
  }

  public void Die()
  {
    gameObject.SetActive(false);
  }
}
