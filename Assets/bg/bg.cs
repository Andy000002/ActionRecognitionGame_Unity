using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bg : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage rawImage;
    public Texture[] textures = new Texture[14];
    void Start(){
        int now = Random.Range(1,14);
        rawImage.texture = textures[now];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
