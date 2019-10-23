using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

public class JobifiedServerBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NetworkPipeline m_Pipeline;
    public NativeList<NetworkConnection> m_Connections;
    public NativeArray<UInt32> m_number;
    private JobHandle ServerJobHandle;

    private void Start()
    {
        this.m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        this.m_number = new NativeArray<UInt32>(1, Allocator.Persistent);
        this.m_Driver = new UdpNetworkDriver(new ReliableUtility.Parameters { WindowSize = 32 });
        this.m_Pipeline = this.m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

        NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
        endpoint.Port = 9000;

        if (this.m_Driver.Bind(endpoint) != 0)
        {
            Debug.Log("Failed to bind to port 9000");
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