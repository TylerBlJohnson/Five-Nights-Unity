using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevel : MonoBehaviour
{
    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private Animator tensDigit;
    [SerializeField] private Animator onesDigit;
    [SerializeField] private int minLevel = 0;
    [SerializeField] private int maxLevel = 20;
    [SerializeField] private int defaultLevel = 3;
    private int level;

    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        if (minLevel > maxLevel) {
            Debug.LogError("MinLevel cannot be greater than maxLevel");
        }
        if (defaultLevel < minLevel) {
            level = minLevel;
        } else if (defaultLevel > maxLevel) {
            level = maxLevel;
        } else {
            level = defaultLevel;
        }
        UpdateLevel();
    }

    // Update is called once per frame
    void Update()
    {
        //Where the mouse is pointing
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Upon clicking something
        if (Physics.Raycast(ray, out hit) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            //click the "leftButton" GameObject
            if (hit.collider.name == leftButton.name) {
                level--;
            }

            //click the "rightButton" GameObject
            if (hit.collider.name == rightButton.name) {
                level++;
            }

            if (level < minLevel) {
                level = minLevel;
            } else if (level > maxLevel) {
                level = maxLevel;
            }
        }
        UpdateLevel();
    }

    private int GetTensDigit(int level) {
        level = level % 100;
        return level / 10;
    }

    private int GetOnesDigit(int level) {
        return level % 10;
    }

    private void UpdateDigit(Animator anim, int num, bool invisibleOnZero = false)
    {
        if (num < 0 || num > 9) {
            Debug.LogError("Invalid number");
        } else {
            if (num == 0 && invisibleOnZero) {
                anim.SetBool("Visible", false);
            } else {
                anim.SetBool("Visible", true);
            }
            anim.SetInteger("Digit", num);
        }
    }

    private void UpdateLevel()
    {
        UpdateDigit(tensDigit, GetTensDigit(level), true);
        UpdateDigit(onesDigit, GetOnesDigit(level), false);
    }

    public int GetLevel() {
        return level;
    }
}
