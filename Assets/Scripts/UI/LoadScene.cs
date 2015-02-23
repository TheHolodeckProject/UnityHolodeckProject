using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{

    public Toggle practiceCheckBox;
    public Toggle oculusCheckBox;
    public Toggle logDataCheckBox;
    public InputField subNumInputField;
    public InputField sessNumInputField;
    public InputField startDiffInputField;
    public InputField sessLengthInputField;
    public Toggle trialLimit;

    public void LoadLevelButton(int index)
    {
        //Converts the checkbox bool to an int
        PlayerPrefs.SetInt("PracticeYesNo", practiceCheckBox.isOn ? 1 : 0);
        PlayerPrefs.SetInt("OculusCamYesNo", oculusCheckBox.isOn ? 1 : 0);
        // ??? How to get this to easily enable / disable the logger?
        PlayerPrefs.SetInt("LogDataYesNo", logDataCheckBox.isOn ? 1 : 0);

        //If no Subject # was entered, uses 0000 as the default
        if (subNumInputField.text == "")
            PlayerPrefs.SetInt("SubjectNumber", 0000);
        else
            PlayerPrefs.SetInt("SubjectNumber", int.Parse(subNumInputField.text));

        //If no Session # was entered, uses 1 as the default
        if (sessNumInputField.text == "")
            PlayerPrefs.SetInt("SessionNumber", 1);
        else
            PlayerPrefs.SetInt("SessionNumber", int.Parse(sessNumInputField.text));

        //If no Starting Difficulty is entered, uses 3 (3 objects) as the default
        if (startDiffInputField.text == "")
            PlayerPrefs.SetInt("StartingDifficulty", 3);
        else
            PlayerPrefs.SetInt("StartingDifficulty", int.Parse(startDiffInputField.text));

        //If no trial limit is entered, will output 999 as the default
        if (trialLimit.isOn) PlayerPrefs.SetInt("TrialLimit", int.Parse(sessLengthInputField.text));
        else PlayerPrefs.SetInt("TrialLimit", 999);

        Application.LoadLevel(index);
    }

    public void EnableSessionLimitInputField(Toggle checkbox)
    {

        if (checkbox.isOn)
            sessLengthInputField.image.enabled = true;
        else
            sessLengthInputField.image.enabled = false;

        // ??? How to shorten this? Took me an hour, and this was the only way I could get it to work.
        //    if (checkbox.name == "TrialsToggle")
        //    {
        //        if (checkbox.isOn)
        //            if (minuteLimit.isOn)
        //                minuteLimit.isOn = false;
        //            else
        //                sessLengthInputField.image.enabled = true;
        //        else
        //            if (!minuteLimit.isOn)
        //                sessLengthInputField.image.enabled = false;
        //    }

        //    else if (checkbox.isOn)
        //            if (trialLimit.isOn)
        //                trialLimit.isOn = false;
        //            else
        //                sessLengthInputField.image.enabled = true;
        //        else
        //            if (!trialLimit.isOn)
        //                sessLengthInputField.image.enabled = false;
        //}
    }
}