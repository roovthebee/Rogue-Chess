using UnityEngine;

public class PlayerRoleManager : MonoBehaviour
{
    // Private Fields

    private Team localTeam;

    // Public Properties

    public static PlayerRoleManager Instance { get; private set; }

    public Team LocalTeam => localTeam;

    public Team OpponentTeam => localTeam == Team.White ? Team.Black : Team.White;

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

    // Public Methods

    public void Initialize(Team team)
    {
        localTeam = team;
    }

    public bool CanControlTeam(Team team)
    {
        return localTeam == team;
    }
}