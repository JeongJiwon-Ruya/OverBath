using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Customer : MonoBehaviour
{
  public int stress;
  public int moisture;
  public Queue<FacilityControlBlock> facilityFlow;
  public Animator animator;

  [SerializeField]private NavMeshAgent agent;

  private void Start()
  {
    InitializeFacilityFlow();
    if (!animator) animator = GetComponent<Animator>();
    if(!agent) agent = GetComponent<NavMeshAgent>();
  }

  public void InitializeFacilityFlow()
  {
    facilityFlow = new Queue<FacilityControlBlock>();
    var ftype = (FacilityType)Random.Range(1, 2);
    if (ftype == FacilityType.Bathtub)
    {
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =ftype ,itemTypeList = new List<BathItemType>() { BathItemType.Aroma }, temperature = Random.Range(33, 39)});
    }
    else
    {
      facilityFlow.Enqueue(new FacilityControlBlock(){facilityType =ftype, itemTypeList = new List<BathItemType> {BathItemType.Shampoo}, temperature = Random.Range(33, 39)});
    }

    Debug.Log($"Customer : {ftype} & {facilityFlow.Peek().temperature} & {facilityFlow.Peek().itemTypeList[0]}");
  }

  public void Move(Vector3 destination)
  {
    if (!facilityFlow.TryPeek(out var fcb)) return;
    agent.isStopped = false;
    fcb.isWaiting = false;
    fcb.isMoving = true;
    animator.SetBool("Move", true);
    agent.SetDestination(destination);
  }
  
  public void Stop()
  {
    if (!facilityFlow.TryPeek(out var fcb)) return;
    agent.isStopped = true;
    agent.velocity = Vector3.zero;
    fcb.isMoving = false;
    fcb.isWaiting = true;
    Debug.Log("Stop!");
    animator.SetBool("Move", false);
  }
}
