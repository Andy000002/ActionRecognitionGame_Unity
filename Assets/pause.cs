using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pause : MonoBehaviour
{
    public GameObject pauseUI,player1,player2;
    public Text txt;

    private int p1_hp,p2_hp;
    void Start(){
        
    }
    // Update is called once per frame
    void Update(){
        if(!p1.is_start){
            txt.text = "READY...";
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
        else{
            txt.text = "PAUSE";
        }
        if(!p1.is_pause&&p1.is_start){
            pauseUI.SetActive(false);
            Time.timeScale = 1;
        }
        else if(p1.is_start){
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }

        if(p1.is_end){
            p1_hp = player1.GetComponent<p1>().ehp;
            p2_hp = player2.GetComponent<p1>().ehp;
            if(p1_hp>p2_hp){
                txt.text = "P2 win"; 
            }
            else if(p1_hp<p2_hp){
                txt.text = "P1 win"; 
            }
            else{
                txt.text = "Tie"; 
            }
            pauseUI.SetActive(true);
        }
    }
}
