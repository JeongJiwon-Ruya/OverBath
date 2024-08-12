using System.Collections.Generic;

public interface IBathItemHandler
{
  public BathItemType[] BathItemTypes { get; set; }
  public int BathItemsQueueSize { get; set; }
  public Queue<BathItem> BathItems { get; set; }

  public bool TryAddBathItem(BathItem bathItem);
  /// <summary>
  /// Start()에서 호출되어야함.
  /// </summary>
  public void InitializeBathItemFields();
}
