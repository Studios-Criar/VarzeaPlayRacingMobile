//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ALIyerEdon;

namespace ALIyerEdon
{
    public class SettingsMenu : MonoBehaviour
    {
        // UI items in settings menu window
        public Slider AccelSensibility;
        public Text AccelSensibilityInfo;
        public Slider SteerWheelSensibility;
        public Text SteerWheelInfo;
        public Slider musicVolume;
        public Text musicVolumeInfo;
        public Dropdown resolution;
        public Dropdown reflection;
        public Dropdown controlType;
        public Toggle positionUI;
        public Toggle localPosition;
        public Toggle dynamicCamera;
        public Toggle fpsCounter;

        // Start is called before the first frame update
        void Start()
        {
            // Load initilial settings            
            AccelSensibility.value = PlayerPrefs.GetFloat("accelSensibility");
            AccelSensibilityInfo.text = AccelSensibility.value.ToString();

            SteerWheelSensibility.value = PlayerPrefs.GetFloat("SteeringWheelSens");
            SteerWheelInfo.text = SteerWheelSensibility.value.ToString();

            controlType.value = PlayerPrefs.GetInt("ControlType");
            reflection.value = PlayerPrefs.GetInt("Reflection");

            resolution.value = PlayerPrefs.GetInt("ResQuality");

            musicVolume.value = PlayerPrefs.GetFloat("Music");

            // 3 => true  0 => false
            if (PlayerPrefs.GetInt("ShowLocalPosition") == 3)
                localPosition.isOn = true;
            else
                localPosition.isOn = false;

            // 3 => true  0 => false
            if (PlayerPrefs.GetInt("Dynamic Camera") == 3)
                dynamicCamera.isOn = true;
            else
                dynamicCamera.isOn = false;

            if (PlayerPrefs.GetInt("ShowPositionUI") == 3)
                positionUI.isOn = true;
            else
                positionUI.isOn = false;

            if (PlayerPrefs.GetInt("Show FPS") == 3)
                fpsCounter.isOn = true;
            else
                fpsCounter.isOn = false;
            

        }
        // Accelerometer  Sensibility
        public void Accelometer_Sensibility()
        {
            PlayerPrefs.SetFloat("accelSensibility", AccelSensibility.value);
            AccelSensibilityInfo.text = AccelSensibility.value.ToString();
        }
        public void SteerWheel_Sensibility()
        {
            PlayerPrefs.SetFloat("SteeringWheelSens", SteerWheelSensibility.value);
            SteerWheelInfo.text = SteerWheelSensibility.value.ToString();
        }
        public void Music_Volume()
        {
            PlayerPrefs.SetFloat("Music", musicVolume.value);
            musicVolumeInfo.text = musicVolume.value.ToString();
            FindObjectOfType<Load_Settings>().Update_MusicVolume(musicVolume.value);
        }
        // Control type : accelerometer , steering wheel , arrow keys
        public void Set_ControlType()
        {
            if (controlType.value == 0)
                PlayerPrefs.SetInt("ControlType", 0);
            if (controlType.value == 1)
                PlayerPrefs.SetInt("ControlType", 1);
            if (controlType.value == 2)
                PlayerPrefs.SetInt("ControlType", 2);
        }

        // Screen resolution quality : best for performance
        public void Set_Resolution()
        {
            PlayerPrefs.SetInt("ResQuality", resolution.value);

            if (PlayerPrefs.GetInt("ResQuality") == 0)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.5f),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 0.5f), true);
            }
            if (PlayerPrefs.GetInt("ResQuality") == 1)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 0.7f),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 0.7f), true);
            }
            if (PlayerPrefs.GetInt("ResQuality") == 2)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 1),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 1), true);
            }
            if (PlayerPrefs.GetInt("ResQuality") == 3)
            {
                Screen.SetResolution((int)(PlayerPrefs.GetInt("OriginalX") * 1.5f),
                    (int)(PlayerPrefs.GetInt("OriginalY") * 1.5f), true);
            }
            if (PlayerPrefs.GetInt("ResQuality") == 4)
            {
                Screen.SetResolution(3840,2160, true);
            }
        }



        // Screen space reflections
        public void Set_Reflection()
        {
            Trive.Rendering.StochasticReflections ssr2;

            // Screen Space Reflections
            UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections ssr1;

            GameObject.FindObjectOfType<
                    UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile.TryGetSettings(out ssr1);

            GameObject.FindObjectOfType<
                    UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile.TryGetSettings(out ssr2);

            if (reflection.value == 0)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.Forward;

                ssr1.enabled.value = false;
                ssr2.enabled.value = false;
            }
            if (reflection.value == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

                ssr1.enabled.value = true;
                ssr2.enabled.value = false;
            }
            if (reflection.value == 2)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

                ssr1.enabled.value = false;
                ssr2.enabled.value = true;

                ssr2.resolveDownsample.value = false;
                ssr2.raycastDownsample.value = false;
            }
        }

        public void Toggle_LocalPosition()
        {
            StartCoroutine(Toggle_LocalPosition_Save());
        }

        IEnumerator Toggle_LocalPosition_Save()
        {
            yield return new WaitForEndOfFrame();

            if (localPosition.isOn)
                PlayerPrefs.SetInt("ShowLocalPosition", 3);
            else
                PlayerPrefs.SetInt("ShowLocalPosition", 0);
        }

        public void Toggle_DaynamicCamera()
        {
            StartCoroutine(Toggle_DaynamicCamera_Save());
        }

        IEnumerator Toggle_DaynamicCamera_Save()
        {
            yield return new WaitForEndOfFrame();

            if (dynamicCamera.isOn)
                PlayerPrefs.SetInt("Dynamic Camera", 3);
            else
                PlayerPrefs.SetInt("Dynamic Camera", 0);
        }


        // Racing Position UI Display
        public void Toggle_PositionUI()
        {
            StartCoroutine(Toggle_PositionUI_Save());
        }

        IEnumerator Toggle_PositionUI_Save()
        {
            yield return new WaitForEndOfFrame();

            if (positionUI.isOn)
                PlayerPrefs.SetInt("ShowPositionUI", 3);
            else
                PlayerPrefs.SetInt("ShowPositionUI", 0);
        }

        // Diplay fps
        public void Toggle_FPSCounter()
        {
            StartCoroutine(Toggle_FPSCounter_Save());
        }

        IEnumerator Toggle_FPSCounter_Save()
        {
            yield return new WaitForEndOfFrame();

            if (fpsCounter.isOn)
                PlayerPrefs.SetInt("Show FPS", 3);
            else
                PlayerPrefs.SetInt("Show FPS", 0);
        }

        public void Close_Save()
        {
            PlayerPrefs.SetInt("Reflection", reflection.value);
        }
    }
}