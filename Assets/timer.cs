using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    // Start is called before the first frame update
    static public float sec = 90;
    public Text timertxt;
    void Update() {
        if(p1.is_gaming){
            if(sec>0)sec -= Time.deltaTime;
            else sec = 0;
        }
        //debug
        if(Input.GetKeyDown("z")){
            sec -= 10;
        }
        Distime(sec);
    }

    // Update is called once per frame
    void Distime(float sec){
        if(sec < 0 ){
            sec = 0;
            p1.is_timeup = true;
        }
        timertxt.text = ((int)sec).ToString();
    }
}
