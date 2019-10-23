using System.Net;
using UnityEngine;

using Unity.Networking.Transport;
using Unity.Collections;
using Unity.Jobs;
/*
internal struct ClientUpdateJob : IJob
{
    public UdpNetworkDriver driver;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;

    public void Execute()
    {
        if (!this.connection[0].IsCreated)
        {
            // Remember that its not a bool anymore.
            if (this.done[0] != 1)
            {
                Debug.Log("Something went wrong during connect");
            }

            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        while ((cmd = this.connection[0].PopEvent(this.driver, out stream)) !=
               NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                int value = 1;
                using (DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    this.connection[0].Send(this.driver, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
                uint value = stream.ReadUInt(ref readerCtx);
                Debug.Log("Got the value = " + value + " back from the server");
                // And finally change the `done[0]` to `1`
                this.done[0] = 1;
                this.connection[0].Disconnect(this.driver);
                this.connection[0] = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                this.connection[0] = default(NetworkConnection);
            }
        }
    }
}*/