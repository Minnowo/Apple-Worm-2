using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;
using UnityEditor.Events;

public class LevelButtonCreator : MonoBehaviour
{
    public GameObject canvas;
    public GameObject songPickButtonPrefab;
    public GameObject songMessengerPrefab;

    public static int selectedSongIndex = 0;

    public bool centerButtonsOnCanvasX = false;
    public bool centerButtonsOnCanvasY = false;

    public int buttonsPerRow;

    public float buttonWidth;
    public float buttonHeight;
    public float leftMargin;
    public float topMargin;

    private RectTransform canvasRect
    {
        get
        {
            return this.GetComponent<RectTransform>();
        }
    }

    private PointF CanvasTopLeft;

    private void Awake()
    {
        if (SongMessenger.Instance == null)
        {
            Instantiate(songMessengerPrefab);
        }
    }


    void Start()
    {
        CanvasTopLeft = new PointF(0, canvasRect.rect.height);
        SongCollection[] songs = SongPickingScript.Instance.songCollections;

        if (centerButtonsOnCanvasX || centerButtonsOnCanvasY)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            if (centerButtonsOnCanvasX)
            {
                leftMargin = canvasRect.sizeDelta.x / 2 - (buttonWidth * buttonsPerRow / 2);
            }

            if (centerButtonsOnCanvasY)
            {
                topMargin = canvasRect.sizeDelta.y / 2 - 
                    (buttonHeight * Mathf.Ceil(songs.Length / buttonsPerRow) / 2);
            }
        }

        int count = 0;
        int row = 0;
        int column = 0;
        foreach(SongCollection sc in songs)
        {
            GameObject button = Instantiate(songPickButtonPrefab);
            RectTransform rect = button.GetComponent<RectTransform>();
            UnityEngine.UI.Text btnText = button.GetComponentInChildren<UnityEngine.UI.Text>();
            UnityEngine.UI.Button b = button.GetComponent<UnityEngine.UI.Button>();


            // change the button size
            rect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

            // want to place the button from top left corner
            // but 0, 0 is the bottom left corner, this is why
            // when we set the transform we subtract the button height an extra time
            rect.pivot = new Vector2(0, 0);

            button.transform.position = new Vector3(
                CanvasTopLeft.X + leftMargin + column * buttonWidth,

                // subtract the button height to adjust the fact that we are placing
                // from the bottom left of the button
                CanvasTopLeft.Y - topMargin - buttonHeight - row * buttonHeight);

            // set the parent of the button to the canvas
            button.transform.SetParent(canvas.transform);
            
            // we need a new variable so that when count is updated
            // it doesn't mess with the arguments
            int arg = count;
            b.onClick.AddListener(() => ButtonClick(arg));

            count++;
            btnText.text = count.ToString();

            column++;

            if (column == buttonsPerRow)
            {
                row++;
                column = 0;
            }
        }

        void ButtonClick(int index)
        {
            selectedSongIndex = index;
            SongMessenger.Instance.songIndex = index;
            SongMessenger.Instance.CurrentSong = SongPickingScript.Instance.songCollections[index].songSets[0].easy;
            LoadGameScene();
        }

        void LoadGameScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GamePlay");
        }
    }
}
