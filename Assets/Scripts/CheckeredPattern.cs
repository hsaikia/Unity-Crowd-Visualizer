using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckeredPattern : MonoBehaviour {

	public Texture2D texture;
	public Color kola;
	public Color boga;
	public int sizeX;
	public int sizeY;
	// Use this for initialization
	void Start () {
		makeTexture();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void makeTexture(){
		texture = new Texture2D(sizeX, sizeY);
		for(int i = 0; i < sizeX; i++){
			for(int j = 0; j < sizeY; j++){
				if (((i + j)%2)==1)
            	{
                	texture.SetPixel(i, j, kola);
            	}
            	else
            	{
                	texture.SetPixel(i, j, boga);
            	}
			}
		}
		texture.Apply();
		GetComponent<Renderer>().material.mainTexture = texture;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Point;
	}

}
