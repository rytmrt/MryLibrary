using System;
using System.Collections;
using System.Collections.Generic;
using Mry;

namespace Test
{
	class MainClass
	{
		class Test : GameConnector.IReceivable
		{
			public void OnReceive (object data)
			{
				var o = data as Dictionary<string, object>;
				Console.WriteLine(o["test"]);
			}
		}
		
		public static void Main (string[] args)
		{
			GameConnector gameConnector = new GameConnector(@"./conf.json");
			gameConnector.CreateConnection("gmsv");
			
			gameConnector.SetReceiveListener("gmsv","test",new MainClass.Test());
			
			gameConnector.AddSendData("gmsv", "test11", "tst");
			gameConnector.AddSendData("gmsv", "test12", "tst");
			gameConnector.AddSendData("gmsv", "test13", "tst");
			gameConnector.Send("gmsv");
			
			gameConnector.Close("gmsv");
		}
	}
}
