using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
  //Base Component
  private Rigidbody rigidbody;
  private Animator animator;

  //Basic Move Parameter
  [SerializeField] private float movementSpeed = 3.0f;
  private Vector3 movement = new();

  // Dash Parameters
  [SerializeField] private float dashForce = 10f;  // 대시 시 순간적으로 증가할 속도
  [SerializeField] private float dashDuration = 0.2f; // 대시 지속 시간
  [SerializeField] private float dashCooldown = 1f;   // 대시 쿨다운 시간
  private bool dashDown;
  private bool isDashing = false;
  private float lastDashTime = 0f;
  private float dashEndTime = 0f;
  
  //Pick Item Parameter
  private bool interactionDown;
  [SerializeField] private GameObject[] itemOnHands;
  [SerializeField]private GameObject nearObject;

  private void Start()
  {
    animator = GetComponent<Animator>();
    rigidbody = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  private void Update()
  {
    GetInput();
    UpdateState();
    Interaction();
    StartDash();
  }

  private void GetInput()
  {
    movement.x = Input.GetAxisRaw("Horizontal");
    movement.z = Input.GetAxisRaw("Vertical");
    dashDown = Input.GetButtonDown("Dash");
    interactionDown = Input.GetButtonDown("Interaction");
  }


  private void FixedUpdate()
  {
    MoveCharacter();
  }

  private void Interaction()
  {
    if (!interactionDown) return;
    if (!nearObject.gameObject.TryGetComponent<Item>(out var item)) return;
    
    foreach (var itemOnHand in itemOnHands) itemOnHand.SetActive(false);
    itemOnHands[(int)item.itemTag].SetActive(true);
    Destroy(nearObject.gameObject);
    interactionDown = false;
  }

  #region Moving

  private void MoveCharacter()
  {
    movement.Normalize();
    Debug.Log(movement);
    if (!(Mathf.Approximately(movement.x, 0) && Mathf.Approximately(movement.z, 0)))
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 10);
    rigidbody.velocity = (movement * movementSpeed);
  }

  private void UpdateState()
  {
    if (Mathf.Approximately(movement.x, 0) && Mathf.Approximately(movement.z, 0)) animator.SetBool("isMove", false);
    else animator.SetBool("isMove", true);

    animator.SetFloat("xDir", movement.x);
    animator.SetFloat("zDir", movement.z);
  }

  #endregion

  private void StartDash()
  {
    if (!dashDown) return;
    dashDown = false;
    
    isDashing = true;
    lastDashTime = Time.time;
    dashEndTime = Time.time + dashDuration;

    // 대시 방향으로 강한 힘을 가함
    rigidbody.AddForce(movement.normalized * dashForce, ForceMode.VelocityChange);
    // 대시 종료 예약
    Invoke(nameof(EndDash), dashDuration);
  }

  private void EndDash()
  {
    isDashing = false;
    rigidbody.velocity = Vector3.zero;  // 대시 후 순간적으로 멈춤
  }
  
  
  private void OnCollisionStay(Collision other)
  {
    if(!other.gameObject.CompareTag("Floor")) nearObject = other.gameObject;
  }

  private void OnCollisionExit(Collision other)
  {
    if (other.gameObject == nearObject) nearObject = null;
  }
}