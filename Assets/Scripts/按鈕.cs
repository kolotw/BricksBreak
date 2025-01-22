using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class 按鈕 : MonoBehaviour
{

    public void 開始遊戲() {
        currentLevel._CurrentLevel = 1;
        SceneManager.LoadScene("LevelScene");
    }
    public void 選擇關卡() {  SceneManager.LoadScene("01_選擇關卡");}
    public void 工作人員() 
    {
        SceneManager.LoadScene("Credit");
    }
    public void 返回() { SceneManager.LoadScene("00homepage"); }
    public void LVSelector(Button button) {
        if (button != null)
        {
            if (button.name == "R") {
                currentLevel._CurrentLevel = 0; //0 代表是隨機場景
                currentLevel.balls = 30;
            }
            else if(button.name == "K")
            {
                currentLevel._CurrentLevel = -1; //keep shooting
                currentLevel.balls = 999;                
            }
            else {                
                currentLevel._CurrentLevel = int.Parse(button.name);
                currentLevel.balls = 30;
            }
            SceneManager.LoadScene("LevelScene");
        }
        else
        {
            Debug.Log("沒有選定的按鈕");
        }
    }
    public void But_Next()
    {
        if (GameObject.Find("/00GameMaster").GetComponent<GameController>().特殊關卡)
        {
            SceneManager.LoadScene("Credit");
            return;
        }
        GameObject[] bb = GameObject.FindGameObjectsWithTag("BRICKS");
        foreach (GameObject bb2 in bb) { 
            Destroy(bb2);
        }
        currentLevel._CurrentLevel++;
        if (currentLevel._CurrentLevel > 9) 
        {
            SceneManager.LoadScene("Credit");
        }
        else
        {
            SceneManager.LoadScene("LevelScene");
        }
    }
    public void But_Back()
    {
        SceneManager.LoadScene("01_選擇關卡");
    }
    
}
