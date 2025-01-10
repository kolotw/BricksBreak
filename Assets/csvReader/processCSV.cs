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
	 * �n���ΤG���}�C����n�A�o�ˤ]�~���D�����Ǹ�
	 * 
	*/
    public string[,] allValue;
    //private string[] sortArray= new string[]{}; //�ѫ��O�ƧǨϥ�

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
        //�d�߫��O���W�r�A�nreturn�~�y�W�r col ... cmdC=3, cmdE=4, cmdI=5

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
        //�d�ߤW�@�h���W�r�A�nreturn�W�r

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
        //�d�ߤW�@�h���W�r�A�nreturn�W�r(Layer0���^������r)
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
    { //��ܫ��O
        /*
		 * �̾�csv��ƪ����ӿz��
		 * �u�n�ŦX�i���ܼƪ���
		 * �N�C�L
		 * positionI = 8
		 * �o�٬O�q��1�ӦC�X�A�O�]��positionI�O��8��A���O��8�ӡI
		 * int ACC : According Column  �d�߲ĴX�����
		 * string matchVal : �n�ŦX���r
		 * positionI : �����ɡA��ܤU�@���A�q�ĴX�Ӷ}�l�C���`�q0�}�l�C
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
                { //�o��O�T�{�q�ĴX�Ӷ}�l�C�X���O
                  //setICON(i); //i=id
                    pagelimit--;
                    if (pagelimit < 1)
                    { //�p�G�n��m�W�L�Ĥ��ӫ��s�A�N���_
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
            // CSV ��Ƥw���J�� allValue
            Debug.Log($"Data Loaded: {allValue.GetLength(0)} rows, {allValue.GetLength(1)} columns");

            // ���տ�X
            //for (int i = 0; i < allValue.GetLength(0); i++)
            //{
            //    for (int j = 0; j < allValue.GetLength(1); j++)
            //    {
            //        Debug.Log($"Value at [{i}, {j}]: {allValue[i, j]}");
            //    }
            //}
            Debug.Log("CSV Data Loaded. Ready for the next step!");
            print(allValue.GetLength(0) + ", " + allValue.GetLength(1));
            �Ϳj��();
        }));
    }

    public IEnumerator LoadAllValues(string fileName, Action callback)
    {
        string filePath = Application.streamingAssetsPath + "/" + fileName;
        
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL ���x�ϥ� UnityWebRequest
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
            // ��L���x����Ū���ɮ�
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
        // ���Φ�
        string[] rows = csvContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // �p��C�ơ]���]�Ĥ@�檺�C�Ƭ���ǡ^
        int columns = rows[0].Split(',').Length;

        // ��l�� allValue �}�C
        allValue = new string[rows.Length, columns];

        // ��R allValue
        for (int i = 0; i < rows.Length; i++)
        {
            string[] rowValues = rows[i].Split(',');

            for (int j = 0; j < columns; j++)
            {
                allValue[i, j] = j < rowValues.Length ? rowValues[j] : ""; // ��R�ťխ�
            }
        }

        Debug.Log($"CSV Loaded: {rows.Length} rows, {columns} columns.");
        callback?.Invoke();
    }

    void �Ϳj��()
    {
        string brickID = string.Empty;
        int brickLife = 0;
        // ���տ�X
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
                        //allValue[i, j] �j��ID
                        brickID = allValue[i, j];
                    }
                    else
                    {
                        //allValue[i, j] �ͩR
                        brickLife = int.Parse(allValue[i, j]);
                        int newi = (i / 2) + 1;
                        int ii = allValue.GetLength(0) - i;
                        GetComponent<���Ϳj��>().levelBricks(ii, j-1, brickID, brickLife);
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
                            //allValue[i, j] �j��ID
                            brickID = allValue[i, j];
                        }
                        else
                        {
                            //allValue[i, j] �ͩR
                            brickLife = int.Parse(allValue[i, j]);
                            int newi = (i/2) + 1;
                            GetComponent<���Ϳj��>().levelBricks(i, j, brickID, brickLife);                            
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
        // �ˬd�ɮ׬O�_�s�b
        if (File.Exists(filePath))
        {
            // Ū���Ҧ���æ^�Ǧ��
            string[] lines = File.ReadAllLines(filePath);
            return lines.Length;
        }
        else
        {
            Debug.LogError("�ɮפ��s�b: " + filePath);
            return 0;
        }
    }
    public void reWriteCSV(int cmdID, string cmdC)
    {
        /*
		 * �o��O���խ��gcsv�ɡA�D�n�\��
		 * 1. �O���A�C�@�ӵo�������O�A���ӳ��n +1�A�í��g
		 * 2. �]�w�G�s�豶�|
		 * 3. �]�w�G�]�w�Q�I��̦W�r(�~�ҦW�r)�H
		 * 3. �s�W�R�����O(���M�i�H��csv/excel�ۦ�s��A�\��ݨD�٬O���g�U�ӧa)
		 * -----------
		 * ���k
		 * 0. �Ǧ^�e�A���T�{tag�A���OYES(���U�@�h)�����ءA�i����������ACT, Face, ��
		 * 1. �i�Ǧ^�u���O����W�v�A�u���o�����ơv�A�u�ӫ��OID�v
		 * 2. �ھګ��O����W�A�d�߹�����ID
		 * 3. ��CallTimes�����o�A�[1�A�M��A��^��ư}�C
		 * 4. �g�J�榡�T�{
		 * 5. �g�J���|�T�{
		 * 6. �u���g�J�A�ȥ��n��UTF-8
		 * 
		*/
        if (allValue[cmdID, 3] == cmdC)
        {
            //8 : Call Times �n�[1
            int cTime = System.Convert.ToInt32(allValue[cmdID, 8]);
            cTime++;
            if (cTime > 65535) cTime = 0;
            allValue[cmdID, 8] = cTime.ToString();
            //print("command C= " + cmdC + " ID = " + cmdID.ToString() + " callTimes= " + allValue[cmdID,8] + " arrayCount= " + arrayCount.ToString());
            //ID,Layer,Relay,HasSubMenu,CmdC,CmdE,CmdI,CmdV,ShortCut,CallTimes,BodyMap,Stage,AudC,AudE,AudI,AudV,Note1,Note2,Note3\n
            //�Ǹ�,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17\n
            //first and second line
            string reWrite = "";
            reWrite = "ID,Layer,Relay,HasSubMenu,CmdC,CmdE,CmdI,CmdV,ShortCut,CallTimes,BodyMap,Stage,AudC,AudE,AudI,AudV,Note1,Note2,Note3\n�Ǹ�,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17\n";
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
