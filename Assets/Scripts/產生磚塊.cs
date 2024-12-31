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
        rounds.text = "ROUND: " + 第幾回合.ToString();
        //偵測勝敗
        磚塊總數 = GameObject.FindGameObjectsWithTag("BRICKS").Length;
        if(磚塊總數 == 0)
        {
            //WIN
            GetComponent<gameMaster>().isWon = true;
        }
    }

    public void genBricks()
    {
        bool 要產生磚塊 = false;
        bPos.z = GetComponent<processCSV>().GetLineCount("/Level/lv01.csv");

        for (int i = 1; i < 9; i++) 
        {
            bPos.x = i;
            bPos.y = 0;

            //bPos.z = 12;
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

    public void levelBricks(int i, int j, string brix, int life)
    {
        Vector3 v3 = Vector3.zero;
        v3.x = (j/2)+1; 
        v3.z = i;
        //bPos.z = GetComponent<processCSV>().GetLineCount("/Level/lv01.csv");
        //i,j 座標
        //brix磚塊 ◢◣◥◤ ■ ⊕⊖❖◉
        GameObject bb;
        switch (brix)
        {
            case "■":
                bb = Instantiate(磚塊[0], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                break;
            case "◢":
                bb = Instantiate(磚塊[1], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                bb.transform.eulerAngles = new Vector3(-90, 0, 90);
                break;
            case "◣":
                bb = Instantiate(磚塊[2], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                bb.transform.eulerAngles = new Vector3(-90, 180, 0);
                break;
            case "◤":
                bb = Instantiate(磚塊[3], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                bb.transform.eulerAngles = new Vector3(-90, 0, -90);
                break;
            case "◥":
                bb = Instantiate(磚塊[4], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                bb.transform.eulerAngles = new Vector3(-90, 0, 0);
                break;
            case "O":
                bb = Instantiate(磚塊[5], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                break;
            case "Y":
                bb = Instantiate(磚塊[6], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                break;
            case "M":
                bb = Instantiate(磚塊[7], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                break;
            case "P":
                bb = Instantiate(磚塊[8], v3, Quaternion.identity);
                bb.GetComponent<boxLife>().life = life;
                break;

            default:
                break;
        }
        
    }

}
