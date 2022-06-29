using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.Menu;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using System.IO;
using UnityEngine;

namespace LostArtifacts
{
	public static class ArtifactRando
	{
		public static RandoSettings RandoSettings = new RandoSettings();
		private static MenuPage SettingsPage;

		public static void HookRando()
		{
			RCData.RuntimeLogicOverride.Subscribe(-498f, DefineLogicItem);
			RequestBuilder.OnUpdate.Subscribe(-498f, DefineArtifacts);
			RequestBuilder.OnUpdate.Subscribe(50f, AddArtifacts);
			RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
		}

		private static void DefineLogicItem(GenerationSettings gs, LogicManagerBuilder lmb)
		{
			if(!RandoSettings.RandomizeArtifacts) return;

			foreach(Artifact artifact in LostArtifacts.Instance.artifacts)
			{
				lmb.AddItem(new EmptyItem(artifact.InternalName()));
			}

			using Stream stream = typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources.logic.json");
			lmb.DeserializeJson(LogicManagerBuilder.JsonType.Locations, stream);
		}

		private static void DefineArtifacts(RequestBuilder rb)
		{
			if(!RandoSettings.RandomizeArtifacts) return;

			foreach(Artifact artifact in LostArtifacts.Instance.artifacts)
			{
				rb.EditItemRequest(artifact.InternalName(), info =>
				{
					info.getItemDef = () => new()
					{
						Name = artifact.InternalName(),
						Pool = "Artifacts",
						MajorItem = false,
						PriceCap = 500
					};
				});
			}

			ItemGroupBuilder artifactGroup = null;
			foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups())
			{
				if(igb.label == "Artifacts")
				{
					artifactGroup = igb;
					break;
				}
			}
			artifactGroup ??= rb.MainItemStage.AddItemGroup("Artifacts");

			rb.OnGetGroupFor.Subscribe(0.01f, ResolveGroup);
			bool ResolveGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
			{
				if(type == RequestBuilder.ElementType.Transition)
				{
					gb = default;
					return false;
				}

				if(!LostArtifacts.Instance.artifactNames.Contains(item) || RandoSettings.UseMainItemGroup)
				{
					gb = default;
					return false;
				}

				gb = artifactGroup;
				return true;
			}
		}

		private static void AddArtifacts(RequestBuilder rb)
		{
			if(!RandoSettings.RandomizeArtifacts) return;

			foreach(Artifact artifact in LostArtifacts.Instance.artifacts)
			{
				rb.AddItemByName(artifact.InternalName());
				rb.AddLocationByName(artifact.InternalName());
			}
		}

		private static bool HandleButton(MenuPage landingPage, out SmallButton button)
		{
			button = new(landingPage, LostArtifacts.Instance.GetName());
			button.AddHideAndShowEvent(landingPage, SettingsPage);
			return true;
		}

		private static void ConstructMenu(MenuPage landingPage)
		{
			SettingsPage = new MenuPage(LostArtifacts.Instance.GetName(), landingPage);
			MenuElementFactory<RandoSettings> factory = new MenuElementFactory<RandoSettings>(SettingsPage, RandoSettings);
			IMenuElement[] elements = factory.Elements;
			new VerticalItemPanel(SettingsPage, new Vector2(0f, 300f), 75f, true, elements);
		}
	}
}
