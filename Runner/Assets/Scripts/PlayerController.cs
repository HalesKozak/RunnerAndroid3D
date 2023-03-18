using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 dir;
    private float maxSpeed = 100;
    private int lineToMove = 1;
    private Animator anim;
    private Score score; 
    public float lineDistance = 3;
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private float gravity;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private int coins;
    [SerializeField] private Text coinsText;
    [SerializeField] private Score scoreScript;

    private bool isImmortal;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        StartCoroutine(SpeedIncrease());
        Time.timeScale = 0;
        coins = PlayerPrefs.GetInt("coins");
        coinsText.text = coins.ToString();
        score = scoreText.GetComponent<Score>();
        score.scoreMultiplier = 1;
        isImmortal = false;
    }

    private void Update()
    {
        if (SwipeContoller.swipeRight)
        {
            if (lineToMove < 2)
                lineToMove++;
        }

        if (SwipeContoller.swipeLeft)
        {
            if (lineToMove > 0)
                lineToMove--;
        }
        if (SwipeContoller.swipeUp)
        {
            if (controller.isGrounded)
                Jump();
        }

        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
        if (lineToMove == 0)
            targetPosition += Vector3.left * lineDistance;
        else if (lineToMove == 2)
            targetPosition += Vector3.right * lineDistance;

        if (transform.position == targetPosition)
            return;
        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
            controller.Move(moveDir);
        else
            controller.Move(diff);
    }

    private void Jump()
    {
        dir.y = jump;
        anim.SetTrigger("Jump");
    }
    void FixedUpdate()
    {
        dir.z = speed;
        dir.y += gravity * Time.fixedDeltaTime;
        controller.Move(dir * Time.fixedDeltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "obstacle")
        {
            if(isImmortal)
                Destroy(hit.gameObject);
            else
            {
                losePanel.SetActive(true);
                int lastRunScore = int.Parse(scoreScript.scoreText.text.ToString());
                PlayerPrefs.SetInt("lastRunScore", lastRunScore);
                Time.timeScale = 0;
            }
            
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Coin")
        {
            coins++;
            PlayerPrefs.SetInt("coins",coins);
            coinsText.text = coins.ToString();
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "BonusStar")
        {
            StartCoroutine(StarBonus());
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Shield")
        {
            StartCoroutine(ShieldBonus());
            Destroy(other.gameObject);
        }
    }
    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(4);
        if(speed < maxSpeed)
        {
            speed += 1;
            StartCoroutine(SpeedIncrease());
        }
    }
    private IEnumerator StarBonus()
    {
        score.scoreMultiplier = 3;

        yield return new WaitForSeconds(5);

        score.scoreMultiplier = 1;
    }
    private IEnumerator ShieldBonus()
    {
        isImmortal = true;

        yield return new WaitForSeconds(5);

        isImmortal = false;
    }
}
