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
  }

  private void GetInput()
  {
    movement.x = Input.GetAxisRaw("Horizontal");
    movement.z = Input.GetAxisRaw("Vertical");
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

  private void OnCollisionStay(Collision other)
  {
    if(!other.gameObject.CompareTag("Floor")) nearObject = other.gameObject;
  }

  private void OnCollisionExit(Collision other)
  {
    if (other.gameObject == nearObject) nearObject = null;
  }
}