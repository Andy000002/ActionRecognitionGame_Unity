using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

public class p1 : MonoBehaviour{
    private Animator anim;
    public Animator enemy;
    private string msg = null;
    private string[] skill_arr= { "attack", "defense", "skill_1", "skill_2", "skill_3", "skill_4", "jump"};
    static public int enemy_hp;
    public int port = 12333, ehp, my_hp;
    private int last_dmg,buff_dmg;
    public RectTransform enemy_health;
    static public bool is_gaming = false, is_start = false, is_end = false,is_timeup = false,is_pause = false;
    bool is_defense, is_idle;
    bool is_out = false;
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
                    if(client.Connected!=false){
                        byte[] rmsg = System.Text.Encoding.ASCII.GetBytes(is_gaming.ToString());
                        stream.Write(rmsg,0,rmsg.Length);
                        print(is_gaming.ToString());
                    }
                }
                stream.Close();
                client.Close();
            }
        }catch(Exception e){
            print("ERR: (listener port:"+port.ToString()+") "+e.ToString());
            listener.Stop();
        }
    }
    void Start(){
        anim = GetComponent<Animator>();
        my_hp = enemy.GetComponent<p1>().ehp;
        ehp = enemy_hp = 100;
        last_dmg = buff_dmg = 0;
        is_gaming = false;
        is_start = false;
        is_end = false;
        is_timeup = false;
        is_pause = false;
        InitTCP();
    }
    void check_win(){
        if(enemy.GetCurrentAnimatorStateInfo(0).IsName("win")||enemy.GetCurrentAnimatorStateInfo(0).IsName("win0")){
            if(!anim.GetCurrentAnimatorStateInfo(0).IsName("lose")){
                anim.Play("lose");  
            }
        }
    }
    bool isinskill(string skill){
        int pos = Array.IndexOf(skill_arr, skill);
        if (pos > -1)
            return true;
        else
            return false;
    }
    // bool isPlaying(Animator animLayer, string stateName){
    //     print(animLayer.GetCurrentAnimatorStateInfo(0).ToString());
    //     if (animLayer.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
    //             animLayer.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
    //         return true;
    //     else
    //         return false;
    // }
    void Deal_msg(){
        if(msg == null)return;
        if(String.Compare(msg,"end")==0){// end
            print("INFO: (msg) "+msg);
            is_timeup = true;
            client.Close();
            listener.Stop();
            receiveThread.Interrupt();
            receiveThread.Abort();
        }
        else if(String.Compare(msg,"start")==0&&!is_start){//start
            print("INFO: (msg) "+msg);
            is_start = true;
            is_gaming = true;
            is_pause = false;
        }
        else{
            //in game
            if(is_gaming){
                print("INFO: (msg) "+msg);
                if(String.Compare(msg,"damaged")==0){// dmg
                        anim.SetTrigger("damaged");
                }
                else if(String.Compare(msg,"None")==0){//out of screen
                    anim.SetBool("out",true);
                }
                else if(String.Compare(msg,"pause")==0){//pause
                    is_pause = true;
                }
                else if(String.Compare(msg,"resume")==0){//resume
                    is_pause = false;
                }
                else{
                    if(is_idle && isinskill(msg)){//skill
                        anim.SetBool("out",false);
                        if(String.Compare(msg,"jump")==0){
                            anim.SetTrigger("jump");
                        }
                        //attack
                        else if(String.Compare(msg,"attack")==0){
                            anim.SetTrigger("attack");
                            last_dmg = 3;
                        }
                        else if(String.Compare(msg,"skill_1")==0){
                            anim.SetTrigger("skill_1");
                            last_dmg = 5;
                        }
                        else if(String.Compare(msg,"skill_2")==0){
                            anim.SetTrigger("skill_2");
                            last_dmg = 5;
                        }
                        else if(String.Compare(msg,"skill_3")==0){
                            anim.SetTrigger("skill_3");
                            buff_dmg = 3;
                        }
                        else if(String.Compare(msg,"skill_4")==0){
                            anim.SetTrigger("skill_4");
                            buff_dmg = 3;
                        }
                        else if(String.Compare(msg,"defense")==0){
                            anim.SetTrigger("defense");
                        }
                    }
                    else{
                        print("ERR: (msg) "+msg.ToString());
                    }
                }
            }
            else{
                print("ERR: (msg) "+msg.ToString());
            }
        }
        msg = null;
    }
    void Deal_status(){
        if(is_gaming && ehp<=0){//real win
            print("INFO: "+anim.name.ToString()+" win");
            is_gaming = false;
            is_end = true;
            anim.Play("win");
        }
        else if(is_timeup && is_gaming){//time up
            print("INFO: time up");
            is_end = true;
            is_gaming = false;
            if(ehp < my_hp){
                anim.Play("win");
                enemy.Play("lose");
            }
            else if(ehp == my_hp){
                print("INFO: tie");
                anim.Play("tie");
                enemy.Play("tie");
            }
        }
    }

    // Update is called once per frame
    void Update(){
        //DEBUG
        if(Input.GetKeyDown("a")){
            msg = "start";
        }
        if(Input.GetKeyDown("s")){
            msg = "pause";
        }
        if(Input.GetKeyDown("d")){
            msg = "resume";
        }
        
        is_defense = enemy.GetCurrentAnimatorStateInfo(0).IsName("defense");
        is_idle = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")||anim.GetCurrentAnimatorStateInfo(0).IsName("out");

        Deal_msg();
        Deal_status();
        check_win();
        
        if(!is_defense && last_dmg != 0){
            enemy.SetTrigger("damaged");
            ehp = ehp - last_dmg - buff_dmg;
            last_dmg = buff_dmg =  0;
        }
        my_hp = enemy.GetComponent<p1>().ehp;
        enemy_hp = ehp;
        // check_win();
        enemy_health.sizeDelta = new Vector2(
            enemy_hp, 
            enemy_health.sizeDelta.y);
    }
    void OnApplicationQuit(){
        print("INFO: Application end");
        listener.Stop();
        receiveThread.Interrupt();
        receiveThread.Abort();
    }
}
