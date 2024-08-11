using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  //Base Component
  private new Rigidbody rigidbody;
  private Animator animator;

  //Basic Move Parameter
  [SerializeField] private float movementSpeed = 7f;
  private Vector3 movement;
  
  // Dash Parameters
  [SerializeField] private float dashForce = 10f;  // 대시 시 순간적으로 증가할 속도
  [SerializeField] private float dashDuration = 0.2f; // 대시 지속 시간
  [SerializeField] private float dashCooldown = 1f;   // 대시 쿨다운 시간
  [SerializeField]private bool dashDown;
  private bool isDashing;
  
  //Interaction Parameter
  [SerializeField] private GameObject[] itemOnHands;
  [SerializeField]private GameObject nearObject;
  
  //String Hash Parameter
  private static readonly int IsMove = Animator.StringToHash("isMove");
  private static readonly int XDir = Animator.StringToHash("xDir");
  private static readonly int ZDir = Animator.StringToHash("zDir");
  
  private void Start()
  {
    animator = GetComponent<Animator>();
    rigidbody = GetComponent<Rigidbody>();
  }
  private void Update()
  {
    UpdateState();
  }

  #region Moving
  public void OnMove(InputAction.CallbackContext context)
  {
    var readContext = context.ReadValue<Vector2>();
    movement = new Vector3(readContext.x, 0, readContext.y);
    if (!(Mathf.Approximately(movement.x, 0) && Mathf.Approximately(movement.z, 0)))
    {
      
    }
  }
  private void UpdateState()
  {
    if (Mathf.Approximately(movement.x, 0) && Mathf.Approximately(movement.z, 0))
    {
      animator.SetBool(IsMove, false);
    }
    else
    {
      animator.SetBool(IsMove, true);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movement), Time.deltaTime * 10);
    }
    if(!isDashing) transform.Translate(movement.normalized * (Time.deltaTime * movementSpeed), Space.World);

    animator.SetFloat(XDir, movement.x);
    animator.SetFloat(ZDir, movement.z);
  }
  #endregion

  #region Dash
  public void OnDash(InputAction.CallbackContext context)
  {
    if (!context.performed) return;
    isDashing = true;
    DOTween.To(() => rigidbody.velocity, x => rigidbody.velocity = x, movement * movementSpeed, dashDuration).SetEase(Ease.OutCubic).From(movement * dashForce).OnComplete(EndDash);
  }
  private void EndDash()
  {
    isDashing = false;
    rigidbody.velocity = Vector3.zero;  // 대시 후 순간적으로 멈춤
  }
  #endregion
  
  #region Interaction
  public void OnInteraction(InputAction.CallbackContext context)
  {
    if (!context.performed) return;
    if (!nearObject) return;
    if (nearObject.gameObject.TryGetComponent<Item>(out var item))
    {
      foreach (var itemOnHand in itemOnHands) itemOnHand.SetActive(false);
      itemOnHands[(int)item.itemTag].SetActive(true);
      Destroy(nearObject.gameObject);
    }

    if (nearObject.gameObject.TryGetComponent<TemperatureControlModule>(out var module))
    {
      module.ChangeFacilitiesTemperature();
    }
    
  }
  
  private void OnCollisionStay(Collision other)
  {
    if(!other.gameObject.CompareTag("Floor")) nearObject = other.gameObject;
  }

  private void OnCollisionExit(Collision other)
  {
    if (other.gameObject == nearObject) nearObject = null;
  }
  #endregion
}