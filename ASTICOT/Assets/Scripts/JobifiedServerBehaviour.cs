using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

public class JobifiedServerBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NativeList<NetworkConnection> m_Connections;
    private JobHandle ServerJobHandle;

    private void Start()
    {
        this.m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        this.m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);

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
            connections = this.m_Connections.AsDeferredJobArray()
        };

        this.ServerJobHandle = this.m_Driver.ScheduleUpdate();
        this.ServerJobHandle = connectionJob.Schedule(this.ServerJobHandle);
        this.ServerJobHandle = serverUpdateJob.Schedule(this.m_Connections, 1, this.ServerJobHandle);
    }
}