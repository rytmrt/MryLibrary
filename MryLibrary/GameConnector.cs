/*----------------------------------------------------------------------------*
 * GameConnection.cs
 * ver 1.0.0 (May. 2, 2013)
 *
 * created and maintained by RYOTA MORITA <ryota.morita.3.8@gmail.com>
 * copyright(c) 2013 RYOTA MORITA
 *----------------------------------------------------------------------------*/
using System;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

namespace Mry
{
	/// <summary>
	/// Game connector.
	/// </summary>
	public class GameConnector
	{
		
		/// <summary>
		/// Receivable.
		/// </summary>
		public interface IReceivable
		{
			/// <summary>
			/// Raises the receive event.
			/// </summary>
			/// <param name='data'>
			/// Data.
			/// </param>
			void OnReceive (object data);
		}
		
		/// <summary>
		/// Server infomation.
		/// </summary>
		public struct ServerInfo
		{
			/// <summary>
			/// The name.
			/// </summary>
			public string name;
			
			/// <summary>
			/// The host.
			/// </summary>
			public string host;
			
			/// <summary>
			/// The port.
			/// </summary>
			public long port;
		}


		/// <summary>
		/// Server connector.
		/// </summary>
		class ServerConnector : Connection.IReceivable
		{
			private Connection con;
			private Dictionary<string, IReceivable> receiveListener;
			private Dictionary<string, object> sendData;
			
			/// <summary>
			/// Initializes a new instance of the <see cref="Mry.GameConnector.ServerConnector"/> class.
			/// </summary>
			/// <param name='host'>
			/// Host.
			/// </param>
			/// <param name='port'>
			/// Port.
			/// </param>
			public ServerConnector (string host, long port)
			{
				con = new Connection (host, port);
				con.SetReceivable(this);
				receiveListener = new Dictionary<string, IReceivable> ();
				sendData = new Dictionary<string, object> ();
			}
			
			public void Close ()
			{
				con.Close();
			}

			/// <summary>
			/// Sets the receive listener.
			/// </summary>
			/// <param name='key'>
			/// Key.
			/// </param>
			/// <param name='receive'>
			/// Receive.
			/// </param>
			public void SetReceiveListener (string key, IReceivable receive)
			{
				receiveListener [key] = receive;
			}
			
			/// <summary>
			/// Adds the send data.
			/// </summary>
			/// <param name='key'>
			/// Key.
			/// </param>
			/// <param name='data'>
			/// Data.
			/// </param>
			public void AddSendData (string key, object data)
			{
				sendData [key] = data;
			}
			
			/// <summary>
			///Ssend the object in stack.
			/// </summary>
			public void Send ()
			{
				var str = Json.Serialize (sendData);
				con.Send (str);
				sendData.Clear ();
			}
			
			/// <summary>
			/// Receive the msg.
			/// </summary>
			/// <param name='msg'>
			/// Message.
			/// </param>
			public void Receive (string msg)
			{
				var obj = Json.Deserialize (msg) as Dictionary<string, object>;
				receiveListener [(string)obj ["on"]].OnReceive (obj["contents"]);
			}
		}
		
		/// <summary>
		/// The servers.
		/// </summary>
		private Dictionary<string, ServerConnector> servers;
		
		private Dictionary<string, ServerInfo> serverInfo;
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Mry.GameConnector"/> class.
		/// </summary>hn5b   nv
		public GameConnector (string confJsonPath)
		{
			servers = new Dictionary<string, ServerConnector> ();
			serverInfo = new Dictionary<string, ServerInfo> ();
			CreateServerInfo(confJsonPath);
		}
		
		/// <summary>
		/// Sets the receive listener.
		/// </summary>
		/// <param name='server'>
		/// Server.
		/// </param>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='receivable'>
		/// Receivable.
		/// </param>
		public void SetReceiveListener (string server, string key, IReceivable receivable)
		{
			servers [server].SetReceiveListener (key, receivable);
		}
		
		/// <summary>
		/// Adds the send data.
		/// </summary>
		/// <param name='server'>
		/// Server.
		/// </param>
		/// <param name='key'>
		/// Key.
		/// </param>
		/// <param name='data'>
		/// Data.
		/// </param>
		public void AddSendData (string server, string key, object data)
		{
			servers [server].AddSendData (key, data);
		}
		
		/// <summary>
		/// Send to the specified server.
		/// </summary>
		/// <param name='server'>
		/// Server.
		/// </param>
		public void Send (string server)
		{
			servers [server].Send ();
		}
		
		/// <summary>
		/// Creates the connection.
		/// </summary>
		/// <returns>
		/// Is connected.
		/// </returns>
		/// <param name='name'>
		/// server name.
		/// </param>
		public bool CreateConnection (string name)
		{
			if (serverInfo.ContainsKey(name)) {
				servers[name] = new ServerConnector(serverInfo[name].host, serverInfo[name].port);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Close the specified server.
		/// </summary>
		/// <param name='name'>
		/// Name.
		/// </param>
		public void Close (string name)
		{
			servers[name].Close();
		}
		
		/// <summary>
		/// Creates the server info.
		/// </summary>
		private void CreateServerInfo (string path)
		{
			string config =System.IO.File.ReadAllText(path);
			
			var objs = Json.Deserialize (config) as List<object>;

			foreach (var i in objs) {
				var obj = i as Dictionary<string, object>;
				
				ServerInfo svinfo = new ServerInfo();
				svinfo.name = (string)obj["name"];
				svinfo.host = (string)obj["host"];
				svinfo.port = (Int64)obj["port"];
				serverInfo[svinfo.name] = svinfo;
 				//servers [(string)obj["name"]] = new ServerConnector ((string)obj["host"], (Int64)obj["port"]);
			}
		}
	}
}

