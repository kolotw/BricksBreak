using UnityEngine;

public class 回收球 : MonoBehaviour
{
    GameObject 發射器;
    Transform 下一個;
    bool firstComming = false;
    // Start is called before the first frame update
    void Start()
    {
        發射器 = GameObject.Find("/發射器");
        下一個 = GameObject.Find("/下一個發射點").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BALL")
        {
            if (GameObject.Find("/00GameMaster").GetComponent<GameController>().isShooting)
            {
                if (!firstComming)
                {
                    下一個.transform.position = new Vector3(other.transform.position.x, 0.25f, 0f);
                    firstComming = true;
                }
            }
            Destroy(other.gameObject);
        }
        
    }
    public void 移動發射器()
    {
        firstComming = false;
        //發射器.transform.position = 下一個.transform.position;
        發射器.transform.position = Vector3.Lerp(發射器.transform.position, 下一個.transform.position, 0.5f);

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        發射器.transform.rotation = Quaternion.Slerp(發射器.transform.rotation, rotation, 20 * Time.deltaTime);
        發射器.transform.eulerAngles = new Vector3(0f, 發射器.transform.eulerAngles.y, 0f);

    }
}
