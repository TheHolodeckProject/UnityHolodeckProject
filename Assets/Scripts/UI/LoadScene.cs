using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour {

    public Toggle practiceCheckBox;

     public void LoadLevelButton(int index)
     {
         //Converts the checkbox bool to an int
         PlayerPrefs.SetInt("PracticeYesNo", practiceCheckBox.isOn ? 1 : 0);

         Application.LoadLevel(index);
     }
 
     public void LoadLevelButton(string levelName)
     {
         Application.LoadLevel(levelName);
     }
 }
