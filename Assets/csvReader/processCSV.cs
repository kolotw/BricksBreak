using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Xml;
using Unity.VisualScripting;
using System;
using UnityEngine.Networking;


public class processCSV : MonoBehaviour
{

    private int valueLimit = 2000;
    private int arrayCount = 0;
    //get CSV file
    cmgw eyeCmd = new cmgw();
    /*
	 * 好像用二維陣列比較好，這樣也才知道它的序號
	 * 
	*/
    public string[,] allValue;
    //private string[] sortArray= new string[]{}; //供指令排序使用

    private string parentName = "";

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(this);
        //getLevel(1);
    }
    public void getLevel(int level)
    {
        //int level = GetComponent<gameMaster>().Level;
        string levelName = "/lv" + string.Format("{0:D2}", level) + ".csv";
        

        //countAllValue(levelName);
        //getAllValue(levelName);
        StartCoroutine(InitializeValues(levelName));
        //valueLimit = GetLineCount(levelName);
        
    }
    public string getTRN(string matchVal, int col)
    {
        //查詢指令的名字，要return外語名字 col ... cmdC=3, cmdE=4, cmdI=5

        for (int i = 0; i < arrayCount; i++)
        {
            if (allValue[i, 3] == matchVal)
            {
                parentName = allValue[i, col];
                return parentName;
            }
        }
        return null;
    }
    public string getParentMenu(string matchVal)
    {
        //查詢上一層的名字，要return名字

        for (int i = 0; i < arrayCount; i++)
        {
            if (allValue[i, 3] == matchVal)
            {
                parentName = allValue[i, 1];
                return parentName;
            }
        }
        return null;
    }
    public string getParentKEY(string matchVal)
    {
        //查詢上一層的名字，要return名字(Layer0的英文關鍵字)
        for (int i = 0; i < arrayCount; i++)
        {
            if (allValue[i, 3] == matchVal)
            {
                parentName = allValue[i, 0];
                return parentName;
            }
        }
        return null;
    }

    public void ShowCmd(int ACC, string matchVal, int positionI)
    { //顯示指令
        /*
		 * 依據csv資料表的欄位來篩選
		 * 只要符合進來變數的值
		 * 就列印
		 * positionI = 8
		 * 卻還是從第1個列出，是因為positionI是第8行，不是第8個！
		 * int ACC : According Column  查詢第幾個欄位
		 * string matchVal : 要符合的字
		 * positionI : 換頁時，顯示下一頁，從第幾個開始。正常從0開始。
		*/
        //posI=0;
        int matchCmd = 0;
        int pagelimit = 7;
        //clearBtn();
        //GM.thisLayerCommandsCount = 0;
        for (int i = 0; i < arrayCount; i++)
        {
            if (allValue[i, ACC] == matchVal)
            {
                //GM.thisLayerCommandsCount++;
                matchCmd++;
                if (matchCmd > positionI)
                { //這邊是確認從第幾個開始列出指令
                  //setICON(i); //i=id
                    pagelimit--;
                    if (pagelimit < 1)
                    { //如果要放置超過第六個按鈕，就切斷
                        return;
                    }
                }
            }
        }
        Invoke("resetIconPosition", 0.01f);
    }
    void countAllValue(string myFN)
    {
        string tm;
        try
        {
            for (int i = 1; i < valueLimit; i++)
            {
                arrayCount++;
                for (int j = 0; j < 18; j++)
                {
                    tm = eyeCmd.getRowByID(myFN, i)[j];
                }

            }
        }
        catch
        {
            //print(arrayCount.ToString());
            valueLimit = arrayCount + 1;
            arrayCount = 0;
            allValue = new string[valueLimit, 18];
        }
    }

    IEnumerator InitializeValues(string filename)
    {
        //CSVReader reader = GetComponent<CSVReader>();
        yield return StartCoroutine(LoadAllValues(filename, () =>
        {
            // CSV 資料已載入到 allValue
            Debug.Log($"Data Loaded: {allValue.GetLength(0)} rows, {allValue.GetLength(1)} columns");

            // 測試輸出
            //for (int i = 0; i < allValue.GetLength(0); i++)
            //{
            //    for (int j = 0; j < allValue.GetLength(1); j++)
            //    {
            //        Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}");
            //    }
            //}
            Debug.Log("CSV Data Loaded. Ready for the next step!");
            print(allValue.GetLength(0) + ", " + allValue.GetLength(1));
            生磚塊();
        }));
    }

    public IEnumerator LoadAllValues(string fileName, Action callback)
    {
        string filePath = Application.streamingAssetsPath + "/" + fileName;
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL 平台使用 UnityWebRequest
            GameObject.Find("/Canvas/filepath").GetComponent<Text>().text = filePath;
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load CSV: " + request.error);
                yield break;
            }

            ParseCSV(request.downloadHandler.text, callback);
        }
        else
        {
            // 其他平台直接讀取檔案
            try
            {
                string fileContent = System.IO.File.ReadAllText(filePath);
                ParseCSV(fileContent, callback);
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading CSV file: " + e.Message);
            }
        }
    }

    private void ParseCSV(string csvContent, Action callback)
    {
        // 分割行
        string[] rows = csvContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // 計算列數（假設第一行的列數為基準）
        int columns = rows[0].Split(',').Length;

        // 初始化 allValue 陣列
        allValue = new string[rows.Length, columns];

        // 填充 allValue
        for (int i = 0; i < rows.Length; i++)
        {
            string[] rowValues = rows[i].Split(',');

            for (int j = 0; j < columns; j++)
            {
                allValue[i, j] = j < rowValues.Length ? rowValues[j] : ""; // 填充空白值
            }
        }

        Debug.Log($"CSV Loaded: {rows.Length} rows, {columns} columns.");
        callback?.Invoke();
    }

    void 生磚塊()
    {
        string brickID = string.Empty;
        int brickLife = 0;
        // 測試輸出
        print(allValue.GetLength(0) + " " + allValue.GetLength(1));
        for (int i = allValue.GetLength(0)-1; i > 0; i--)
        {
            for (int j = 1; j < allValue.GetLength(1); j++)
            {
                Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}" +  " = " + allValue[i,j]);
                if (allValue[i, j].Length > 0)
                {
                    if (j % 2 == 1)
                    {
                        //allValue[i, j] 磚塊ID
                        brickID = allValue[i, j];
                    }
                    else
                    {
                        //allValue[i, j] 生命
                        brickLife = int.Parse(allValue[i, j]);
                        int newi = (i / 2) + 1;
                        int ii = allValue.GetLength(0) - i;
                        GetComponent<產生磚塊>().levelBricks(ii, j-1, brickID, brickLife);
                    }
                }
            }
        }      
    }
    void getAllValue(string myFN)
    { 
        //Read All Value
        //print(valueLimit);
        string brickID = "";
        int brickLife = 0;

        try
        {
            for (int i = 1; i < valueLimit; i++)
            {
                arrayCount++;
                for (int j = 0; j < 18; j++)
                {
                    allValue[i, j] = eyeCmd.getRowByID(myFN, i)[j];
                    //Debug.Log(i + "," + j + ":" + allValue[i, j]);
                    if (allValue[i, j] != string.Empty)
                    {
                        if (j % 2 == 0)
                        {
                            //allValue[i, j] 磚塊ID
                            brickID = allValue[i, j];
                        }
                        else
                        {
                            //allValue[i, j] 生命
                            brickLife = int.Parse(allValue[i, j]);
                            int newi = (i/2) + 1;
                            GetComponent<產生磚塊>().levelBricks(i, j, brickID, brickLife);                            
                        }
                    }
                }
            }
        }
        catch 
        {
            //print("Wrong? ");
        }
    }
    public int GetLineCount(string filePath)
    {
        filePath = Application.dataPath + filePath;
        // 檢查檔案是否存在
        if (File.Exists(filePath))
        {
            // 讀取所有行並回傳行數
            string[] lines = File.ReadAllLines(filePath);
            return lines.Length;
        }
        else
        {
            Debug.LogError("檔案不存在: " + filePath);
            return 0;
        }
    }
    public void reWriteCSV(int cmdID, string cmdC)
    {
        /*
		 * 這邊是嘗試重寫csv檔，主要功能
		 * 1. 記次，每一個發音的指令，應該都要 +1，並重寫
		 * 2. 設定：編輯捷徑
		 * 3. 設定：設定被呼喚者名字(外勞名字)？
		 * 3. 新增刪除指令(雖然可以用csv/excel自行編輯，功能需求還是先寫下來吧)
		 * -----------
		 * 做法
		 * 0. 傳回前，先確認tag，不是YES(有下一層)的項目，可接受的應有ACT, Face, 空
		 * 1. 可傳回「指令中文名」，「取得的次數」，「該指令ID」
		 * 2. 根據指令中文名，查詢對應的ID
		 * 3. 把CallTimes欄位取得，加1，然後再放回資料陣列
		 * 4. 寫入格式確認
		 * 5. 寫入路徑確認
		 * 6. 真正寫入，務必要轉UTF-8
		 * 
		*/
        if (allValue[cmdID, 3] == cmdC)
        {
            //8 : Call Times 要加1
            int cTime = System.Convert.ToInt32(allValue[cmdID, 8]);
            cTime++;
            if (cTime > 65535) cTime = 0;
            allValue[cmdID, 8] = cTime.ToString();
            //print("command C= " + cmdC + " ID = " + cmdID.ToString() + " callTimes= " + allValue[cmdID,8] + " arrayCount= " + arrayCount.ToString());
            //ID,Layer,Relay,HasSubMenu,CmdC,CmdE,CmdI,CmdV,ShortCut,CallTimes,BodyMap,Stage,AudC,AudE,AudI,AudV,Note1,Note2,Note3\n
            //序號,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17\n
            //first and second line
            string reWrite = "";
            reWrite = "ID,Layer,Relay,HasSubMenu,CmdC,CmdE,CmdI,CmdV,ShortCut,CallTimes,BodyMap,Stage,AudC,AudE,AudI,AudV,Note1,Note2,Note3\n序號,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17\n";
            try
            {
                for (int i = 1; i < arrayCount; i++)
                {
                    for (int j = 0; j < 18; j++)
                    {
                        if (j == 0)
                        {
                            reWrite = reWrite + i.ToString() + ",";
                        }
                        if (j == 17)
                        {
                            reWrite = reWrite + allValue[i, j] + "\n";
                        }
                        else
                        {
                            reWrite = reWrite + allValue[i, j] + ",";
                        }

                        //allValue[i,j] = eyeCmd.getRowByID(myFN,i)[j];
                    }

                }
            }
            catch { }
            //print(reWrite);
            System.IO.File.WriteAllText(Application.dataPath + "/EYEC.csv", reWrite, Encoding.UTF8);
        }
    }
}
