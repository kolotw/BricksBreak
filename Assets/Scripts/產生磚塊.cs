using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class 產生磚塊 : MonoBehaviour
{
    public GameObject[] 磚塊;
    int bID = 0;
    int bLife = 10;
    public TextMeshPro tx;
    public TextMeshPro rounds;
    public int ballNum = 10;
    Vector3 bPos = Vector3.zero;

    public int 第幾回合 = 0;
    GameObject[] 現場磚塊;
    int 磚塊總數 = 0;  
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tx.text = "Ball Number: " + GameObject.Find("/發射器").GetComponent<發射器>().球數;
        rounds.text = "計算次數: " + 第幾回合.ToString();
        //偵測勝敗
        磚塊總數 = GameObject.FindGameObjectsWithTag("BRICKS").Length;
        if(磚塊總數 == 0)
        {
            //WIN
            GameObject.Find("/00GameMaster").GetComponent<gameMaster>().isWon = true;
        }
    }

    public void genBricks()
    {
        bool 要產生磚塊 = false;
        for (int i = 1; i < 9; i++) 
        {
            bPos.x = i;
            bPos.y = 0;
            bPos.z = 12;
            int shape = Random.Range(1, 99);
            if (shape % 12 == 0)
            {
                //三角形 ID: 1,2,3,4
                bID = Random.Range(1,4); 
                要產生磚塊 = true;
            }
            else if (shape % 5 == 0)
            {
                //方形
                bID = 0;
                要產生磚塊 = true;
            }
            else if (shape % 9 == 0)
            {
                //資源
                bID = 5;
                要產生磚塊 = true;
            }
            else 
            {
                //不產生
                要產生磚塊 = false;
            }
            if (要產生磚塊)
            {
                GameObject bb = Instantiate(磚塊[bID], bPos, Quaternion.identity);
                switch(bb.transform.name)
                {
                    case "◥(Clone)":
                        bb.transform.eulerAngles = new Vector3(-90, 0, 0);
                        break;
                    case "◣(Clone)":
                        bb.transform.eulerAngles = new Vector3(-90, 180, 0);
                        break;
                    case "◤(Clone)":
                        bb.transform.eulerAngles = new Vector3(-90, 0, -90);
                        break;
                    case "◢(Clone)":
                        bb.transform.eulerAngles = new Vector3(-90, 0, 90);
                        break;
                    default: break;
                }
                if (bID == 5)
                {
                    bb.GetComponent<boxLife>().life = 1;
                }
                else
                {
                    bb.GetComponent<boxLife>().life = shape;
                }
                
            }
        }
    }


}
