using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ResolutionSettings : MonoBehaviour
{

    public Dropdown dropdown;
    public Toggle toggle;
    void Start()
    {
        CreateResolutionDropdown();
    }
    private void CreateResolutionDropdown()
    {
        dropdown.ClearOptions();
        Resolution[] resolutions = Screen.resolutions;
        List<string> resolutionOptions = new List<string>();
        foreach (Resolution resolution in resolutions)
        {
            string option = resolution.width + "x" + resolution.height;
            resolutionOptions.Add(option);
        }
        dropdown.AddOptions(resolutionOptions);
        dropdown.value = FindCurrentResolutionIndex();
        dropdown.RefreshShownValue();

    }
    int FindCurrentResolutionIndex()
    {
        Resolution currentResolution = Screen.currentResolution;

        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == currentResolution.width &&
                Screen.resolutions[i].height == currentResolution.height)
            {
                return i;
            }
        }
        return 0;
    }
    public void OnResolutionChanged()
    {
        string selectedResolution = dropdown.options[dropdown.value].text;

        string[] resolutionParts = selectedResolution.Split('x');
        int width = int.Parse(resolutionParts[0].Trim());
        int height = int.Parse(resolutionParts[1].Trim());

        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    public void WindowMode()
    {
        Screen.fullScreen = !toggle.isOn;
    }
}
