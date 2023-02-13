namespace Logic
{
    public class ClientObject : ISnapshot<ClientObjectSnapshot>
    {
        public static ulong NextUid { get; set; }

        protected readonly ClientMain Client;

        public ulong Uid { get; private set; }

        public ClientObject(ClientMain client)
        {
            Client = client;
            Uid = ++NextUid;
        }

        public ClientObject(ClientMain client, ClientObjectSnapshot snapshot)
        {
            Client = client;
            this.RevertToSnapShot(snapshot, false);
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

        public void RevertToSnapShot(ClientObjectSnapshot snapshot, bool needBase)
        {
            Uid = snapshot.Uid;
        }
    }
}