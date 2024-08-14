using System.Collections.Generic;

public interface IBathItemHandler
{
  public BathItemType[] BathItemTypes { get; set; } //허용되는 아이템 종류
  public int BathItemsQueueSize { get; set; } //시설에 최대로 들어가는 아이템 개수
  public Queue<BathItemType> BathItems { get; set; } //현재 적용되어있는 아이템 목록

  public bool TryAddBathItem(BathItemType bathItem);
  /// <summary>
  /// Start()에서 호출되어야함.
  /// </summary>
  public void InitializeBathItemFields();
}
