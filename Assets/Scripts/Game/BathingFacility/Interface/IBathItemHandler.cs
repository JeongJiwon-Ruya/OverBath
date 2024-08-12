using System.Collections.Generic;

public interface IBathItemHandler
{
  public BathItemType[] BathItemTypes { get; set; }
  public Queue<BathItem> BathItems { get; set; }

  public bool TryAddBathItem(BathItem bathItem);
}
