namespace Logic
{
    /// <summary>
    /// base class for all objects in logical client
    /// </summary>
    public abstract class ClientObject : ISnapshot<ClientObjectSnapshot>
    {
        public static ulong NextUid { get; set; }

        public ClientMain Client { get;private set; }

        public ulong Uid { get; private set; }

        protected ClientObject(ClientMain client)
        {
            Client = client;
            Uid = ++NextUid;
        }

        protected ClientObject(ClientMain client, ClientObjectSnapshot snapshot)
        {
            Client = client;
            this.RevertFromSnapShot(snapshot);
        }

        protected virtual void InnerUpdate()
        {
        }

        public virtual void LogicUpdate()
        {
            InnerUpdate();
        }

        public void SaveToSnapShot(ClientObjectSnapshot snapshot)
        {
            snapshot.Uid = Uid;
        }

        public void RevertFromSnapShot(ClientObjectSnapshot snapshot)
        {
            Uid = snapshot.Uid;
        }
    }
}