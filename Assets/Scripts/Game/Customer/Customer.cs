using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    facilityFlow.Enqueue(new FacilityControlBlock(){temperature = 40});
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
