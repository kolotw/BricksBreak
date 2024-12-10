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
        string lv = this.transform.GetChild(0).GetComponent<TextMeshPro>().text;
        print(lv);
    }
}
