using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Subject", menuName = "Create new Subject", order = 1)]
public class subject : ScriptableObject
{
    public string subjectname;
    public Color borderColor;
    public Color innerColor;
    public Sprite face;
   // public Color nameplate;
    //public Color faceplate;
}
