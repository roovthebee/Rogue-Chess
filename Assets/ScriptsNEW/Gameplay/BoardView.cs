using UnityEngine;

public class BoardView : MonoBehaviour
{
    // Serialized Fields

    [SerializeField] private Camera boardCamera;

    // Unity Messages

    private void Start()
    {
        if (PlayerRoleManager.Instance.LocalTeam != Team.Black)
        {
            return;
        }

        boardCamera.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
    }

    // Private Helpers
}