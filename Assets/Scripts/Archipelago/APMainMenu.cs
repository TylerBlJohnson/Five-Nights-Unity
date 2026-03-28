using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using JetBrains.Annotations;
using Archipelago.MultiClient.Net;
using System.Linq;
using UnityEditor.PackageManager;

public class APMainMenu : MonoBehaviour
{
    [SerializeField] private Animator chooseNightNumber;
    [SerializeField] private Camera APMenuCamera;
    [SerializeField] private bool[] nightAccess = new bool[7];
    private int currentNightNumber = 1;
    private const int LOWEST_NIGHT_NUMBER = 1;
    private const int HIGHEST_NIGHT_NUMBER = 7;
    private int total_nights;
    private int goal_night;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            AdjustSelectedNight(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            AdjustSelectedNight(-1);
        }

        if (Input.GetKeyDown(KeyCode.Return) && APMenuCamera.gameObject.activeSelf) {
            FindObjectOfType<MainMenu>().RequestStartNight(currentNightNumber);
        }

        chooseNightNumber.SetInteger("Progress", currentNightNumber);
    }

    void AdjustSelectedNight(int add)
    {
        if (!nightAccess.Any(night => night)) {
            Debug.Log("WARNING: No nights are active. This is probably a bug.");
            return;
        }
        do {
            currentNightNumber += add;
            if (currentNightNumber > HIGHEST_NIGHT_NUMBER) currentNightNumber = LOWEST_NIGHT_NUMBER;
            else if (currentNightNumber < LOWEST_NIGHT_NUMBER) currentNightNumber = HIGHEST_NIGHT_NUMBER;
        } while (!nightAccess[currentNightNumber-1]);
    }

    bool IsProgressiveNight()
    {
        //TODO: Add progressive nights toggle
        return true;
    }

    public void UpdateNightAccess(bool[] nightAccess)
    {
        this.nightAccess = nightAccess;
    }

    

    public void ForceUpdate(ArchipelagoSession session = null, Dictionary<string, object> slotData = null)
    {
        if (session != null && slotData != null)
        {
            if (slotData.ContainsKey("total_nights"))
            {
                Debug.Log("Total nights is " + slotData["total_nights"]);
                total_nights = (int)(long)slotData["total_nights"];
            }
            else
            {
                Debug.Log("No total_nights key??");
                foreach (var kvp in slotData)
                {
                    Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value} (Type: {kvp.Value.GetType()})");
                }
            }

            if (slotData.ContainsKey("goal_night"))
            {
                Debug.Log("Goal night is " + slotData["goal_night"]);
                goal_night = (int)(long)slotData["goal_night"];
            }
        }
    }
}
