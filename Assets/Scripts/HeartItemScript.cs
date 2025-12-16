using NatureBackgroundsPixelArt;
using UnityEngine;

public class HeartItemScript : MonoBehaviour
{
    [SerializeField] private HeartUI heartUI;

    [Header("Audio")]
    public AudioClip heartAudio;
    private AudioSource audioSource;
    private void Awake()
    {
        if (heartUI == null)
            heartUI = Object.FindFirstObjectByType<HeartUI>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();
            audioSource.PlayOneShot(heartAudio, 0.9f);
            if (player.health < 3 && player.health > 0) player.health+=1;
            Destroy(gameObject, 0.1f);
            heartUI.UpdateHearts(player.health);
        }
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update()
    {
        
    }

}
