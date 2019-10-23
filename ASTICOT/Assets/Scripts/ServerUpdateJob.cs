using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Assertions;
/*
internal struct ServerUpdateJob : IJobParallelFor
{
    public UdpNetworkDriver.Concurrent driver;
    public NativeArray<NetworkConnection> connections;

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
                uint number = stream.ReadUInt(ref readerCtx);

                Debug.Log("Got " + number + " from the Client adding + 2 to it.");
                number += 2;

                using (DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp))
                {
                    writer.Write(number);
                    this.driver.Send(NetworkPipeline.Null, this.connections[index], writer);
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
}*/