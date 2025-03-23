//______________________________________________
// ALIyerEdon
// https://assetstore.unity.com/publishers/23606
//______________________________________________

using UnityEngine;
using System.Collections;
using ALIyerEdon;

namespace ALIyerEdon
{
	public class Load_Settings : MonoBehaviour
	{


		public AudioSource music;

		void Start()
		{


            music.volume = PlayerPrefs.GetFloat("Music");

			if (FindObjectOfType<Race_Manager>())
			{
				if (PlayerPrefs.GetInt("ShowLocalPosition") == 3)
					FindObjectOfType<Race_Manager>().showLocalPosition = true;
				else
					FindObjectOfType<Race_Manager>().showLocalPosition = false;
			}

            Trive.Rendering.StochasticReflections ssr2;

            // Screen Space Reflections
            UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections ssr1;

            GameObject.FindObjectOfType<
                    UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile.TryGetSettings(out ssr1);

            GameObject.FindObjectOfType<
                    UnityEngine.Rendering.PostProcessing.PostProcessVolume>().profile.TryGetSettings(out ssr2);


            if (PlayerPrefs.GetInt("Reflection") == 0)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.Forward;

                ssr1.enabled.value = false;
                ssr2.enabled.value = false;
            }
            if (PlayerPrefs.GetInt("Reflection") == 1)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

                ssr1.enabled.value = true;
                ssr2.enabled.value = false;
            }
            if (PlayerPrefs.GetInt("Reflection") == 2)
            {
                GameObject.FindGameObjectWithTag("MainCamera").
                GetComponent<Camera>().renderingPath = RenderingPath.DeferredShading;

                ssr1.enabled.value = false;
                ssr2.enabled.value = true;

                ssr2.resolveDownsample.value = false;
                ssr2.raycastDownsample.value = false;
            }
        }

		public void Update_MusicVolume(float volume)
		{
			music.volume = volume;


		}
	}
}