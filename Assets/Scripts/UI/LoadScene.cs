using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour {

    public Toggle practiceCheckBox;
    public Toggle oculusCheckBox;
    public InputField subNumInputField;
    private int subNum;

    void Start() {
        subNum = 9999;
    }

     public void LoadLevelButton(int index)
     {
         //Checks that subject # is a non-negative integer
         //subNumInputField.characterValidation = InputField.CharacterValidation.Integer;
         //Debug.Log("InputFieldValidation = " + subNumInputField.characterValidation);
         if subNumInputField.text==""
             De
         Debug.Log("Sub num = " + int.Parse(subNumInputField.text));
         PlayerPrefs.SetInt("SubjectNumber", int.Parse(subNumInputField.text));
   

         //Converts the checkbox bool to an int
         PlayerPrefs.SetInt("PracticeYesNo", practiceCheckBox.isOn ? 1 : 0);
         PlayerPrefs.SetInt("OculusCamYesNo", oculusCheckBox.isOn ? 1 : 0);

         Application.LoadLevel(index);
     }
 
     public void LoadLevelButton(string levelName)
     {
         Application.LoadLevel(levelName);
     }
 }
