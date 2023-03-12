using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    private Rigidbody2D rb;
    private readonly float jumpForce = 4;
    private readonly float rotationMultiplier = 10f;
    private readonly float rotationLerpSpeed = 20f;
    public Transform tail;
    public AnimationCurve tailCurve;
    private readonly float tailDuration = 0.2f;
    public AudioClip jumpSound;
    public AudioClip deathSound;
    public AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.instance.curState == GameController.State.Menu || 
            GameController.instance.curState == GameController.State.Countdown ||
            GameController.instance.curState == GameController.State.Loading)
        {
            if (transform.position.y <= 0f)
            {
                Jump();
            }
        }

        if (GameController.instance.curState == GameController.State.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        if (GameController.instance.curState == GameController.State.Menu ||
            GameController.instance.curState == GameController.State.Countdown ||
            GameController.instance.curState == GameController.State.Playing ||
            GameController.instance.curState == GameController.State.Loading)
        {
            RotateWithVelocity();
        }

        if (GameController.instance.curState == GameController.State.Playing)
        {
            if (rb.position.y > 3f)
            {
                rb.MovePosition(new Vector2(rb.position.x, 3f));
                rb.velocity = Vector2.zero;
            }
            if (rb.position.y <= -3f)
            {
                GameController.instance.ChangeState(GameController.State.Dying);
            }
        }
    }

    public void ResetCharacter()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.SetRotation(0f);
        rb.transform.position = (new Vector2(0, 0));
    }

    void RotateWithVelocity()
    {
        rb.SetRotation(Mathf.Lerp(rb.rotation, rb.velocity.y * rotationMultiplier, Time.deltaTime * rotationLerpSpeed));
    }


    void Jump()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(jumpSound);
        StartCoroutine(TailRoutine());
        rb.velocity = new Vector2(0, jumpForce);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameController.instance.curState == GameController.State.Playing)
        {
            GameController.instance.ChangeState(GameController.State.Dying);
            StartCoroutine(DieRoutine());
        }
    }

    IEnumerator TailRoutine()
    {
        float timer = 0f;
        float tailAngle = 45f;
        Vector3 tailRotation = tail.localEulerAngles;
        float startAngle = 0;

        static float LerpAngleUnclamped(float a, float b, float t)
        {
            float delta = Mathf.Repeat((b - a), 360);
            if (delta > 180)
                delta -= 360;
            return a + delta * t;
        }

        while (timer <= tailDuration)
        {
            tailRotation.z = LerpAngleUnclamped(startAngle, tailAngle, tailCurve.Evaluate(timer / tailDuration));
            tail.localEulerAngles = tailRotation;
            timer += Time.deltaTime;
            yield return null;
        }

        tailRotation.z = startAngle;
        tail.localEulerAngles = tailRotation;
    }

    IEnumerator DieRoutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(deathSound);
        Time.timeScale = 1f;
        rb.AddForce(new Vector3(-1, 1, 0) * 5f, ForceMode2D.Impulse);
        rb.AddTorque(2f, ForceMode2D.Impulse);
    }
}