#if UNITY_EDITOR
using UnityEditor.Presets;
#endif
using UnityEngine;
using cowsins; 

public class CrosshairShape : MonoBehaviour
{
    #if UNITY_EDITOR
    public Preset currentPreset;

    public Preset defaultPreset;
    #endif  
    [System.Serializable]
    public class Parts
    {
        public bool topPart, downPart, leftPart, rightPart;
    }

    public Parts parts;

    public string presetName; 

    #if UNITY_EDITOR
    private void Start() => ResetCrosshair(defaultPreset);

    private void ResetCrosshair(Preset preset) =>CowsinsUtilities.ApplyPreset(preset, this);
    #endif
}
