using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  //Base Component
  private Rigidbody rigidBody;
  private Animator animator;

  //Basic Move Parameter
  [SerializeField] private float movementSpeed = 7f;
  private Vector3 movement;

  // Dash Parameters
  [SerializeField] private float dashForce = 10f; // 대시 시 순간적으로 증가할 속도
  [SerializeField] private float dashDuration = 0.2f; // 대시 지속 시간
  [SerializeField] private float dashCooldown = 1f; // 대시 쿨다운 시간
  [SerializeField] private bool dashDown;
  private bool isDashing;

  //Interaction Parameter
  [SerializeField] private GameObject[] itemOnHands;
  [SerializeField] private GameObject[] equipmentsOnHands;
  [SerializeField] private GameObject[] cleaningObjectsOnHands;
  [SerializeField] private GameObject nearObject;

  public BathItemType currentBathItem
  {
    get
    {
      var onHand = itemOnHands.FirstOrDefault(x => x.activeSelf);
      return onHand ? onHand.GetComponent<BathItem>().Type : BathItemType.None;
    }
  }

  public EquipmentType currentEquipment
  {
    get
    {
      var onHand = equipmentsOnHands.FirstOrDefault(x => x.activeSelf);
      return onHand ? onHand.GetComponent<PlayerEquipment>().Type : EquipmentType.None;
    }
  }

  //Action Parameter
  private bool isDocking;
  
  //String Hash Parameter
  private static readonly int IsMove = Animator.StringToHash("isMove");
  private static readonly int XDir = Animator.StringToHash("xDir");
  private static readonly int ZDir = Animator.StringToHash("zDir");

  private void Start()
  {
    animator = GetComponent<Animator>();
    rigidBody = GetComponent<Rigidbody>();
  }

  private void Update()
  {
    UpdateState();
  }

  public float dropDistance;

  #region Moving

  public void OnMove(InputAction.CallbackContext context)
  {
    if (isDocking) return;
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

    if (!isDashing) transform.Translate(movement.normalized * (Time.deltaTime * movementSpeed), Space.World);

    animator.SetFloat(XDir, movement.x);
    animator.SetFloat(ZDir, movement.z);
  }

  #endregion

  #region Dash

  public void OnDash(InputAction.CallbackContext context)
  {
    if (!context.performed) return;
    isDashing = true;
    DOTween.To(() => rigidBody.velocity, x => rigidBody.velocity = x, movement * movementSpeed, dashDuration)
        .SetEase(Ease.OutCubic).From(movement * dashForce).OnComplete(EndDash);
  }

  private void EndDash()
  {
    isDashing = false;
    rigidBody.velocity = Vector3.zero; // 대시 후 순간적으로 멈춤
  }

  #endregion

  #region Interaction

  public void OnItemInteraction(InputAction.CallbackContext context)
  {
    if (!context.performed) return;

    if (itemOnHands.Any(x => x.activeSelf))
    {
      HandleItemOnHands();
      return;
    }

    if (equipmentsOnHands.Any(x => x.activeSelf))
    {
      HandleEquipmentOnHands();
      return;
    }

    if (cleaningObjectsOnHands.Any(x => x.activeSelf))
    {
      HandleCleaningObjectOnHands();
      return;
    }
    
    if (nearObject) HandleNearObject();
  }

  private void HandleItemOnHands()
  {
    var activeItem = itemOnHands.First(x => x.activeSelf);

    if (nearObject)
    {
      if (nearObject.gameObject.TryGetComponent<IBathItemHandler>(out var itemHandler))
      {
        if (itemHandler.TryAddBathItem(activeItem.GetComponent<BathItem>().Type))
        {
          DeactivateAllItemsOnHands();
        }
      }
    }
    else
    {
      DropItem(activeItem);
      DeactivateAllItemsOnHands();
    }
  }
  
  private void HandleEquipmentOnHands()
  {
    var activeItem = equipmentsOnHands.First(x => x.activeSelf);

    if (nearObject)
    {
      if (nearObject.gameObject.TryGetComponent<IPlayerDocking>(out var dockingHandler))
      {
        //R키로 도킹하세요.
        Debug.Log("Press R to docking " + dockingHandler.CurrentPlayer.name);
      }
    }
    else
    {
      DropItem(activeItem);
      DeactivateAllEquipmentsOnHands();
    }
  }

  private void HandleCleaningObjectOnHands()
  {
    var activeObject = cleaningObjectsOnHands.First(x => x.activeSelf);
    if (nearObject)
    {
      if (nearObject.TryGetComponent<CleaningBox>(out var cleaningBox))
      {
        if (cleaningBox.TakeCleaningObject(activeObject))
        {
          DeactivateAllCleaningObjectsOnHand();
        }
      }
    }
    else
    {
      DropItem(activeObject);
      DeactivateAllCleaningObjectsOnHand();
    }
  }
  
  private void HandleNearObject()
  {
    if (nearObject.gameObject.TryGetComponent<IWarehouse>(out var warehouse))
    {
      HandleWarehouseInteraction(warehouse);
      return;
    }

    if (nearObject.gameObject.TryGetComponent<BathItem>(out var bathItem))
    {
      HandleBathItemInteraction(bathItem);
    }

    if (nearObject.gameObject.TryGetComponent<PlayerEquipment>(out var equipment))
    {
      HandlePlayerEquipmentInteraction(equipment);
    }

    if (nearObject.gameObject.TryGetComponent<Bucket>(out var cleaningObject))
    {
      HandleCleaningObjectInteraction(cleaningObject);
    }
  }
  
  private void HandleWarehouseInteraction(IWarehouse warehouse)
  {
    var item = warehouse.BathItemOut();
    DeactivateAllItemsOnHands();
    itemOnHands[(int)item].SetActive(true);
  }

  private void HandleBathItemInteraction(BathItem bathItem)
  {
    DeactivateAllItemsOnHands();
    Destroy(bathItem.gameObject);
    itemOnHands[(int)bathItem.Type].SetActive(true);
  }

  private void HandlePlayerEquipmentInteraction(PlayerEquipment equipment)
  {
    DeactivateAllEquipmentsOnHands();
    Destroy(equipment.gameObject);
    equipmentsOnHands[(int)equipment.Type].SetActive(true);
  }

  private void HandleCleaningObjectInteraction(Bucket cleaningObject)
  {
    DeactivateAllCleaningObjectsOnHand();
    GameObjectPool.DespawnObject(cleaningObject.gameObject);
    cleaningObjectsOnHands[0].SetActive(true);
  }
  
  private void DropItem(GameObject item)
  {
    var newObj = Instantiate(item);
    newObj.transform.position = transform.position + transform.forward * dropDistance;
    newObj.transform.localScale = Vector3.one * 3.0f;

    if (newObj.TryGetComponent<Collider>(out var newObjectCollider))
    {
      newObjectCollider.enabled = true;
    }

    newObj.AddComponent<Rigidbody>();
  }

  private void DeactivateAllCleaningObjectsOnHand()
  {
    foreach (var cleaningObjectInHand in cleaningObjectsOnHands) cleaningObjectInHand.SetActive(false);
  }
  
  private void DeactivateAllItemsOnHands()
  {
    foreach (var itemOnHand in itemOnHands) itemOnHand.SetActive(false);
  }
  private void DeactivateAllEquipmentsOnHands()
  {
    foreach (var equipmentOnHand in equipmentsOnHands) equipmentOnHand.SetActive(false);
  }
  
  #endregion

  #region Action
  public void OnAction(InputAction.CallbackContext context)
  {
    if (!context.performed) return;
    if (!nearObject) return;

    HandleNearInteraction();
  }
  public void ReleaseDocking()
  {
    isDocking = false;
  }

  private void HandleNearInteraction()
  {
    if (nearObject.gameObject.TryGetComponent<TemperatureControlModule>(out var module))
    {
      module.ChangeFacilitiesTemperature();
      return;
    }
    if (nearObject.gameObject.TryGetComponent<IPlayerDocking>(out var playerDocking))
    {
      HandleDockingInteraction(playerDocking);
    }
  }
  
  private void HandleDockingInteraction(IPlayerDocking playerDocking)
  {
    if (!isDocking)
    {
      if (playerDocking.TrySetPlayer(this))
      {
        isDocking = true;
        /*
         * 시설에 도킹
         */
      }
    }
    else
    {
      playerDocking.ActionInput();
    }
  }
  #endregion
  
  #region ColliderEvent

  private void OnTriggerStay(Collider other)
  {
    if (!other.CompareTag("Floor")) nearObject = other.gameObject;
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject == nearObject) nearObject = null;
  }

  #endregion
}