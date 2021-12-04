using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class show : MonoBehaviour{
    public string msg;
    public string[] arr;
    public Vector3[] vec_arr = new Vector3[13];
    public GameObject[] sphere = new GameObject[13];
    private int[,] conn = new int[,]{ {1,3},{3,5},{1,2},{2,4},{4,6},{1,7},{2,8},{7,8},{7,9},{9,11},{8,10},{10,12} };
    public int port = 13000;
    private Thread receiveThread;
    private TcpClient client;
    private TcpListener listener;
    void InitTCP(){
        try{
            receiveThread = new Thread(new ThreadStart(receiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }catch(Exception e){
            print("ERR: "+e.ToString());
        }
        
    }
    private void receiveData(){
        try{
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"),port);
            listener.Start();
            print("INFO: (port:"+port.ToString()+") is listening");
            Byte[] bytes = new Byte[1024];
            while(true){
                client = listener.AcceptTcpClient();
                print("INFO: (port:"+port.ToString()+") connected");
                NetworkStream stream = client.GetStream();
                int length;
                while((length = stream.Read(bytes,0,bytes.Length))!=0){
                    var incommingData = new byte[length];
                    Array.Copy(bytes,0,incommingData,0,length);
                    string clientMessage = Encoding.ASCII.GetString(incommingData);
                    msg = clientMessage;
                    byte[] rmsg = System.Text.Encoding.ASCII.GetBytes("True");
                    stream.Write(rmsg,0,rmsg.Length);
                }
                stream.Close();
                client.Close();
            }
        }catch(Exception e){
            print("ERR: (listener port:"+port.ToString()+") "+e.ToString());
            listener.Stop();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitTCP();
        for(int i = 0 ;i<13;i++){
            sphere[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere[i].transform.position = new Vector3(0, 0, 0);
            sphere[i].transform.localScale = new Vector3(10, 10, 10);
            sphere[i].GetComponent<Renderer>().material.SetColor("_Color",Color.blue);
        }
        sphere[0].GetComponent<Renderer>().material.SetColor("_Color",Color.red);
        sphere[5].GetComponent<Renderer>().material.SetColor("_Color",Color.black);
        sphere[6].GetComponent<Renderer>().material.SetColor("_Color",Color.black);

    }

    // Update is called once per frame
    void Update()
    {
        if(String.Compare(msg,"")!=0){
            // try{
                arr = msg.Split(',');
                for(int i=0;i<13;i++){
                    int j = i*3;
                    vec_arr[i].Set(float.Parse(arr[j]),-float.Parse(arr[j+1]),float.Parse(arr[j+2]));
                    sphere[i].transform.position = vec_arr[i];
                }
                for(int i =0;i<12;i++){
                    Debug.DrawLine(vec_arr[conn[i,0]], vec_arr[conn[i,1]], Color.green);
                }
            // }catch(Exception e){
            //     print(e.ToString());
            // }
        }
        
    }
}
