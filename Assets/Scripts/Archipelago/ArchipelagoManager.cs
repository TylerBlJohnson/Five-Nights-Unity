using UnityEngine;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System.Collections.Generic;
using System.Linq;
using System;
using Archipelago.MultiClient.Net.Packets;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using IEnumerator = System.Collections.IEnumerator;
using System.Net.WebSockets;

public class ArchipelagoManager : MonoBehaviour
{
    public enum CheckType
    {
        Night_Victory,
        Hour_Reached
    }

    private int[] checkID = { 
            90000, 
            90100
        };

    [Header("Connection Settings")]
    [SerializeField] private string gameName = "Five Nights at Freddy's Unity";
    [SerializeField] TMP_Text resultTMP;

    [Header("Items to Watch For")]
    // This lets you define "If I get Item ID X, turn on GameObject Y"
    public List<ItemMapping> itemMappings;

    private ArchipelagoSession session;

    private bool[] nightAccess = new bool[7];

    [System.Serializable]
    public struct ItemMapping {
        public string itemName;
        public GameObject objectToEnable;
    }

    private HashSet<long> receivedItemIds = new HashSet<long>();

    public static ArchipelagoManager Instance;

    public Dictionary<string, object> slotData;

    private const int LOWEST_NIGHT_NUMBER = 1;
    private const int HIGHEST_NIGHT_NUMBER = 7;

    [SerializeField] private int goal_night;
    private int total_nights;

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu") StartCoroutine("EndOfFrameSceneLoaded");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    void Start() {
        PrintResultText("");
    }

    void Update() {
        if (session == null || !session.Socket.Connected) return;

        while (session.Items.Any()) {
            var item = session.Items.DequeueItem();
            Debug.Log($"Received: {item.ItemName}");
            
            // 1. Add to the "Inventory"
            receivedItemIds.Add(item.ItemId);

            // 2. Check the list to see if anything should be unlocked
            foreach (var mapping in itemMappings) {
                if (mapping.itemName == item.ItemName) {
                    mapping.objectToEnable.SetActive(true);
                }
            }

            UpdateItems();
        }
    }

    public void AttemptToConnect(string playerName, string serverURL, string playerPassword)
    {
        try {
            if (serverURL.Contains(":")) {
                string hostName = serverURL.Substring(0, serverURL.IndexOf(':'));
                int port = int.Parse(serverURL.Substring(serverURL.IndexOf(':') + 1));

                session = ArchipelagoSessionFactory.CreateSession(hostName, port);

                var result = session.TryConnectAndLogin(gameName, playerName, ItemsHandlingFlags.AllItems, password: playerPassword);
                
                if (result.Successful) {
                    PrintResultText("Connected to Archipelago!");
                    DontDestroyOnLoad(this.gameObject);
                    slotData = session.DataStorage.GetSlotData();
                    if (slotData.ContainsKey("total_nights"))
                    {
                        total_nights = (int)(long)slotData["total_nights"];
                    }

                    if (slotData.ContainsKey("goal_night"))
                    {
                        goal_night = (int)(long)slotData["goal_night"];
                    }
                    
                    FindObjectOfType<APMainMenu>().ForceUpdate(session, slotData);
                    TempData.isArchipelagoGame = true;
                    /*session.Items.ItemReceived += (helper) =>
                    {
                        OnItemReceived(helper);
                    };
                    */
                    UpdateItems();
                }
                else PrintResultText("Connection Failed!");
            }
            else
            {
                PrintResultText("Host must contain ':'!");
            }
        }
        catch (Exception e)
        {
            PrintResultText(e.ToString());
        }
    }

    private void PrintResultText(string resultText)
    {
        resultTMP.text = resultText;
        Debug.Log(resultText);
    }

    public void DetermineCheck(CheckType check, int hour = 0) 
    {
        if (check == CheckType.Night_Victory)
        {
            SendCheck(checkID[(int)CheckType.Night_Victory] + TempData.loadNight);
        }
        else if (check == CheckType.Hour_Reached && hour > 0)
        {
            SendCheck(checkID[(int)CheckType.Hour_Reached] + (TempData.loadNight * 5) + hour);
        }
    }

    // Call this from Button OnClick, pass the ID in the Inspector
    public void SendCheck(int locationId) 
    {
        if (!CheckForVictoryCondition()) {
            session.Locations.CompleteLocationChecks(locationId);
            Debug.Log($"Sent Location ID: {locationId}");
        }
    }

    bool CheckForVictoryCondition()
    {
        if (goal_night == TempData.loadNight && IsProgressiveNight())
        {
            DeclareVictory();
            return true;
        }
        else return false;
    }

    public void DeclareVictory() 
    {
        var statusUpdatePacket = new StatusUpdatePacket();
        statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
        session.Socket.SendPacket(statusUpdatePacket);

        Debug.Log("GOAL REACHED! Sent victory to server.");
        //victoryButton.SetActive(false);
    }

    public bool IsConnected()
    {
        return session != null && session.Socket.Connected && session.ConnectionInfo.Slot != -1;
    }

    public ArchipelagoSession GetArchipelagoSession()
    {
        return session;
    }

    private void UpdateItems()
    {
        if (IsProgressiveNight())
        {
            Debug.Log("Updating Nights");
            int nightsUnlocked = session.Items.AllItemsReceived.Count(item => session.Items.GetItemName(item.ItemId) == "Progressive Night");

            for (int i = LOWEST_NIGHT_NUMBER - 1; i < HIGHEST_NIGHT_NUMBER; i++)
            {
                nightAccess[i] = i <= LOWEST_NIGHT_NUMBER + nightsUnlocked - 1;
            }
            string status = string.Join(", ", nightAccess);
            Debug.Log("Night Status: [" + status + "]");
            UpdateAPMainMenu();
        }
        else
        {
            //TODO: Add progressive night toggle
        }
    }

    private bool IsProgressiveNight()
    {
        //TODO: Add progressive nights toggle
        return true;
    }

    private void UpdateAPMainMenu()
    {
        APMainMenu apMenu = FindObjectOfType<APMainMenu>();
        if (apMenu != null)
        {
            apMenu.ForceUpdate(session, slotData);
            apMenu.UpdateNightAccess(nightAccess);
        }
    }

    private IEnumerator EndOfFrameSceneLoaded()
    {
        yield return new WaitForEndOfFrame();
        GameObject resultTextObject = GameObject.FindGameObjectWithTag("ResultText");
        if (resultTextObject != null) resultTMP = resultTextObject.GetComponent<TMP_Text>();
        UpdateAPMainMenu();
    }
}