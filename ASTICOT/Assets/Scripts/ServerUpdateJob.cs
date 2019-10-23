using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;

internal struct ServerUpdateJob : IJobParallelFor
{
    public UdpNetworkDriver.Concurrent driver;

    [ReadOnly]
    public NetworkPipeline pipeline;
    public NativeArray<NetworkConnection> connections;
    public NativeArray<UInt32> number;

    public void Execute(int index)
    {
        DataStreamReader stream;
        if (!this.connections[index].IsCreated)
        {
            Assert.IsTrue(true);
        }

        NetworkEvent.Type cmd;
        while ((cmd = this.driver.PopEventForConnection(this.connections[index], out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Data)
            {
                DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
                number[0] += stream.ReadUInt(ref readerCtx);

                Debug.Log("Got " + number[0] + " from the Client adding + 1 to it. Now " + this.number[0]);

                using (DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(number[0]);
                    this.driver.Send(this.pipeline, this.connections[index], writer);
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client disconnected from server");
                this.connections[index] = default(NetworkConnection);
            }
        }
    }
}

internal struct ServerUpdateConnectionsJob : IJob
{
    public UdpNetworkDriver driver;
    public NativeList<NetworkConnection> connections;

    public void Execute()
    {
        // Clean up connections
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (!this.connections[i].IsCreated)
            {
                this.connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = this.driver.Accept()) != default(NetworkConnection))
        {
            this.connections.Add(c);
            Debug.Log("Accepted a connection");
        }
    }
}