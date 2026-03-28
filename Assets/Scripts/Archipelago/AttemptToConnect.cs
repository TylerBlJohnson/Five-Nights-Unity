using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AttemptToConnect : MonoBehaviour
{
    [SerializeField] private TMP_Text slot;
    [SerializeField] private TMP_Text host;
    [SerializeField] private TMP_Text password;

    private void Start()
    {
    }

    private void OnMouseDown()
    {
        string slotString = slot.text.Trim((char)8203);
        string hostString = host.text.Trim((char)8203);
        string passwordString = password.text.Trim((char)8203);
        ArchipelagoManager.Instance.AttemptToConnect(slotString, hostString, passwordString);
    }
}
