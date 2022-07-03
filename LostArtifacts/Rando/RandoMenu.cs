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

		public static bool HandleButton(MenuPage landingPage, out SmallButton button)
		{
			button = new(landingPage, LostArtifacts.Instance.GetName());
			button.AddHideAndShowEvent(landingPage, SettingsPage);
			return true;
		}

		public static void ConstructMenu(MenuPage landingPage)
		{
			SettingsPage = new MenuPage(LostArtifacts.Instance.GetName(), landingPage);
			MenuElementFactory<RandoSettings> factory =
				new MenuElementFactory<RandoSettings>(SettingsPage, LostArtifacts.RandoSettings);
			IMenuElement[] elements = factory.Elements;
			new VerticalItemPanel(SettingsPage, new Vector2(0f, 300f), 75f, true, elements);
		}
	}
}
