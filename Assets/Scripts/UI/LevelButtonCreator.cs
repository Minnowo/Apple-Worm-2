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

    private PointF CanvasTopLeft;

    private RectTransform canvasRect
    {
        get
        {
            return this.GetComponent<RectTransform>();
        }
    }

    private UnityEngine.UI.Button[] levelButtons;
    private UnityEngine.UI.Button[] difficultyButtons;

    private SongCollection.SongSet songSet;

    private void Awake()
    {
        if (SongMessenger.Instance == null)
        {
            Instantiate(songMessengerPrefab);
        }
    }


    void Start()
    {
        RectTransform canvas = canvasRect;
        CanvasTopLeft = new PointF(0, canvas.rect.height);
        SongCollection[] songs = SongPickingScript.Instance.songCollections;

        if (centerButtonsOnCanvasX || centerButtonsOnCanvasY)
        {
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
        levelButtons = new UnityEngine.UI.Button[songs.Length];
        foreach (SongCollection sc in songs)
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

            levelButtons[count] = b;

            count++;
            btnText.text = sc.name;//count.ToString();

            column++;

            if (column == buttonsPerRow)
            {
                row++;
                column = 0;
            }
        }

        count = 0;
        float xpos = canvas.sizeDelta.x / 4;
        float ypos = canvas.sizeDelta.y / 2;
        difficultyButtons = new UnityEngine.UI.Button[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject buttonGameObject = Instantiate(songPickButtonPrefab);

            buttonGameObject.transform.position = new Vector3(xpos * (i+1), ypos, buttonGameObject.transform.position.z);
            buttonGameObject.transform.SetParent(canvas.transform);

            UnityEngine.UI.Text text = buttonGameObject.GetComponentInChildren<UnityEngine.UI.Text>();
            //text.font = LevelSelectorPrefabs.BubbleFont;
            text.fontStyle = FontStyle.Bold;
            text.fontSize = 16; 

            switch (i)
            {
                case 0:
                    text.text = "Easy";
                    break;
                case 1:
                    text.text = "Normal";
                    break;
                case 2:
                    text.text = "Hard";
                    break;
            }

            UnityEngine.UI.Button button = buttonGameObject.GetComponent<UnityEngine.UI.Button>();
            difficultyButtons[i] = button;

            int arg = i;
            button.onClick.AddListener(() => ButtonClick(arg, true));
        }

        DifficultyButtonsVisible(false);
    }


    void ButtonClick(int index, bool isDifficultyButton = false)
    {
        if (isDifficultyButton)
        {
            if (songSet == null || songSet.IsEmpty())
                return;

            switch (index.Clamp(0, 2))
            {
                case 0:
                    SongMessenger.Instance.CurrentSong = songSet.easy;
                    break;
                case 1:
                    SongMessenger.Instance.CurrentSong = songSet.normal;
                    break;
                case 2:
                    SongMessenger.Instance.CurrentSong = songSet.hard;
                    break;
            }

            Helpers.LoadSceneByName("GamePlay");
            return;
        }

        selectedSongIndex = index;
        SongMessenger.Instance.songIndex = index;
        //SongMessenger.Instance.CurrentSong = SongPickingScript.Instance.songCollections[index].songSets[0].easy;
        songSet = SongPickingScript.Instance.songCollections[index].songSets[0];

        LevelButtonVisible(false);
        DifficultyButtonsVisible(songSet, true);
    }

    void DifficultyButtonsVisible(SongCollection.SongSet set, bool visible)
    {
        if (set.easy != null && set.easy.tracks.Length == 2 && set.easy.Song != null)
            difficultyButtons[0].gameObject.SetActive(visible);

        if (set.normal != null && set.normal.tracks.Length == 2 && set.normal.Song != null)
            difficultyButtons[1].gameObject.SetActive(visible);

        if (set.hard != null && set.hard.tracks.Length == 2 && set.hard.Song != null)
            difficultyButtons[2].gameObject.SetActive(visible);
    }

    void DifficultyButtonsVisible(bool makeVisible)
    {
        if (difficultyButtons == null || difficultyButtons.Length < 1)
            return;

        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            difficultyButtons[i].gameObject.SetActive(makeVisible);
        }
    }


    void LevelButtonVisible(bool makeVisible)
    {
        if (levelButtons == null || levelButtons.Length < 1)
            return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].gameObject.SetActive(makeVisible);
        }
    }

}
