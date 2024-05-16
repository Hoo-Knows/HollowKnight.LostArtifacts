using ItemChanger;
using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using UnityEngine;

namespace LostArtifacts.Rando
{
	public static class RandoMenu
	{
		private static MenuPage SettingsPage;
		private static SmallButton pageRootButton;
		private static ToggleButton enableArtifactsToggle;
		private static ToggleButton randomizeArtifactsToggle;
		private static ToggleButton useCustomLocations;
		private static NumericEntryField<int> artifactGroupField;

		public static bool HandleButton(MenuPage landingPage, out SmallButton button)
		{
			pageRootButton = new(landingPage, LostArtifacts.Instance.GetName());
			pageRootButton.AddHideAndShowEvent(landingPage, SettingsPage);
			ChangeTopLevelColor();
			button = pageRootButton;
			return true;
		}

		public static void ConstructMenu(MenuPage landingPage)
		{
			SettingsPage = new MenuPage(LostArtifacts.Instance.GetName(), landingPage);
			MenuElementFactory<RandoSettings> factory = new MenuElementFactory<RandoSettings>(SettingsPage, LostArtifacts.RandoSettings);
			IMenuElement[] elements = factory.Elements;
			new VerticalItemPanel(SettingsPage, new Vector2(0f, 300f), 75f, true, elements);

			enableArtifactsToggle = (ToggleButton)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.EnableArtifacts)];
			randomizeArtifactsToggle = (ToggleButton)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.RandomizeArtifacts)];
			useCustomLocations = (ToggleButton)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.UseCustomLocations)];
			artifactGroupField = (NumericEntryField<int>)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.ArtifactGroup)];
			enableArtifactsToggle.SelfChanged += EnableArtifactsChanged;
			randomizeArtifactsToggle.SelfChanged += RandomizeArtifactsChanged;
			useCustomLocations.SelfChanged += UseCustomLocationsChanged;
			artifactGroupField.SelfChanged += ArtifactGroupChanged;

			if(!LostArtifacts.RandoSettings.RandomizeArtifacts)
			{
				artifactGroupField.Hide();
				useCustomLocations.Hide();
			}
			else
			{
				artifactGroupField.Show();
				useCustomLocations.Show();
			}
		}

		private static void EnableArtifactsChanged(IValueElement obj)
		{
			if(!(bool)obj.Value)
			{
				randomizeArtifactsToggle.SetValue(false);
			}
			ChangeTopLevelColor();
		}

		private static void RandomizeArtifactsChanged(IValueElement obj)
		{
			if(!(bool)obj.Value)
			{
				artifactGroupField.Hide();
				useCustomLocations.Hide();
			}
			else
			{
				enableArtifactsToggle.SetValue(true);
				artifactGroupField.Show();
				useCustomLocations.Show();
			}
			ChangeTopLevelColor();
		}

		private static void ArtifactGroupChanged(IValueElement obj)
		{
			// Randomizing artifacts within their own group requires adding custom locations
			if((int)obj.Value > 0)
			{
				useCustomLocations.SetValue(true);
			}
		}

		private static void UseCustomLocationsChanged(IValueElement obj)
		{
			// Can't remove custom locations while still randomizing artifacts within their own group
			if(!(bool)obj.Value)
			{
				artifactGroupField.SetValue(-1);
			}
		}

		private static void ChangeTopLevelColor()
		{
			if(pageRootButton != null)
			{
				pageRootButton.Text.color = Colors.FALSE_COLOR;
				if(LostArtifacts.RandoSettings.EnableArtifacts) pageRootButton.Text.color = Colors.DEFAULT_COLOR;
				if(LostArtifacts.RandoSettings.RandomizeArtifacts) pageRootButton.Text.color = Colors.TRUE_COLOR;
			}
		}

		// RSM Interop
		public static void Apply(RandoSettings settings)
		{
			enableArtifactsToggle.SetValue(settings.EnableArtifacts);
			randomizeArtifactsToggle.SetValue(settings.RandomizeArtifacts);
			useCustomLocations.SetValue(settings.UseCustomLocations);
			artifactGroupField.SetValue(settings.ArtifactGroup);
		}

		public static void Disable()
		{
			enableArtifactsToggle.SetValue(false);
			randomizeArtifactsToggle.SetValue(false);
			useCustomLocations.SetValue(false);
			artifactGroupField.SetValue(-1);
		}
	}
}
