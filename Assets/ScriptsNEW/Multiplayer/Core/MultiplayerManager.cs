using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : MonoBehaviour
{
    // Constants

    // Serialized Fields

    // Private Fields

    private static MultiplayerManager instance;

    private bool isInitialized;

    private SessionService sessionService;

    private ISession currentSession;

    // Public Properties

    public bool IsInitialized => isInitialized;

    public string PlayerId => AuthenticationService.Instance.PlayerId;

    public ISession CurrentSession => currentSession;

    // Events

    public event Action SessionPlayersChanged;

    // Unity Messages

    private async void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        await InitializeServicesAsync();
    }

    // Public Methods

    public async Task<string> HostGameAsync()
    {
        if (currentSession != null)
        {
            throw new InvalidOperationException("You are already hosting a session.");
        }

        currentSession = await sessionService.CreateSessionAsync();

        currentSession.Changed += OnSessionChanged;

        PlayerRoleManager.Instance.Initialize(Team.White);

        return currentSession.Code;
    }

    public async Task JoinGameAsync(string joinCode)
    {
        if (currentSession != null)
        {
            throw new MultiplayerException("You are already connected to a session.");
        }

        if (string.IsNullOrWhiteSpace(joinCode))
        {
            throw new MultiplayerException("Please enter a join code.");
        }

        currentSession = await sessionService.JoinSessionAsync(joinCode);

        currentSession.Changed += OnSessionChanged;

        PlayerRoleManager.Instance.Initialize(Team.Black);
    }

    public void StartGame()
    {
        if (currentSession == null)
        {
            throw new MultiplayerException("You are not connected to a session.");
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            throw new MultiplayerException("Only the host can start the game.");
        }

        NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.Gameplay, LoadSceneMode.Single);
    }

    public async Task LeaveGameAsync()
    {
        if (currentSession == null)
        {
            throw new MultiplayerException("You are not connected to a session.");
        }

        currentSession.Changed -= OnSessionChanged;

        try
        {
            await currentSession.LeaveAsync();    
        }
        finally
        {
            currentSession = null;
        }
    }

    // Private Workflow

    private async Task InitializeServicesAsync()
    {
        if (isInitialized)
        {
            return;
        }

        try
        {
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            sessionService = new SessionService();

            isInitialized = true;
        }  
        catch (System.Exception exception)
        {
            Debug.LogException(exception);
        } 
    }

    // Private Event Handlers

    private void OnSessionChanged()
    {
        SessionPlayersChanged?.Invoke();
    }

    // Private Validation

    // Private Helpers
}