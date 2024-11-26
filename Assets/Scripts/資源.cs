using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 資源 : MonoBehaviour
{
    public GameObject 縱向刪除;
    public GameObject 橫向刪除;
    public bool 最後要刪除 = false;
    // Start is called before the first frame update
    void Start()
    {
        if(this.name == "-")
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        else if (this.name == "+")
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("BALL"))
        {
            switch (this.transform.name)
            {
                case "Spread":
                    // 分散方向
                    int 角度 = Random.Range(0, 359);
                    collision.transform.eulerAngles = new Vector3(collision.transform.eulerAngles.x,
                        角度,
                        collision.transform.eulerAngles.z);
                    collision.gameObject.GetComponent<Rigidbody>().velocity = collision.transform.forward * 20;
                    最後要刪除 = true;
                    break;
                case "-":
                    
                    break;
                case "+":
                    break;
                default:
                    break;
            }            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "BALL") 
        {
            if(this.transform.name=="-")
            {
                GameObject go = Instantiate(橫向刪除, this.transform.position, Quaternion.identity);
                Destroy(go, 0.05f);
                最後要刪除 = true;
            }
            else if (this.transform.name == "+")
            {
                GameObject go = Instantiate(橫向刪除, this.transform.position, Quaternion.identity);
                GameObject ga = Instantiate(縱向刪除, this.transform.position, Quaternion.identity);
                Destroy(go, 0.05f);
                Destroy(ga, 0.05f);
                最後要刪除 = true;
            }
        }
    }
}
