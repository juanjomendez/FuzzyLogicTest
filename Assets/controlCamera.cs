using UnityEngine;

public class controlCamera : MonoBehaviour
{

    float R, maxR, minR, angle;


    void Start()
    {

        R = 40;
        minR = 5;
        maxR = 100;
        angle = Mathf.PI;

    }


    void Update()
    {

        if (Input.GetKey(KeyCode.UpArrow) == true)
            R -= 0.5f;
        if (Input.GetKey(KeyCode.DownArrow) == true)
            R += 0.5f;

        R = Mathf.Clamp(R, minR, maxR);

        if (Input.GetKey(KeyCode.LeftArrow) == true)
            angle -= 0.05f;
        if (Input.GetKey(KeyCode.RightArrow) == true)
            angle += 0.05f;

        angle %= (Mathf.PI * 2f);

        transform.localPosition = new Vector3(0 + R * Mathf.Sin(angle), R, 0 + R * Mathf.Cos(angle));
        //transform.localPosition = new Vector3(0,R,0);

        transform.LookAt(Vector3.zero);

    }

}
