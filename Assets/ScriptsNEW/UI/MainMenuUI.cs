using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private MultiplayerManager multiplayerManager;

    [SerializeField] private TMP_Text statusText;

    [SerializeField] private Button hostButton;

    [SerializeField] private TMP_InputField joinCodeInput;

    [SerializeField] private Button joinButton;

    [SerializeField] private Button startGameButton;

    [SerializeField] private Button leaveButton;

    // Private Fields

    private bool isBusy;

    // Unity Messages

    private void Start()
    {
        RefreshButtonState(SessionStatus.None);
    }

    private void OnEnable()
    {
        multiplayerManager.SessionPlayersChanged += OnSessionPlayersChanged;
    }

    private void OnDisable()
    {
        multiplayerManager.SessionPlayersChanged -= OnSessionPlayersChanged;
    }

    // Public Methods

    public async void HostGame()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        RefreshButtonState(SessionStatus.Connecting);

        statusText.text = "Creating Session...";

        try
        {
            string joinCode = await multiplayerManager.HostGameAsync();

            statusText.text = $"Join Code: {joinCode}";
        }
        catch (MultiplayerException exception)
        {
            statusText.text = exception.Message;
        }
        catch (Exception exception)
        {
            statusText.text = "Failed to create session.";

            Debug.LogException(exception);
        }
        finally
        {
            isBusy = false;
            
            RefreshButtonState(SessionStatus.Connected);
        }
    }

    public async void JoinGame()
    {
        if (isBusy)
        {
            return;
        }

        isBusy = true;

        RefreshButtonState(SessionStatus.Connecting);

        statusText.text = "Joining session...";

        try
        {
            await multiplayerManager.JoinGameAsync(joinCodeInput.text);

            statusText.text = "Joined session!";
        }
        catch (MultiplayerException exception)
        {
            statusText.text = exception.Message;
        }
        catch (Exception exception)
        {
            statusText.text = "Failed to join session.";

            Debug.LogException(exception);
        }
        finally
        {
            isBusy = false;

            RefreshButtonState(SessionStatus.Connected);
        }
    }

    public void StartGame()
    {
        try
        {
            multiplayerManager.StartGame();
        }
        catch (MultiplayerException exception)
        {
            statusText.text = exception.Message;
        }
    }

    public async void LeaveGame()
    {
        try
        {
            RefreshButtonState(SessionStatus.Connecting);

            await multiplayerManager.LeaveGameAsync();

            RefreshButtonState(SessionStatus.None);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
    }

    // Private Event Handlers

    private void OnSessionPlayersChanged()
    {
        RefreshButtonState(SessionStatus.Connected);

        if (!NetworkManager.Singleton.IsHost && multiplayerManager.CurrentSession.PlayerCount < 2)
        {
            LeaveGame();
        }
    }

    // Private Helpers

    private void RefreshButtonState(SessionStatus status)
    {
        if (status == SessionStatus.None)
        {
            hostButton.interactable = true;
            joinButton.interactable = true;

            startGameButton.interactable = false;
            leaveButton.interactable = false;
        }
        else if (status == SessionStatus.Connecting)
        {
            hostButton.interactable = false;
            joinButton.interactable = false;

            startGameButton.interactable = false;
            leaveButton.interactable = false;
        }
        else if (status == SessionStatus.Connected)
        {
            hostButton.interactable = false;
            joinButton.interactable = false;

            leaveButton.interactable = true;

            startGameButton.interactable = NetworkManager.Singleton.IsHost && multiplayerManager.CurrentSession.PlayerCount == 2;
        }
    }
}