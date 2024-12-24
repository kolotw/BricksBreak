using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System.Xml;


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
    private string[,] allValue;
    //private string[] sortArray= new string[]{}; //�ѫ��O�ƧǨϥ�

    private string parentName = "";

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(this);
        countAllValue("/Level/lv01.csv");
        getAllValue("/Level/lv01.csv");
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
                    Debug.Log(i + "," + j + ":" + allValue[i, j]);
                    if (allValue[i, j] != string.Empty)
                    {
                        if (j % 2 == 0)
                        {
                            //allValue[i, j] �j��ID
                            brickID = allValue[i, j];
                            print(brickID);
                        }
                        else
                        {
                            //allValue[i, j] �ͩR
                            brickLife = int.Parse(allValue[i, j]);
                            GetComponent<���Ϳj��>().levelBricks(i, j - 2, brickID, brickLife);
                            print(brickLife);
                        }
                    }
                }
            }
        }
        catch { }
        //print(allValue[1,4]);
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
