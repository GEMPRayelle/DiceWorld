using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player_State : MonoBehaviour
{
    public int maxHealth = 20;//Ã¼·Â
    public int currentHealth;
    public int money = 0;//µ·
    public int orbAttack_p = 5;//Ä®¿Àºê
    public int orbIntell_p = 5;//Ã¥¿Àºê
    public bool isDead = false;
    public HealthBar healthBar;
    public Text coinText;
    public Text BookText;
    public Text SwordText;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip audioClip2;
    public GameObject game_manager;
    public int playerId;
    public int score;

    void Start()
    {
        //playerId = this.gameObject.GetComponent<pawn_move>().player_ID;
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        money = 0;
        audioSource = GetComponent<AudioSource>();
        game_manager = GameObject.Find("game_manager");
    }

    // Update is called once per frame
    void Update()
    {
        score = currentHealth + money;
    }

    public void Damaged(int damage){
        if(this.currentHealth > 0  && isDead == false){
            audioSource.Play();
            this.currentHealth -= damage*2;
            healthBar.setHealth(currentHealth);
        }
        if(this.currentHealth <= 0 && isDead == false)
        { 
            Die();
        }
    }
    public void healing(int heal)
    {
        if (this.currentHealth > 0 && isDead == false)
        {
            this.currentHealth += heal;
            if (this.currentHealth >= 20)
            {
                this.currentHealth = maxHealth;
            }
            healthBar.setHealth(currentHealth);
        }
        
    }
    public void increaseMoney(int money)
    {
        audioSource.clip = audioClip2;
        audioSource.Play();
        this.money += money;
        coinText.text = $"{this.money}";
    }
    public void decreaseMoney(int money)
    {
        this.money -= money;
        if(this.money < 0)
        {
            this.money = 0;
            Damaged(2);
        }
        coinText.text = $"{this.money}";
    }
    public void increaseSword()
    {
        orbAttack_p += 5;
        if(orbAttack_p >= 5)
        {
            orbAttack_p = 5;
        }
        SwordText.text = $"{this.orbAttack_p}" + " / 5";
    }
    public void decreaseSword(int sword)
    {
        if (orbAttack_p < sword)
        {
            sword -= orbAttack_p;
            orbAttack_p = 0;
            Damaged(sword);
            SwordText.text = $"{this.orbAttack_p}" + " / 5";
        }
        else
        {
            orbAttack_p -= sword;
            SwordText.text = $"{this.orbAttack_p}" + " / 5";
            increaseMoney(sword*3);
        }
        
    }
    public void increaseBook()
    {
        orbIntell_p += 5;
        if (orbIntell_p >= 5)
        {
            orbIntell_p = 5;
        }
        BookText.text = $"{orbIntell_p}" + " / 5";
    }
    public void decreaseBook(int book)
    {
        if (orbIntell_p < book)
        {
            BookText.text = $"{this.orbIntell_p}" + " / 5";
        }
        else
        {
            orbIntell_p -= book;
            BookText.text = $"{orbIntell_p}" + " / 5";
            increaseMoney(book*5);
        }
    }

    public void Die(){
        //ÇÃ·¹ÀÌ¾î Á×¾î¼­ °ÔÀÓÁ¾·á
        //¿ÀºêÁ§Æ® ¼û±è
        isDead = true;
        playerId = GetComponent<pawn_move>().player_ID;
        game_manager.GetComponent<game_manager>().pushRank(playerId);
        Destroy(gameObject);
    }
}
