using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AssetFactory.Audio;

#if LOCALIZATION
using UnityEngine.Localization.Settings;
#endif

//Originally from AssetFactory
namespace AssetFactory.UI
{
	public class SettingsMenu : SubMenu
	{
		[SerializeField] private TMP_Dropdown resolutionUI;
		[SerializeField] private Toggle fullscreenUI;
#if LOCALIZATION
		[SerializeField] private TMP_Dropdown languageUI;
#endif
		[SerializeField] private Button rebindButton;
		[SerializeField] private Slider masterVolume;
		[SerializeField] private Slider musicVolume;
		[SerializeField] private Slider sfxVolume;
		[SerializeField] private Slider uiVolume;

		private List<Resolution> resolutionList;
		public void InitializeSettings()
		{
			masterVolume.SetValueWithoutNotify(Settings.current.masterVolume);
			sfxVolume.SetValueWithoutNotify(Settings.current.sfxVolume);
			uiVolume.SetValueWithoutNotify(Settings.current.uiVolume);
			musicVolume.SetValueWithoutNotify(Settings.current.musicVolume);

			rebindButton.interactable = !PauseMenu.IsPaused;
			RefreshResolutionList();
			fullscreenUI.SetIsOnWithoutNotify(Settings.current.fullscreen);

//#if LOCALIZATION
//			if (LocalizationSettings.InitializationOperation.IsDone)
//			{
//				languageUI.interactable = true;
//				RefreshLocalesList();
//			}
//			else
//			{
//				languageUI.ClearOptions();
//				languageUI.captionText.text = "-";
//				languageUI.interactable = false;
//			}
//#endif
		}
		private void RefreshResolutionList()
		{
			resolutionList = new List<Resolution>(Screen.resolutions.Length);
			for (int i = 0; i < Screen.resolutions.Length; i++)
			{
				Resolution fromScreen = Screen.resolutions[i];
				if (fromScreen.height < 360)
					continue;
				for (int j = 0; j < resolutionList.Count; j++)
				{
					Resolution fromList = resolutionList[j];
					if (fromList.width == fromScreen.width && fromList.height == fromScreen.height)
					{
						if (fromList.refreshRate < fromScreen.refreshRate)
							goto EndOfLoop;
						else
						{
							resolutionList[j] = fromScreen;
							goto EndOfLoop;
						}
					}
				}
				resolutionList.Add(fromScreen);
			EndOfLoop:
				continue;
			}

			resolutionUI.options = new List<TMP_Dropdown.OptionData>(resolutionList.Count);
			for (int j = 0; j < resolutionList.Count; j++)
			{
				Resolution r = resolutionList[j];

				resolutionUI.options.Add(new TMP_Dropdown.OptionData($"{r.width}x{r.height}"));

				if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
				{
					resolutionUI.SetValueWithoutNotify(j);
					return;
				}
			}
			resolutionUI.captionText.text = "-";
		}
#if LOCALIZATION
		private void RefreshLocalesList()
		{
			LocalesProvider pr = (LocalesProvider)LocalizationSettings.Instance.GetAvailableLocales();
			languageUI.ClearOptions();
			languageUI.options = new List<TMP_Dropdown.OptionData>(pr.Locales.Count);
			int select = 0;
			for (int i = 0; i < pr.Locales.Count; i++)
			{
				languageUI.options.Add(new TMP_Dropdown.OptionData(pr.Locales[i].LocaleName));
				if (pr.Locales[i].Identifier == LocalizationSettings.SelectedLocale.Identifier)
				{
					select = i;
				}
			}
			if (select == 0 && select < languageUI.options.Count - 1)
				languageUI.value = select + 1;
			languageUI.value = select;
		}
#endif
		public void UpdateVolume()
		{
			Settings.current.masterVolume = masterVolume.value;
			Settings.current.musicVolume = musicVolume.value;
			Settings.current.sfxVolume = sfxVolume.value;
			Settings.current.uiVolume = uiVolume.value;
			AudioManager.Inst.UpdateMixers();
		}

		public void SetSettings()
		{
			UpdateVolume();
			Resolution r = resolutionList[resolutionUI.value];
			Settings.current.fullscreen = fullscreenUI.isOn;

			if (r.width != Settings.current.resolution.width || r.height != Settings.current.resolution.height)
			{
				Settings.current.resolution = r;
			}

#if LOCALIZATION
			LocalesProvider pr = (LocalesProvider)LocalizationSettings.AvailableLocales;
			if (LocalizationSettings.SelectedLocale != pr.Locales[languageUI.value])
				LocalizationSettings.SelectedLocale = pr.Locales[languageUI.value];
#endif

			Settings.current.Save();
			Settings.current.SetScreen();
		}

		
		public void Revert()
		{
			Settings.current = new Settings();
			Settings.current.SetScreen();
			AudioManager.Inst.UpdateMixers();
		}
	}
}