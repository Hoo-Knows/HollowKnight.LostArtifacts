using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.Json;
using RandomizerMod.Menu;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using RandomizerMod.Logging;
using System.IO;
using Newtonsoft.Json;

namespace LostArtifacts.Rando
{
	public static class ArtifactRando
	{
		public static bool isRandoSave => RandomizerMod.RandomizerMod.IsRandoSave;

		public static void HookRando()
		{
			RCData.RuntimeLogicOverride.Subscribe(-498f, DefineLogicItem);
			RequestBuilder.OnUpdate.Subscribe(-498f, DefineArtifacts);
			RequestBuilder.OnUpdate.Subscribe(50f, AddArtifacts);
			RandomizerMenuAPI.AddMenuPage(RandoMenu.ConstructMenu, RandoMenu.HandleButton);
			SettingsLog.AfterLogSettings += AddSettingsToLog;
		}

		private static void DefineLogicItem(GenerationSettings gs, LogicManagerBuilder lmb)
		{
			if(!LostArtifacts.RandoSettings.EnableArtifacts) return;

			foreach(Artifact artifact in LostArtifacts.Instance.artifacts)
			{
				lmb.AddItem(new EmptyItem(artifact.InternalName()));
			}

			using Stream stream = typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources.logic.json");
			JsonLogicFormat fmt = new();
			lmb.DeserializeFile(LogicFileType.Locations, fmt, stream);
		}

		private static void DefineArtifacts(RequestBuilder rb)
		{
			if(!LostArtifacts.RandoSettings.EnableArtifacts) return;

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

			// -1 or 0 means artifacts will be in the main item group, snippet from Flibber
			if(LostArtifacts.RandoSettings.ArtifactGroup > 0)
			{
				ItemGroupBuilder artifactGroup = null;
				string label = RBConsts.SplitGroupPrefix + LostArtifacts.RandoSettings.ArtifactGroup;
				foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups())
				{
					if(igb.label == label)
					{
						artifactGroup = igb;
						break;
					}
				}
				artifactGroup ??= rb.MainItemStage.AddItemGroup("Artifacts");

				rb.OnGetGroupFor.Subscribe(0.01f, ResolveGroup);
				bool ResolveGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
				{
					if(!LostArtifacts.Instance.artifactNames.Contains(item))
					{
						gb = default;
						return false;
					}

					gb = artifactGroup;
					return true;
				}
			}
		}

		private static void AddArtifacts(RequestBuilder rb)
		{
			if(!LostArtifacts.RandoSettings.RandomizeArtifacts) return;

			foreach(Artifact artifact in LostArtifacts.Instance.artifacts)
			{
				rb.AddItemByName(artifact.InternalName());
				if(LostArtifacts.RandoSettings.UseCustomLocations) rb.AddLocationByName(artifact.InternalName());
			}
		}

		private static void AddSettingsToLog(LogArguments args, TextWriter tw)
		{
			tw.WriteLine("Logging LostArtifacts settings:");
			using JsonTextWriter jtw = new(tw) { CloseOutput = false, };
			RandomizerMod.RandomizerData.JsonUtil._js.Serialize(jtw, LostArtifacts.RandoSettings);
			tw.WriteLine();
		}
	}
}
