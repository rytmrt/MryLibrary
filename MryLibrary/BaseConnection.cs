/*----------------------------------------------------------------------------*
 * BaseConnection.cs
 * ver 1.0.0 (Apr. 30, 2013)
 *
 * created and maintained by RYOTA MORITA <ryota.morita.3.8@gmail.com>
 * copyright(c) 2013 RYOTA MORITA
 *----------------------------------------------------------------------------*/

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Mry
{
	/// <summary>
	/// BaseConnection.
	/// </summary>
    public class BaseConnection 
	{
		/// <summary>
		/// Receivable.
		/// </summary>
		public interface IReceivable
		{
			/// <summary>
			/// Receive the specified msg.
			/// </summary>
			/// <param name='msg'>
			/// Message.
			/// </param>
			void Receive(string msg);
		}

        /// <summary>
        /// The client.
        /// </summary>
		private TcpClient client;

        /// <summary>
        /// The stream.
        /// </summary>
        private NetworkStream stream;

		/// <summary>
		/// The receive thread.
		/// </summary>
		private Thread receiveThread;

		/// <summary>
		/// The receiver.
		/// </summary>
		private IReceivable receiver;

		/// <summary>
		/// The is closed.
		/// </summary>
		private bool isClosed;


		/// /// <summary>
		/// Initializes a new instance of the <see cref="Mry.BaseConnection"/> class.
		/// </summary>
		/// <param name='host'>
		/// Host.
		/// </param>
		/// <param name='port'>
		/// Port.
		/// </param>
        public BaseConnection(string host, long port)
		{
			isClosed = false;
            client = new TcpClient(host, (int)port);
            stream = client.GetStream();
			Send(" ");
			receiveThread =  new Thread(new ThreadStart(Receive));
			receiveThread.Start();
        }

		/// <summary>
		/// Send the specified msg.
		/// </summary>
		/// <param name='msg'>
		/// Message.
		/// </param>
        public void Send(string msg)
		{
			byte[] data = Encoding.UTF8.GetBytes(msg);
            stream.Write(data, 0, data.Length);
        }

		/// <summary>
		/// Close the receiveThread.
		/// </summary>
		public void Close()
		{
			isClosed = true;
		}

		/// <summary>
		/// Sets the receivable.
		/// </summary>
		/// <param name='receiver'>
		/// Receiver.
		/// </param>
		public void SetReceivable(IReceivable receiver)
		{
			this.receiver = receiver;
		}

		/// <summary>
		/// Receive the message from server
		/// </summary>
		private void Receive()
		{
			byte[] data = new byte[256];

			while (!isClosed) {
				stream.Read(data, 0, data.Length);
				if (receiver != null) {
					receiver.Receive(Encoding.UTF8.GetString(data));
				}
			}
			stream.Close();
			client.Close();
		}
    }
}
