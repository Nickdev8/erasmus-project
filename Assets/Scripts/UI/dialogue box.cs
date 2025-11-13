using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class dialoguebox : MonoBehaviour
{

    public DialogueSegment[] dialogueSegments;
    [Space]
    public Image Speakerfacedisplay;
    public Image Boxinner;
    public Image Boxouter;
    public Image faceplate;
    public Image nameplate;
    public Image Skipthing;
    [Space]
    public TextMeshProUGUI Speakername;
    public TextMeshProUGUI Dialoguebox;
    [Space]
    public float TextSpeed;

    private bool canskip;
    private int dialogueindex;


    // Start is called before the first frame update
    void Start()
    {
        Setstyle(dialogueSegments[0].speaker);
        StartCoroutine(PlayDialogue(dialogueSegments[0].dialogue));
    }

    public void ClickStartButton()
    {
        SceneManager.LoadScene(11);
    }

    // Update is called once per frame
    void Update()
    {
        Skipthing.enabled = canskip;
        if (Input.GetKeyDown(KeyCode.Space) && canskip)
        {
            Setstyle(dialogueSegments[dialogueindex].speaker);
            StartCoroutine(PlayDialogue(dialogueSegments[dialogueindex].dialogue));
        }
    }


    void Setstyle(subject Speaker)
    {
        if (Speaker.face == null)
        {
            Speakerfacedisplay.color = new Color(0, 0, 0, 0);
        }
        else
        {
            Speakerfacedisplay.sprite = Speaker.face;
            Speakerfacedisplay.color = Color.white;
        }

        Boxinner.color = Speaker.innerColor;
        Boxouter.color = Speaker.borderColor;
        Speakername.SetText(Speaker.subjectname);
        //Speakername.color = Speaker.nameplate;
        //Speakerfacedisplay.color = Speaker.faceplate;
    }

    IEnumerator PlayDialogue(string Dialogue)
    {
        canskip = false;
        Dialoguebox.SetText(string.Empty);

        for (int i = 0; i < Dialogue.Length; i++)
        {
            Dialoguebox.text += Dialogue[i];
            yield return new WaitForSeconds(1f / TextSpeed);
        }

        canskip = true;
    }
}

[System.Serializable]
public class DialogueSegment
{
    public string dialogue;
    public subject speaker;

}
