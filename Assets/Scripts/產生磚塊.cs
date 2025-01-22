using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class 產生磚塊 : MonoBehaviour
{
    public GameObject[] 磚塊;
    int bID = 0;
    public TextMeshPro tx;
    public TextMeshPro rounds;
    //public int ballNum = 10;
    Vector3 bPos = Vector3.zero;
    // Start is called before the first frame update

    public void genBricks()
    {
        //這是隨機產生磚塊
        int 最多 = 99 + currentLevel.balls + 10;
        int 最少 = currentLevel.balls - 29;

        if (GameObject.Find("00GameMaster").GetComponent<GameController>().特殊關卡)
        {
            最多 = 99 + GameObject.Find("00GameMaster").GetComponent<GameController>().當前回合 + 10;
            最少 = GameObject.Find("00GameMaster").GetComponent<GameController>().當前回合 + 1;
        }

        bool 要產生磚塊 = false;

        bPos.z = 15; //在畫面最上層生成
        bPos.y = 0; //固定Y軸位置
        for (int i = 1; i < 9; i++)  //i是橫向的座標 1-8
        {
            bPos.x = i; 

            int shape = Random.Range(最少, 最多); //隨機生成一個數字
            //用隨機生成的數字，%(一個數值) 以取餘數的方式得到磚塊的ID
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
                //資源 ●
                if (GameObject.Find("00GameMaster").GetComponent<GameController>().特殊關卡) return;
                bID = 5;
                要產生磚塊 = true;
            }
            else if (shape % 57 == 0)
            {
                //資源 ❖
                bID = 6;
                要產生磚塊 = true;
            }
            else if (shape % 37 == 0)
            {
                //資源 一
                bID = 7;
                要產生磚塊 = true;
            }
            else if (shape % 49 == 0)
            {
                //資源 十
                bID = 8;
                要產生磚塊 = true;
            }
            else 
            {
                //不產生
                要產生磚塊 = false;
            }
            if (要產生磚塊)
            {
                //print(bID);
                GameObject bb = Instantiate(磚塊[bID], bPos, Quaternion.identity);
                switch(bb.transform.name)
                {
                    //調整三角型模型的角度
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
                    //資源沒有血量
                }
                else if (bID > 5)
                {

                }
                else
                {
                    bb.GetComponent<boxLife>().life = shape;  //剛隨機生成的數值即為磚塊的血量生命值
                }
                
            }
        }
    }

    public void levelBricks(int i, int j, string brix, int life)
    {
        //這是透過讀取csv產生磚塊
        Vector3 v3 = Vector3.zero;
        v3.x = (j/2)+1; 
        v3.z = i;
        //bPos.z = GetComponent<processCSV>().GetLineCount("/Level/lv01.csv");
        //i,j 座標
        //brix磚塊 ◢◣◥◤ ■ ❖ 十一●
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
            case "●":
                bb = Instantiate(磚塊[5], v3, Quaternion.identity);
                break;
            case "❖":
                bb = Instantiate(磚塊[6], v3, Quaternion.identity);
                break;
            case "一":
                bb = Instantiate(磚塊[7], v3, Quaternion.identity);
                break;
            case "十":
                bb = Instantiate(磚塊[8], v3, Quaternion.identity);
                break;

            default:
                break;
        }
        
    }

}
