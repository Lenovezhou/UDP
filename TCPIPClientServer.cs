using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;


public class TCPIPClientServer : MonoBehaviour {
	public string ip;
	public static Socket ClientSocket;
	public string clientReceiveValue;
	public string clientSendValue;

	void Start()
	{
				Debug.Log(GetIP().ToString());
	}
	private void InitConnect()
	{
		ClientSocket = new Socket (AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		string ip = GetIP().ToString();
		IPAddress ipa = IPAddress.Parse (ip);
		IPEndPoint iep = new IPEndPoint (ipa,8000);
		try 
		{
			ClientSocket.Connect(iep);
			Thread thread=new Thread(new ThreadStart(ClientReceive));
			thread.Start();
		} 
		catch (System.Exception ex)
		{
			Debug.Log (ex.Message);
			clientReceiveValue = ex.Message;
			
		}
	}

	/*接收来自客户端的消息*/
	public void ClientReceive()
	{
		clientReceiveValue = "已建立链接";
		while (true)
		{
			byte[] bytes=new byte[100];
			int rec = ClientSocket.Receive (bytes,bytes.Length,0);
			if (rec<=0) 
			{
				break;
			}
			string strev = System.Text.Encoding.Default.GetString (bytes);
			clientReceiveValue = "服务器对客户端说："+strev+"\r\n";
		}
	}


	private void ClientSend()
	{
		if (ClientSocket.Connected) {
			byte[] SendMessage = new byte[100];
			SendMessage = Encoding.ASCII.GetBytes (clientSendValue);
			//从数据中指定位置开始将数据发送到链接的Socket；
			ClientSocket.Send (SendMessage);
		} 
		else
		{
			Debug.Log ("未建立连接");
			clientSendValue = "未建立连接";
		}
	}


	private string serverReceiveValue="";
	private string serverSendValue = "";
	void OnGUI()

	{

		//服务器

		GUI.TextField(new Rect(100, 100, 200, 50), serverReceiveValue);

		if (GUI.Button(new Rect(310, 100, 100, 20), "开始监听"))

		{

			Listen();

		}

		serverSendValue = GUI.TextField(new Rect(100, 150, 200, 40), serverSendValue);

		if (GUI.Button(new Rect(310, 150, 100, 20), "服务器发送"))

		{

			SeverSend();

		}



		///客户端

		GUI.TextField(new Rect(100, 350, 200, 40), clientReceiveValue);

		if (GUI.Button(new Rect(310, 350, 100, 20), "连接服务器"))

		{

			InitConnect();



		}



		clientSendValue = GUI.TextField(new Rect(100, 400, 200, 40), clientSendValue);

		if (GUI.Button(new Rect(310, 400, 100, 20), "客户端发送"))

		{

			ClientSend();

		}

	}
	private Thread LisThread;
	private Socket LisSocket;
	private int port=8000;//定义侦听端口号
	private Socket newSocket;
	private EndPoint point;
	private string strmes;

	private void Listen()
	{
		LisThread = new Thread (new ThreadStart(BeginListern));
		LisThread.Start();
		serverReceiveValue = GetIP ().ToString () + "正在监听";

	}
	private void BeginListern()
	{
		//实例化Socket
		LisSocket = new Socket (AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		IPAddress ServerIp = GetIP ();/*获取本地IP*/
		IPEndPoint iep = new IPEndPoint (ServerIp,port);
		LisSocket.Bind(iep);/*将Socket绑定ip*/
		LisSocket.Listen(50);//Socket开始监听
		newSocket=LisSocket.Accept();//获取连接请求的Socket
		/*接收客户端Socket所发的信息*/
		while (true) {
			try 
			{
				byte[] byteMessage=new byte[100];
				newSocket.Receive(byteMessage);// 接收消息
				point=newSocket.RemoteEndPoint;// 获取客户端的Socket的相关消息
				IPEndPoint IPpoint=(IPEndPoint)point;
				strmes+=IPpoint.Address.ToString()+"说"+Encoding.Default.GetString(byteMessage).Trim(new char[]{'\0'})+"\r\n";
				serverReceiveValue=strmes;
			} 
			catch (System.Exception ex) 
			{
				Debug.Log (ex.Message);
				serverReceiveValue = ex.ToString ();
			}
		}
	}
	private IPAddress GetIP()
	{
		/*获取本地服务器IP地址*/
		IPHostEntry iep = Dns.GetHostEntry (Dns.GetHostName ());
		IPAddress ip = iep.AddressList [3];
		Debug.Log (ip);
		return ip;
	}
	private void SeverSend()
	{
		byte[] byteData = Encoding.Default.GetBytes (serverSendValue);
		newSocket.Send (byteData);//发送消息即由服务器往客户端
	}

	void OnDestroy()
	{
		LisThread.Abort ();

		ClientSocket.Close ();
		LisSocket.Close ();
		newSocket.Close ();
		Debug.Log ("结束");
	}

}
