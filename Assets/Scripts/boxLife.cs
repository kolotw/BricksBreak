using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class boxLife : MonoBehaviour
{
    public int life = 10;
    public TextMeshPro tx;
    float h, s, v;
    Renderer _render;
    // Start is called before the first frame update
    void Start()
    {
        _render = GetComponent<Renderer>();
        s = 1f;
        v = 1f;
        changeColor();
        tx.text = life.ToString(); 
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.tag == "BALL")
        {
            life--;
            tx.text = life.ToString();
            changeColor();
            if (life <= 0)
            {
                if(this.transform.name == "AddBall(Clone)")
                {
                    //GameObject.Find("/發射器").GetComponent<發射器>().球數++;
                    currentLevel.balls++;
                }
                Destroy(this.gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.transform.name == "AddBall(Clone)")
        {
            if (other.transform.tag == "BALL")
            {
                life--;
                tx.text = life.ToString();
                changeColor();
                if (life <= 0)
                {

                    {
                        //GameObject.Find("/發射器").GetComponent<發射器>().球數++;
                        currentLevel.balls++;
                    }
                    Destroy(this.gameObject);
                }
            }
        }
           
    }
    void changeColor()
    {
        float hh = Mathf.Abs(life - 100);
        h = hh / 360;
        Color _color = Color.HSVToRGB(h, s, v);
        _render.material.color = _color;
        changeTextColor();
    }
    void changeTextColor()
    {
        float hh = Mathf.Abs(life - 180);
        h = hh / 360;
        Color _color = Color.HSVToRGB(h, s, 0.3f);
        //tx.renderer.material.color = _color;
        tx.color = _color;
    }
    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.transform.tag == "BALL")
    //    {
    //        life--;
    //        tx.text = life.ToString();
    //        changeColor();
    //        if (life <= 0)
    //        {
    //            Destroy(this.gameObject);
    //        }
    //    }
    //}
}
