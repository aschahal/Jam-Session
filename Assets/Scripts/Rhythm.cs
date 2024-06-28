using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;
using UnityEngine.SceneManagement;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Creates the notes that in the jam session
/// Needs to be placed on the Rhythm Vizualizer in order to work correctly
/// </summary>
public class Rhythm : MonoBehaviour
{
    public GameObject shortNotePrefab;
    public GameObject longNotePrefab;
    public GameObject duoNotePrefab;

    public AudioSource audio;
    public List<GameObject> song;
    public float bpm;
    public float delay;
    public float timer = 0.06f;
    public float firstNoteTime = 0;
    public int noteCount = 0;
    public GameObject lastNote;

    public void ImportSong(string path)
    {
        StreamReader sr = new StreamReader(path);
        string text;

        do
        {
            text = sr.ReadLine();
            if (float.TryParse(text, out float t))
            {
                bpm = t;
                delay = bpm / 160;
                //Debug.Log(delay);
            }
            else if (text != null)
            {
                GameObject note;
                if (float.Parse(text.Split(',')[1]) <= 1.2f)
                {
                    note = shortNotePrefab;
                }
                else
                {
                    note = longNotePrefab;
                }

                note.GetComponent<Note>().GenerateNote(text, bpm);
                song.Add(GameObject.Instantiate(note));
                note.GetComponent<Note>().prefab = note;
                song[song.Count - 1].SetActive(false);
                //Debug.Log(note.GetComponent<Note>().ToString() + " " + song.Count);
            }
            //Debug.Log(text);
        } while (text != null);
        sr.Close();

        firstNoteTime = song[0].GetComponent<Note>().time;
        //Debug.Log(song[0].GetComponent<Note>().ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        song = new List<GameObject>();
        ImportSong("Assets/Text/Jam 1.txt");
        //delay = 2;
        Debug.Log(song.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (firstNoteTime >= delay && !audio.isPlaying)
        {
            audio.Play();
            foreach (GameObject note in song)
            {
                note.GetComponent<Note>().time -= delay * (bpm / 100);
            }
        }

        if (noteCount < song.Count && timer >= song[noteCount].GetComponent<Note>().time)
        {
            if (timer >= delay + firstNoteTime && !audio.isPlaying)
            {
                audio.Play();
            }
            song[noteCount].SetActive(true);
            //Debug.Log(song[noteCount].GetComponent<Note>().note + " " + song[noteCount].GetComponent<Note>().time);
            if (song[noteCount].GetComponent<Note>().length >= 1.2f)
            {
                //Debug.Log("Long");
                //song[noteCount].GetComponent<Transform>().localScale = new
                //    Vector3(song[noteCount].GetComponent<Note>().length * song[noteCount].GetComponent<Note>().speed * 1000, 2, 1);
                song[noteCount].GetComponent<Transform>().localScale = new Vector3(song[noteCount].GetComponent<Transform>().localScale.x +
                    (song[noteCount].GetComponent<Note>().length * song[noteCount].GetComponent<Note>().speed), 2, 1);
                lastNote = song[noteCount];
                //lastNote.GetComponent<Note>().speed = 0;
            }
            noteCount++;
        }

        if (lastNote != null && lastNote.GetComponent<Note>().length >= 1.2f && song[noteCount].GetComponent<Transform>().localScale != new
            Vector3(lastNote.GetComponent<Note>().length / lastNote.GetComponent<Note>().speed, 2, 1))
        {
            Debug.Log(lastNote.GetComponent<Note>().length * lastNote.GetComponent<Note>().speed);
            lastNote.GetComponent<Transform>().localScale = new Vector3(lastNote.GetComponent<Transform>().localScale.x +
                    (lastNote.GetComponent<Note>().length * lastNote.GetComponent<Note>().speed), 2, 1);

        }

        if (audio.time < timer && noteCount >= song.Count)
        {
            SceneManager.LoadScene("WalkTestScene");
        }

        timer += Time.deltaTime;
    }

    //    void PlaySong(string song)
    //    {

    //    }
}
