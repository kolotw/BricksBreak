using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 按鈕 : MonoBehaviour
{
    //public int currentLevel = 0;
    private void Start()
    {
        //DontDestroyOnLoad(this); 
    }
    public void 開始遊戲() {
        currentLevel._CurrentLevel = 1;
        SceneManager.LoadScene("LV01_基礎場景");
    }
    public void 選擇關卡() {  SceneManager.LoadScene("01_選擇關卡");}
    public void 工作人員() 
    {
        SceneManager.LoadScene("Credit");
    }
    public void 返回() { SceneManager.LoadScene("00homepage"); }
    public void LVSelector(Button button) {
        Debug.Log(button.name);

        if (button != null)
        {
            //Debug.Log("按下的按鈕名稱是：" + selectedButton.name);
            SceneManager.LoadScene("LV01_基礎場景");
            currentLevel._CurrentLevel = int.Parse(button.name);
            //GameObject.Find("00GameMaster").GetComponent<gameMaster>().Start();
        }
        else
        {
            Debug.Log("沒有選定的按鈕");
        }
    }
    public void But_Next()
    {
        //currentLevel++;
        //currentLevel = GameObject.Find("00GameMaster").GetComponent<gameMaster>().Level++;
        //GameObject.Find("按鈕功能選單").GetComponent<按鈕>().currentLevel++;
        currentLevel._CurrentLevel++;
        GameObject.Find("00GameMaster").GetComponent<gameMaster>().Start();
        
    }
    public void But_Back()
    {
        SceneManager.LoadScene("01_選擇關卡");
    }
    
}
