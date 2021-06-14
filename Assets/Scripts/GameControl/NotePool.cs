using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    public static NotePool Instance = null;

    public GameObject NotePrefab;

    public List<MusicNode> NotesObjectList;

    public const int generalDamage = 10;
    public const int spikeDamage = 20;

    public int InitialAmount = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        NotesObjectList = new List<MusicNode>();

        for(int i = 0; i < InitialAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(NotePrefab);
            obj.SetActive(false);
            NotesObjectList.Add(obj.GetComponent<MusicNode>());
        }
    }

    public MusicNode GetNode(float posX, float startY, float endX, float removeLineX, float posZ, float beat, NoteType nt = NoteType.Normal)
    {
        //check if there is an inactive instance
        // we will re-use it if there is 
        foreach (MusicNode node in NotesObjectList)
        {
            if (!node.gameObject.activeInHierarchy)
            {
                node.Initialize(posX, startY, endX, removeLineX, posZ, beat, nt);
                node.gameObject.SetActive(true);
                return node;
            }
        }

        //no inactive instances, instantiate a new GetComponent
        MusicNode musicNode = CreateNote(posX, startY, endX, removeLineX, posZ, beat, nt);
        NotesObjectList.Add(musicNode);
        return musicNode;
    }

    public MusicNode CreateNote(float posX, float startY, float endX, float removeLineX, float posZ, float beat, NoteType nt = NoteType.Normal)
    {
        // create a new gameobject using the note prefab
        MusicNode note = ((GameObject)Instantiate(NotePrefab)).GetComponent<MusicNode>();
        note.Initialize(posX, startY, endX, removeLineX, posZ, beat, nt);
        return note;
    }
}
