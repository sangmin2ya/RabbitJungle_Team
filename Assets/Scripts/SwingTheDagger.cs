using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwingTheDagger : MonoBehaviour
{
    public float swingSpeed = 500.0f;
    public GameObject swordObject;
    public GameObject yeolpacham;
    //GameObject powerSlash;
    float deltaAngle = 0;
    float deltaTime = 0;
    float coolTime = 0.5f;
    float lifespan = 2.0f;
    float swingAngle = 90.0f;
    Queue<GameObject> q = new Queue<GameObject>();
    // you must get class identification number from player status
    //int playerClass = 404;
    public bool bClass = true;
    //

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && bClass) 
        {
            if(!swordObject.activeSelf)
            {
                swordObject.SetActive(true);

                Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = rotation;

                deltaAngle = 0;

                StartCoroutine("Swing");
            }
        }
        if (Input.GetMouseButtonDown(1) && (deltaTime == 0 || deltaTime >= coolTime) && bClass)
        {
            deltaTime = 0;

            StartCoroutine(PowerSlash(0f));
        }
    }

    IEnumerator Swing()
    {
        while (deltaAngle < swingAngle)
        {
            yield return null;
        
            float delta = swingSpeed * Time.deltaTime;

            transform.Rotate(-Vector3.forward * delta);
            deltaAngle += delta;

            if (deltaAngle > swingAngle)
            {
                swordObject.SetActive(false);
            }
        }
    }
    IEnumerator PowerSlash(float t)
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        GameObject powerSlash = Instantiate(yeolpacham, new Vector2(transform.position.x, transform.position.y) + direction.normalized * 2, rotation);
        q.Enqueue(powerSlash);

        while (true)
        {
            yield return null;
            if (powerSlash.IsDestroyed())
            {
                q.Dequeue();
                deltaTime = 0;
                break;
            }
            Vector3 dir3 = rotation.eulerAngles;
            Vector3 dir = new Vector3(Mathf.Cos(dir3.z * Mathf.Deg2Rad), Mathf.Sin(dir3.z * Mathf.Deg2Rad), 0);
            powerSlash.transform.position += dir.normalized * Time.deltaTime * 50;
            deltaTime += Time.deltaTime;
            t += Time.deltaTime;

            if (t >= lifespan)
            {
                Destroy(q.Dequeue());
                break;
            }
        }
    }
}
