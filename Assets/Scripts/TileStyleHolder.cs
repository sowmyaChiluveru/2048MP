using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileStyleHolder : MonoBehaviour
{
    public TileStyle[] tiles;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class TileStyle
{
    public int tileNumber;
    public Color32 tileColor;
    public Color32 textColor;
}
