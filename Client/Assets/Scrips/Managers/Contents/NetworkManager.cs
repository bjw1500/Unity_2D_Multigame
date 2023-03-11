using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Google.Protobuf;

public class NetworkManager
{
	ServerSession _session = new ServerSession();
	public string IpAddress = "172.30.1.12";
	public bool _connect = false;
	public bool _isInit = false;

	public void Send(ArraySegment<byte> packet)
	{
		_session.Send(packet);
	}

	public void Send(IMessage packet)
	{
		_session.Send(packet);
	}

	public void Init()
	{
		_isInit = true;

		// DNS (Domain Name System)
		string host = Dns.GetHostName();
		IPHostEntry ipHost = Dns.GetHostEntry(host);

		//IPAddress ipAddr = ipHost.AddressList[1];

		//IPAddress ipAddr = IPAddress.Parse("121.168.117.240");
		IPAddress ipAddr = IPAddress.Parse(IpAddress);
		//System.Net.IPAddress ipAddr = System.Net.IPAddress.Parse("172.30.1.10");

		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

		Debug.Log($"접속 서버 아이피 주소 : {ipAddr}");

		//Connector connector = new Connector();

		//connector.Connect(endPoint,
		//	() => { return _session; },
		//	1);
	}

	public void Connect()
    {
		IPAddress ipAddr = IPAddress.Parse(IpAddress);
		IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);
		Connector connector = new Connector();
		connector.Connect(endPoint,
			() => { return _session; },
			1);

	}


	public void Update()
	{
		List<PacketMessage> list = PacketQueue.Instance.PopAll();
		foreach (PacketMessage packet in list)
		{
			Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
			if (handler != null)
				handler.Invoke(_session, packet.Message);
		}
	}

}
