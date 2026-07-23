using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip moveClip;

    [SerializeField] private AudioClip captureClip;

    [SerializeField] private AudioClip checkClip;

    // Private Fields

    // Public Properties

    public static AudioManager Instance { get; private set; }

    // Events

    // Unity Messages

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);

            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        BoardManager.PieceMoved += HandlePieceMoved;
    }

    private void OnDisable()
    {
        BoardManager.PieceMoved -= HandlePieceMoved;
    }

    // Public Methods

    public void PlayCheck()
    {
        PlayClip(checkClip);
    }

    // Private Workflow

    // Private Event Handlers

    private void HandlePieceMoved(MoveData move)
    {
        AudioClip clip = move.IsCapture ? captureClip : moveClip;

        PlayClip(clip);
    }

    // Private Helpers

    private void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip);
    }
}