// Nguyen Ngoc Kha - Base - V 1.0.6
using System.Collections.Generic;
using UnityEngine;
using Services;
using UnityEngine.SceneManagement;
using Audio;

namespace Entry
{
	public class EntryController : MonoBehaviour
	{
		private const string soundObjectName = "Sound";
		[SerializeField] private EntryModel model;

		[SerializeField] private List<Sound> sounds;
		[SerializeField] private Music music;
		[SerializeField] private GameObject musicObject;
		private GameServices gameServices = null;

		void Awake()
		{
			if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
			{
                GameObject gameServiceObject = new(nameof(GameServices))
                {
                    tag = Constants.ServicesTag
                };
                gameServices = gameServiceObject.AddComponent<GameServices>();
				DontDestroyOnLoad(musicObject);
				GameObject soundObject = new(soundObjectName);
				DontDestroyOnLoad(soundObject);
				gameServices.AddService(new AudioService(music, sounds, soundObject));
				gameServices.AddService(new GameService(model.TOSURL, model.PrivacyURL, model.RateURL));
				var audioService = gameServices.GetService<AudioService>();
				gameServices.AddService(new AudioService(music, sounds, soundObject));
				audioService.MusicOn = true;
				audioService.SoundOn = true;
				audioService.StopMusic();
			}
		}

	}
}
