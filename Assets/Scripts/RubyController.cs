using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;

    public GameObject projectilePrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip winSound;
    public AudioClip gameOverSound;
    public AudioSource musicSource;
    public AudioClip backgroundMusic;

    public static int level = 1;

    public int health { get { return currentHealth; } }
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);

    AudioSource audioSource;

    public ParticleSystem damage;

    public int scoreValue = 0;
    public int currentAmmo = 5;
    public Text scoreText;
    public Text GameOver;
    public Text ammoText;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();

        musicSource.clip = backgroundMusic;
        musicSource.Play();
        musicSource.loop = true;

        scoreText.text = "Fixed Robots: " + scoreValue.ToString();
        GameOver.text = " ";
        ammoText.text = "Ammo Left: " + currentAmmo.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        scoreText.text = "Fixed Robots: " + scoreValue.ToString();
        ammoText.text = "Ammo Left: " + currentAmmo.ToString();

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (scoreValue == 4)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
                if (hit.collider != null)
                {
                    if (scoreValue == 4)
                    {
                        level = 2;
                        SceneManager.LoadScene("Second");
                    }
                }
            }
        }

        if (level == 1)
        {
            if (currentHealth == 0)
            {
                speed = 0.0f;
                GameOver.text = "You Lose! Restart by pressing R. Game by Connor Wardell";
                musicSource.Stop();
                musicSource.clip = gameOverSound;
                musicSource.loop = false;
                musicSource.Play();

                if (Input.GetKey(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }

            if (scoreValue == 4)
            {
                GameOver.text = "Talk to Jambi to move to level 2!";
            }
        }

        if (level == 2)
        {

            if (currentHealth == 0)
            {
                speed = 0.0f;
                GameOver.text = "You Lose! Restart by pressing R. Game by Connor Wardell";
                musicSource.Stop();
                musicSource.clip = gameOverSound;
                musicSource.loop = false;
                musicSource.Play();

                if (Input.GetKey(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }

            if (scoreValue == 4)
            {
                GameOver.text = "You Win! Restart by pressing R. Game by Connor Wardell";
                musicSource.Stop();
                musicSource.clip = winSound;
                musicSource.loop = false;
                musicSource.Play();

                    if (Input.GetKey(KeyCode.R))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if(currentAmmo > 0)
            {
                Launch();
                currentAmmo = currentAmmo - 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);
            Instantiate(damage, GameObject.Find("Ruby").transform.position, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore(int scoreAmount)
    {
        if(scoreAmount > 0)
        {
            scoreValue += 1;
        }
    }

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void changeCogs(int ammo)
    {
        currentAmmo = currentAmmo + ammo;
        ammoText.text = "Ammo Left: " + ammo.ToString();
    }
}