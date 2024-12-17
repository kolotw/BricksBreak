using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class 按鈕 : MonoBehaviour
{
    public void 開始遊戲() {
        SceneManager.LoadScene("LV01");
    }
    public void 選擇關卡() {  SceneManager.LoadScene("01_選擇關卡");}
    public void 工作人員() { }
    public void 返回() { SceneManager.LoadScene("00homepage"); }
    public void LVSelector() {
        print(this.name);

        //string lv = this.transform.GetChild(0).GetComponent<TextMeshPro>().text;
        //print(lv);
    }
    public void LV1() { SceneManager.LoadScene("LV01"); }
    public void LV2() { SceneManager.LoadScene("LV01"); }
    public void LV3() { SceneManager.LoadScene("LV01"); }
    public void LV4() { SceneManager.LoadScene("LV01"); }
    public void LV5() { SceneManager.LoadScene("LV01"); }
    public void LV6() { SceneManager.LoadScene("LV01"); }
    public void LV7() { SceneManager.LoadScene("LV01"); }
    public void LV8() { SceneManager.LoadScene("LV01"); }
    public void LV9() { SceneManager.LoadScene("LV01"); }

    public void But_Next()
    {
        int lv = GameObject.Find("00GameMaster").GetComponent<gameMaster>().Level;
        switch (lv) 
        {
            case 1:
                SceneManager.LoadScene("LV01");
                break;
            default:
                break;
        }
    }
    public void But_Back()
    {
        SceneManager.LoadScene("01_選擇關卡");
    }
}
