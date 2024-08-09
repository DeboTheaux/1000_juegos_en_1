using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public TipoDeMovimiento tipoDeMovimiento;
    [HideInInspector] public string horizontalAxis;
    [HideInInspector] public string verticalAxis;
    [HideInInspector] public bool pointAndClick = false;
    [HideInInspector] public bool puedeSaltar = false;
    public LayerMask floorLayer; //PROGRAMAR DESDE GAMEMANAGER
    [Header("Velocidad de movimiento del Personaje")]
    public float velocidad = 1.5f;
    [Header("Fuerza del salto")]
    [Tooltip("Solo funciona si hemos activado el salto")]
    public float fuerza = 5f;

    float jumpStop = 0.3f;
    Vector2 jumpRange = new Vector2(0.5f, 2f);
    bool isJump = false;

    float movX = 0;
    float movY = 0;

    Vector2 movement;

    Rigidbody2D rb;
    Transform checkFloor;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        checkFloor = transform.Find("CheckFloor");
    }


    void FixedUpdate()
    {
        switch (tipoDeMovimiento)
        {
            case TipoDeMovimiento.DosDirecciones:
                movX = Input.GetAxis(horizontalAxis);
                movement = new Vector3(movX * velocidad, rb.velocity.y);
                break;
            case TipoDeMovimiento.CuatroDirecciones:

                movX = Input.GetAxis(horizontalAxis);
                movY = Input.GetAxis(verticalAxis);

                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
                {
                    movX = 0;
                }

                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                {
                    movY = 0;
                }

                movement = new Vector3(movX, movY) * velocidad;
                break;
            case TipoDeMovimiento.OchoDirecciones:
                movX = Input.GetAxis(horizontalAxis);
                movY = Input.GetAxis(verticalAxis);
                movement = new Vector3(movX, movY) * velocidad;
                break;
        }

        Mover();

        if (puedeSaltar)
        {
            Saltar();
        }
    }

    void Mover()
    {
        if (!pointAndClick)
        {
            rb.velocity = movement;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            rb.velocity += movement;
        }
    }


    void Saltar()
    {
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpStop);
            }

            return;
        }


        Collider2D hit = Physics2D.OverlapBox(this.transform.position, jumpRange, 0, floorLayer);

        if (hit != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.velocity = new Vector2(rb.velocity.x, fuerza);
                isJump = true;
            }
        }
    }
}
