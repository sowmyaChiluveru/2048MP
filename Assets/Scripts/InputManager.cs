using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoveDirection
{
  None,Left,Right,Up,Down
}
public class InputManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //RaycastHit2D hit;
        //    //if(Physics2D.Raycast(Input.mousePosition))
        //}
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {          
            GameManager.Instance.gamePanelScript1.Move(MoveDirection.Right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.Instance.gamePanelScript1.Move(MoveDirection.Left);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {           
            GameManager.Instance.gamePanelScript1.Move(MoveDirection.Up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {            
            GameManager.Instance.gamePanelScript1.Move(MoveDirection.Down);
        }
    }

}
