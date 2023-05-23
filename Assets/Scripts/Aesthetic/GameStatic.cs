using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatic : MonoBehaviour
{
    private enum GameState {
        Game,
        Menu
    }

    [SerializeField] private GameState gameState = GameState.Game;
    private Renderer rend;
    private float R;
    private float G;
    private float B;
    private int bigNum;
    private float timeSinceLastRand = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rend = gameObject.GetComponent<Renderer>();
        R = rend.material.color.r;
        G = rend.material.color.g;
        B = rend.material.color.b;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Menu) {
            timeSinceLastRand += Time.deltaTime;
            if (timeSinceLastRand >= 9.0f / 100.0f) {
                timeSinceLastRand -= 9.0f / 100.0f;
                rend.material.color = new Color(R, G, B, Random.Range(50, 150) / 255.0f);
            }
        }

        if (gameState == GameState.Game) {
            //Once per second, a large stepper of alpha transparency is applied to the static (15, 30, or 45)
            timeSinceLastRand += Time.deltaTime;
            if (timeSinceLastRand >= 1.0f) {
                bigNum = Random.Range(1, 4);
                timeSinceLastRand -= 1.0f;
            }

            //Every frame, a small range of alpha transparency is applied to the static
            int smallNum = Random.Range(0, 50);
            float staticAlpha = (50.0f + (float)smallNum + ((float)bigNum * 15.0f)) / 255.0f;
            rend.material.color = new Color(R, G, B, staticAlpha);
        }
    }
}
