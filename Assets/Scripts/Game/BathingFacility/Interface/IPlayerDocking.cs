public interface IPlayerDocking
  {
    public bool IsPlayerIn { get; set; }
    public Player CurrentPlayer { get; set; }
    public bool TrySetPlayer(Player player);

    public void ActionInput();

  }