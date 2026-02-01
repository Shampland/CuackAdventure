using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;
    public float fuerzaSalto = 7f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("Better Jump")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Interacción")]
    public Transform puntoRaycast;
    public float distanciaInteraccion = 1.2f;
    public LayerMask capaObjetos;

    [Header("Ocultamiento")]
    public bool estaOculto;
    private bool bloqueadoPorAnimacion;

    [Header("Muerte")]
    public bool estaMuerto;

    [Header("Muerte - Movimiento")]
    public float offsetYDead = -0.366f;   // De -3.260 a -3.626
    public float tiempoMovimientoDead = 0.3f;

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsule;

    private float movimientoX;
    private bool enSuelo;
    private bool saltoBloqueado;

    private int layerPlayer;
    private int layerEnemy;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider2D>();

        layerPlayer = LayerMask.NameToLayer("Player");
        layerEnemy = LayerMask.NameToLayer("Enemy");
    }

    void Update()
    {
        // ================= MUERTE =================
        if (estaMuerto)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // ================= OCULTARSE =================
        if (Input.GetKeyDown(KeyCode.Q) && !bloqueadoPorAnimacion)
        {
            AlternarOcultamiento();
        }

        if (estaOculto || bloqueadoPorAnimacion)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        // ================= MOVIMIENTO =================
        movimientoX = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("Speed", Mathf.Abs(movimientoX));

        if (movimientoX > 0) Girar(true);
        else if (movimientoX < 0) Girar(false);

        // ================= SUELO =================
        enSuelo = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        animator.SetBool("Grounded", enSuelo);

        if (enSuelo)
        {
            coyoteTimeCounter = coyoteTime;
            saltoBloqueado = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // ================= JUMP BUFFER =================
        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !saltoBloqueado)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
            saltoBloqueado = true;
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
        }

        // ================= BETTER JUMP =================
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up *
                Physics2D.gravity.y *
                (fallMultiplier - 1) *
                Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector2.up *
                Physics2D.gravity.y *
                (lowJumpMultiplier - 1) *
                Time.deltaTime;
        }

        // ================= INTERACTUAR =================
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interactuar();
        }
    }

    void FixedUpdate()
    {
        if (estaOculto || bloqueadoPorAnimacion || estaMuerto) return;

        rb.linearVelocity = new Vector2(
            movimientoX * velocidad,
            rb.linearVelocity.y
        );
    }

    // ================= MUERTE =================

    public void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;
        bloqueadoPorAnimacion = true;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;

        if (capsule != null)
            capsule.enabled = false;

        animator.SetBool("Dead", true);

        StartCoroutine(MoverYAlMorir());

        GameManager.Instance?.GameOver();
    }

    private IEnumerator MoverYAlMorir()
    {
        Vector3 inicio = transform.position;
        Vector3 destino = new Vector3(
            inicio.x,
            inicio.y + offsetYDead,
            inicio.z
        );

        float tiempo = 0f;

        while (tiempo < tiempoMovimientoDead)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / tiempoMovimientoDead;

            transform.position = new Vector3(
                inicio.x,
                Mathf.Lerp(inicio.y, destino.y, t),
                inicio.z
            );

            yield return null;
        }

        transform.position = destino;
    }

    // ================= OCULTAMIENTO =================

    void AlternarOcultamiento()
    {
        bloqueadoPorAnimacion = true;
        estaOculto = !estaOculto;

        rb.linearVelocity = Vector2.zero;
        animator.SetBool("Oculto", estaOculto);

        Physics2D.IgnoreLayerCollision(layerPlayer, layerEnemy, estaOculto);
    }

    // EVENTOS DE ANIMACIÓN
    public void OnOcultamientoCompleto()
    {
        bloqueadoPorAnimacion = false;
    }

    public void OnSalirOcultamiento()
    {
        bloqueadoPorAnimacion = false;
    }

    // ================= UTILIDADES =================

    void Girar(bool mirarDerecha)
    {
        Vector3 escala = transform.localScale;
        escala.x = mirarDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    void Interactuar()
    {
        Vector2 direccion = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        RaycastHit2D hit = Physics2D.Raycast(
            puntoRaycast.position,
            direccion,
            distanciaInteraccion,
            capaObjetos
        );

        if (hit.collider != null && hit.collider.CompareTag("Obj"))
        {
            hit.collider.GetComponent<ObjetoInteractuable>()?.Interactuar();
        }
    }
}
