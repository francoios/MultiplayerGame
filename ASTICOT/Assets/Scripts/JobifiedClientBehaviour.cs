using System.Net;
using CustomMasterClass;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

internal struct ClientUpdateJob : IJob
{
    public UdpNetworkDriver driver;
    public NetworkPipeline pipeline;
    public NativeArray<NetworkConnection> connection;
    public NativeArray<byte> done;

    public void Execute()
    {
        if (!this.connection[0].IsCreated)
        {
            if (this.done[0] != 1)
            {
                Debug.Log("[CLIENT] Something went wrong during connect");
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
                Debug.Log("[CLIENT] We are now connected to the server");

                int value = 1;
                using (DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(value);
                    this.connection[0].Send(this.driver, this.pipeline, writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
                uint value = stream.ReadUInt(ref readerCtx);
                Debug.Log("[CLIENT] Got the value = " + value + " back from the server");
                // And finally change the `done[0]` to `1`
                this.done[0] = 1;
                this.connection[0].Disconnect(this.driver);
                this.connection[0] = default(NetworkConnection);
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("[CLIENT] Client got disconnected from server");
                this.connection[0] = default(NetworkConnection);
            }
        }
    }
}

public class JobifiedClientBehaviour : AsticotMonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkPipeline m_Pipeline;
    public NativeArray<NetworkConnection> m_Connection;
    public NativeArray<byte> m_Done;

    public JobHandle ClientJobHandle;

    public override void Start()
    {
        this.m_Driver = new UdpNetworkDriver(new ReliableUtility.Parameters {WindowSize = 32});
        this.m_Pipeline = this.m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));
        this.m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        this.m_Done = new NativeArray<byte>(1, Allocator.Persistent);
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(IPAddress.Loopback.ToString(), 9000);
        this.m_Connection[0] = this.m_Driver.Connect(endpoint);
    }

    public void OnDestroy()
    {
        this.ClientJobHandle.Complete();
        this.m_Connection.Dispose();
        this.m_Driver.Dispose();
        this.m_Done.Dispose();
    }

    private void Update()
    {
        this.ClientJobHandle.Complete();

        ClientUpdateJob job = new ClientUpdateJob
        {
            driver = this.m_Driver,
            pipeline = this.m_Pipeline,
            connection = this.m_Connection,
            done = this.m_Done
        };
        this.ClientJobHandle = this.m_Driver.ScheduleUpdate();
        this.ClientJobHandle = job.Schedule(this.ClientJobHandle);
    }
}