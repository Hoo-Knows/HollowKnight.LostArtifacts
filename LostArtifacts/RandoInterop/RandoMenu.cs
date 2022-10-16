using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using System;
using UnityEngine;

namespace LostArtifacts.Rando
{
	public static class RandoMenu
	{
		private static MenuPage SettingsPage;
		private static SmallButton pageRootButton;
		private static ToggleButton enabledButton;
		private static ToggleButton randomizedButton;
		private static ToggleButton useMainItemGroupButton;

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
			MenuElementFactory<RandoSettings> factory =
				new MenuElementFactory<RandoSettings>(SettingsPage, LostArtifacts.RandoSettings);
			IMenuElement[] elements = factory.Elements;
			new VerticalItemPanel(SettingsPage, new Vector2(0f, 300f), 75f, true, elements);

			enabledButton = (ToggleButton)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.Enabled)];
			randomizedButton = (ToggleButton)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.RandomizeArtifacts)];
			useMainItemGroupButton = (ToggleButton)factory.ElementLookup[nameof(LostArtifacts.RandoSettings.UseMainItemGroup)];
			enabledButton.SelfChanged += EnabledChanged;
			randomizedButton.SelfChanged += RandomizedChanged;
			useMainItemGroupButton.SelfChanged += UseMainItemGroupChanged;
		}

		private static void EnabledChanged(IValueElement obj)
		{
			if(!(bool)obj.Value) randomizedButton.SetValue(false);
		}

		private static void RandomizedChanged(IValueElement obj)
		{
			if(!(bool)obj.Value) useMainItemGroupButton.SetValue(false);
			else enabledButton.SetValue(true);
			ChangeTopLevelColor();
		}

		private static void UseMainItemGroupChanged(IValueElement obj)
		{
			if((bool)obj.Value) randomizedButton.SetValue(true);
		}

		private static void ChangeTopLevelColor()
		{
			if(pageRootButton != null)
			{
				pageRootButton.Text.color = Colors.FALSE_COLOR;
				if(LostArtifacts.RandoSettings.Enabled) pageRootButton.Text.color = Colors.DEFAULT_COLOR;
				if(LostArtifacts.RandoSettings.RandomizeArtifacts) pageRootButton.Text.color = Colors.TRUE_COLOR;
			}
		}
	}
}
