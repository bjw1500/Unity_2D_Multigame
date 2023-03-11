using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using ServerCore;
using System.Net;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();
		static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

		static void TickRoom(GameRoom room, int tick = 100)
        {
			var timer = new System.Timers.Timer();
			timer.Interval = tick;
			timer.Elapsed += ((s, e) => { room.Update(); });
			timer.AutoReset = true;
			timer.Enabled = true;
			_timers.Add(timer);
        }



		static void Main(string[] args)
		{
			ConfigManager.LoadConfig();
			DataManager.LoadData();

			GameRoom room = RoomManager.Instance.Add(2);
			TickRoom(room, 50);

			WaitRoom waitRoom = RoomManager.Instance.WaitRoomAdd();
		
			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[1];
			//IPAddress ipAddr = IPAddress.Parse("121.168.117.240");

			Console.WriteLine($"내부 Server Ip Address :{ipHost.AddressList[1]} : {ipHost.AddressList[0]} \n");
			CheckIpAddress();

			IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");


			while (true)
			{

				Thread.Sleep(100);		

			}
		}


		static void CheckIpAddress()
        {

			try

			{

				string checkipURL = "http://checkip.dyndns.org/";

				WebClient wc = new WebClient();

				UTF8Encoding utf8 = new UTF8Encoding();

				string requestHtml = "";



				requestHtml = utf8.GetString(wc.DownloadData(checkipURL));

				//"<html><head><title>Current IP Check</title></head><body>Current IP Address: 111.222.333.444</body></html>\r\n"

				requestHtml = requestHtml.Substring(requestHtml.IndexOf("Current IP Address:"));

				requestHtml = requestHtml.Substring(0, requestHtml.IndexOf("</body>"));

				requestHtml = requestHtml.Split(':')[1].Trim();



				IPAddress externalIp = null;

				externalIp = IPAddress.Parse(requestHtml);


				string address = externalIp.ToString();

				Console.WriteLine($"외부 Server Ip Address :{address}  \n");


			}

			catch (Exception ex)

			{

				throw ex;

			}

		}

	}
}
