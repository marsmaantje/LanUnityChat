namespace shared
{
    /**
     * Empty placeholder class for the PlayerInfo object which is being tracked for each client by the server.
     * Add any data you want to store for the player here and make it extend ASerializable.
     */
    public class PlayerInfo 
    {
        public enum PlayerState { LOGIN, LOBBY, PLAYING};
        
        public string Name { get; set; }

        public PlayerState State { get; set; }
    }
}
