using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class 按鈕 : MonoBehaviour
{
    public int currentLevel = 0;
    private void Start()
    {
        //DontDestroyOnLoad(this); 
    }
    public void 開始遊戲() {
        currentLevel = 1;
        SceneManager.LoadScene("LV01_基礎場景");
    }
    public void 選擇關卡() {  SceneManager.LoadScene("01_選擇關卡");}
    public void 工作人員() 
    {
        SceneManager.LoadScene("Credit");
    }
    public void 返回() { SceneManager.LoadScene("00homepage"); }
    public void LVSelector() {
        Debug.Log(this.name);

        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        if (selectedButton != null)
        {
            //Debug.Log("按下的按鈕名稱是：" + selectedButton.name);
            SceneManager.LoadScene("LV01_基礎場景");
            currentLevel = int.Parse(selectedButton.name);
        }
        else
        {
            Debug.Log("沒有選定的按鈕");
        }
    }
    public void But_Next()
    {
        //print("PRESS");
        currentLevel = GameObject.Find("00GameMaster").GetComponent<gameMaster>().Level++;
        GameObject.Find("00GameMaster").GetComponent<gameMaster>().Start();
        GameObject.Find("00GameMaster").GetComponent<processCSV>().getLevel(currentLevel);
    }
    public void But_Back()
    {
        SceneManager.LoadScene("01_選擇關卡");
    }
}
