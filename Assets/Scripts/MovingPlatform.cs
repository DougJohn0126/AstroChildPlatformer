using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform PointA;
    public Transform PointB;
    public float MoveSpeed = 2f;

    private Vector3 NextPosition;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.position = PointA.position;
        NextPosition = PointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(this.transform.position, NextPosition, MoveSpeed *Time.deltaTime);
        if (transform.position == NextPosition)
        {
            NextPosition = (NextPosition == PointA.position) ? PointB.position : PointA.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.parent = null;
        }
    }
}
