using Unity.Collections;
using Unity.Jobs;
using Unity.Networking.Transport;
using UnityEngine;

public class JobifiedClientBehaviour : MonoBehaviour
{
    public UdpNetworkDriver m_Driver;
    public NativeArray<NetworkConnection> m_Connection;
    public NativeArray<byte> m_Done;
    public JobHandle ClientJobHandle;

    // Start is called before the first frame update
    private void Start()
    {
        this.m_Driver = new UdpNetworkDriver(new INetworkParameter[0]);
        this.m_Connection = new NativeArray<NetworkConnection>(1, Allocator.Persistent);
        this.m_Done = new NativeArray<byte>(1, Allocator.Persistent);

        NetworkEndPoint endpoint = NetworkEndPoint.Parse("127.0.0.1", 9000);
        this.m_Connection[0] = this.m_Driver.Connect(endpoint);
    }

    // Update is called once per frame
    private void Update()
    {
        this.ClientJobHandle.Complete();

        ClientUpdateJob job = new ClientUpdateJob
        {
            driver = this.m_Driver,
            connection = this.m_Connection,
            done = this.m_Done
        };
        this.ClientJobHandle = this.m_Driver.ScheduleUpdate();
        this.ClientJobHandle = job.Schedule(this.ClientJobHandle);
    }

    private void OnDestroy()
    {
        this.ClientJobHandle.Complete();

        this.m_Connection.Dispose();
        this.m_Driver.Dispose();
        this.m_Done.Dispose();
    }
}