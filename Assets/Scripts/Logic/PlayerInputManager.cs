namespace Logic
{
    /// <summary>
    /// manager for player input,
    /// check input validity, format operation data and send to server
    /// </summary>
    public class PlayerInputManager
    {
        private readonly ClientMain mClient;

        public PlayerInputManager(ClientMain client)
        {
            mClient = client;
        }

        public void SendKeyOperation(KeyType key, KeyOpType opType)
        {
            var operation = new KeyOperation(key, opType);
            mClient.SendToServer(operation);
        }
    }
}