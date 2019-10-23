using CustomMasterClass;
﻿using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

internal struct ServerUpdateConnectionsJob : IJob
{
    public UdpNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public void Execute()
    {
        // CleanUpConnections
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (!this.connections[i].IsCreated)
            {
                this.connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // AcceptNewConnections
        NetworkConnection c;
        while ((c = this.driver.Accept()) != default(NetworkConnection))
        {
            this.connections.Add(c);
            ClientConnectionMsg msg = new ClientConnectionMsg();
            msg.ClientInfo = c;
            Debug.Log("c.id === " +  c.InternalId);
            EventManager.TriggerEvent(GameHandlerData.PlayerJoinedServerHandler, msg);
            Debug.Log("[SERVER] Accepted a connection");
        }
    }
}

internal struct ServerUpdateJob : IJobParallelFor
{
    [ReadOnly]
    public NetworkPipeline pipeline;

    public UdpNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connections;

    public NativeArray<uint> number;

    public void Execute(int index)
    {
        DataStreamReader stream;
        if (!this.connections[index].IsCreated)
        {
            Assert.IsTrue(true);
        }

        NetworkEvent.Type cmd;
        while ((cmd = this.driver.PopEventForConnection(this.connections[index], out stream)) !=
               NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
                this.number[index] += stream.ReadUInt(ref readerCtx);

                Debug.Log("[SERVER] Got ping from the Client. Now " + this.number[index] + " Clients.");

                using (DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(this.number[index]);
                    this.driver.Send(this.pipeline, this.connections[index], writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("[SERVER] Client disconnected from server");
                this.connections[index] = default(NetworkConnection);
            }
        }
    }
}

public class JobifiedServerBehaviour : AsticotMonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkPipeline m_Pipeline;
    public NativeList<NetworkConnection> m_Connections;
    public NativeArray<uint> m_number;
    private JobHandle ServerJobHandle;

    public override void Start()
    private uint numberConnectedClient;
    {
        this.m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        this.m_number = new NativeArray<uint>(16, Allocator.Persistent);
        this.m_Driver = new UdpNetworkDriver(new ReliableUtility.Parameters {WindowSize = 32});
        this.m_Pipeline = this.m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 9000;

        if (this.m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("[SERVER] Failed to bind to port 9000");
        }
        else
        {
            this.m_Driver.Listen();
        }
    }

    public void OnDestroy()
    {
        // Make sure we run our jobs to completion before exiting.
        this.ServerJobHandle.Complete();
        this.m_Connections.Dispose();
        this.m_Driver.Dispose();
        this.m_number.Dispose();
    }

    private void Update()
    {
        this.ServerJobHandle.Complete();

        this.numberConnectedClient = (uint)this.m_number.Sum(x => x);
        //Debug.Log("[SERVER] Connected clients: " + this.numberConnectedClient);

        ServerUpdateConnectionsJob connectionJob = new ServerUpdateConnectionsJob
        {
            driver = this.m_Driver,
            connections = this.m_Connections
        };

        ServerUpdateJob serverUpdateJob = new ServerUpdateJob
        {
            driver = this.m_Driver.ToConcurrent(),
            pipeline = this.m_Pipeline,
            connections = this.m_Connections.AsDeferredJobArray(),
            number = this.m_number
        };

        this.ServerJobHandle = this.m_Driver.ScheduleUpdate();
        this.ServerJobHandle = connectionJob.Schedule(this.ServerJobHandle);
        this.ServerJobHandle = serverUpdateJob.Schedule(this.m_Connections, 1, this.ServerJobHandle);
    }
}