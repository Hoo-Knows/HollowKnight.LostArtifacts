﻿using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.UIDefs;
using Modding;
using Satchel;
using Satchel.BetterMenus;
using SFCore;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LostArtifacts
{
	public class LostArtifacts : Mod, ILocalSettings<Settings>, ICustomMenuMod
	{
		public static LostArtifacts Instance;
		public static Dictionary<string, Dictionary<string, GameObject>> Preloads;

		private GameObject prefabUI;
		private string iconPath;

		public GameObject artifactsGO;
		public Artifact[] artifacts;
		public GameObject artifactsUI;
		public PlayMakerFSM pageFSM;

		public static Settings Settings { get; set; } = new Settings();

		public bool ToggleButtonInsideMenu => false;
		private Menu MenuRef;

		public void OnLoadLocal(Settings s)
		{
			Log("Loading Lost Artifacts settings");
			Settings = s;
		}
		public Settings OnSaveLocal()
		{
			Log("Saving Lost Artifacts settings");
			return Settings;
		}

		public override string GetVersion() => AssemblyUtils.GetAssemblyVersionHash();

		public LostArtifacts() : base("LostArtifacts")
		{
			Instance = this;

			//Load UI prefab
			AssetBundle assetBundle = AssetBundle.LoadFromStream(
				typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources.artifactsui"));
			if(assetBundle == null)
			{
				Log("Failed to load AssetBundle!");
				return;
			}
			prefabUI = assetBundle.LoadAsset<GameObject>("Artifacts");

			artifactsUI = UnityEngine.Object.Instantiate(prefabUI);
			GameObject.DontDestroyOnLoad(artifactsUI);
			artifactsUI.SetActive(false);

			//Add inventory page with SFCore
			InventoryHelper.AddInventoryPage(
				InventoryPageType.Empty,
				"LostArtifacts.Artifacts",
				"LostArtifacts.PageConvKey",
				"Artifacts",
				"artifactsUnlocked",
				EditInventory);

			//Add artifacts
			artifactsGO = new GameObject("Artifacts GO");
			artifacts = new Artifact[20];
			UnityEngine.Object.DontDestroyOnLoad(artifactsGO);

			//Create icons directory if it doesn't exist
			iconPath = Path.Combine(AssemblyUtils.getCurrentDirectory(), "Icons");
			IoUtils.EnsureDirectory(iconPath);

			AddArtifact<TravelersGarment>();
			AddArtifact<PavingStone>();
			AddArtifact<LushMoss>();
			AddArtifact<NoxiousShroom>();
			AddArtifact<CryingStatue>();
			AddArtifact<TotemShard>();
			AddArtifact<DungBall>();
			AddArtifact<Tumbleweed>();
			AddArtifact<ChargedCrystal>();
			AddArtifact<Dreamwood>();
			AddArtifact<LumaflyEssence>();
			AddArtifact<ThornedLeaf>();
			AddArtifact<WeaverSilk>();
			AddArtifact<WyrmAsh>();
			AddArtifact<BeastShell>();
			AddArtifact<Honeydrop>(); //Needs visual feedback
			AddArtifact<InfectedRock>();
			AddArtifact<Buzzsaw>();
			AddArtifact<Voidstone>();
			AddArtifact<AttunedJewel>(); //Needs visual feedback
		}

		public override List<ValueTuple<string, string>> GetPreloadNames()
		{
			return new List<ValueTuple<string, string>>
			{
				new ValueTuple<string, string>("GG_Uumuu", "Mega Jellyfish GG")
			};
		}

		public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
		{
			Log("Initializing");

			Preloads = preloadedObjects;

			foreach(Artifact artifact in artifacts)
			{
				ItemChanger.Items.BoolItem item = new ItemChanger.Items.BoolItem()
				{
					name = artifact.InternalName(),
					fieldName = "unlockedArtifact_" + artifact.ID(),
					UIDef = new MsgUIDef()
					{
						name = new LanguageString("UI", "LostArtifacts." + artifact.InternalName()),
						shopDesc = new LanguageString("UI", "LostArtifacts." + artifact.InternalName() + "Desc"),
						sprite = artifact.sprite
					}
				};
				Finder.DefineCustomItem(item);
			}

			On.HeroController.Start += HeroControllerStart;
			On.HeroController.OnDisable += HeroControllerOnDisable;
			On.UIManager.StartNewGame += UIManagerStartNewGame;
			ModHooks.LanguageGetHook += LanguageGetHook;
			ModHooks.GetPlayerBoolHook += GetPlayerBoolHook;
			ModHooks.SetPlayerBoolHook += SetPlayerBoolHook;

			Log("Initialized");
		}

		private void UnlockAllArtifacts()
		{
			foreach(Artifact artifact in artifacts)
			{
				Settings.unlocked[artifact.ID()] = true;
			}
		}

		private void HeroControllerStart(On.HeroController.orig_Start orig, HeroController self)
		{
			//Activate artifacts if they are equipped
			orig(self);
			foreach(Artifact artifact in artifacts)
			{
				if(artifact == null || artifact.active) continue;

				//Activate artifact
				artifact.level = 0;
				if(Settings.slotHandle == artifact.ID()) artifact.level = 1;
				if(Settings.slotBladeL == artifact.ID()) artifact.level = 2;
				if(Settings.slotBladeR == artifact.ID()) artifact.level = 2;
				if(Settings.slotHead == artifact.ID()) artifact.level = 3;
				if(artifact.level > 0)
				{
					artifact.Activate();
				}
			}
		}

		private void HeroControllerOnDisable(On.HeroController.orig_OnDisable orig, HeroController self)
		{
			//Deactivate artifacts
			foreach(Artifact artifact in artifacts)
			{
				if(artifact == null || !artifact.active) continue;

				artifact.Deactivate();
			}
			orig(self);
		}

		private void EditInventory(GameObject page)
		{
			//Cache FSM
			pageFSM = page.LocateMyFSM("Empty UI");

			//Create state
			pageFSM.AddState("Artifacts");

			//Change to state after init
			pageFSM.ChangeTransition("Init Heart Piece", "FINISHED", "Artifacts");

			//Add left and right arrow transitions out of the inventory
			pageFSM.AddTransition("Artifacts", "LEFT", "Move Pane L");
			pageFSM.AddTransition("Artifacts", "RIGHT", "Move Pane R");

			//Set inventory active
			pageFSM.AddCustomAction("Artifacts", () => artifactsUI.SetActive(true));

			//Set inventory inactive
			pageFSM.InsertCustomAction("Move Pane L", () => CloseInventory(false), 0);
			pageFSM.InsertCustomAction("Move Pane R", () => CloseInventory(false), 0);

			On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter += CloseInventoryHook;
		}

		private void CloseInventoryHook(On.HutongGames.PlayMaker.Actions.SendEventByName.orig_OnEnter orig, SendEventByName self)
		{
			orig(self);
			if(self.Fsm.Name == "Inventory Control" && self.State.Name == "Loop Through" && self.sendEvent.Value == "INV PANEL CHANGE")
			{
				CloseInventory(false);
			}
			if(self.Fsm.Name == "Inventory Control" &&
				(self.State.Name == "Close" || self.State.Name == "Damage Close" || self.State.Name == "R Lock Close") &&
				self.sendEvent.Value == "MAP KEY DOWN")
			{
				CloseInventory(true);
			}
		}

		private void CloseInventory(bool full)
		{
			artifactsUI.SetActive(false);
			//Needed if closing the inventory while on the Artifacts tab
			if(full) pageFSM.SetState("Init");
		}

		public void AddArtifact<T>() where T : Artifact
		{
			Artifact artifact = artifactsGO.AddComponent<T>();
			artifact.sprite = new ArtifactSprite()
			{
				name = artifact.InternalName()
			};
			artifacts[artifact.ID()] = artifact;
		}

		public Sprite GetArtifactSprite(string name)
		{
			Texture2D texture = TextureUtils.createTextureOfColor(64, 64, Color.clear);

			string path = Path.Combine(iconPath, name + ".png");

			//Extract sprite from Resources if it doesn't exist
			if(!File.Exists(path))
			{
				try
				{
					Stream stream = typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources." + name + ".png");
					{
						var buffer = new byte[stream.Length];
						stream.Read(buffer, 0, buffer.Length);
						File.WriteAllBytes(path, buffer);
						stream.Dispose();
					}
				}
				catch
				{
					Log("Failed to extract sprite for " + name);
				}
			}
			if(File.Exists(path)) texture = GetPremultipliedAlpha(TextureUtils.LoadTextureFromFile(path));

			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 64f);
		}

		public static Texture2D GetPremultipliedAlpha(Texture2D source)
		{
			Color[] sourcePixels = source.GetPixels();
			Color[] destPixels = new Color[sourcePixels.Length];

			for(int i = 0; i < sourcePixels.Length; i++)
			{
				Color px = sourcePixels[i];
				destPixels[i] = new Color(px.r * px.a, px.g * px.a, px.b * px.a, px.a);
			}

			Texture2D dest = new(source.width, source.height, TextureFormat.ARGB32, false);
			dest.SetPixels(destPixels);
			dest.Apply();
			return dest;
		}

		private void UIManagerStartNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
		{
			orig(self, permaDeath, bossRush);

			if(bossRush)
			{
				UnlockAllArtifacts();
				return;
			}

			ItemChangerMod.CreateSettingsProfile();

			List<AbstractPlacement> placements = new List<AbstractPlacement>();
			foreach(Artifact artifact in artifacts)
			{
				placements.Add(artifact.Location().Wrap().Add(Finder.GetItem(artifact.InternalName())));
			}
			ItemChangerMod.AddPlacements(placements, PlacementConflictResolution.Ignore);
		}

		private string LanguageGetHook(string key, string sheetTitle, string orig)
		{
			if(key.StartsWith("LostArtifacts."))
			{
				string str = key.Split(new char[] { '.' })[1];
				if(str == "PageConvKey") return "Artifacts";
				foreach(Artifact artifact in artifacts)
				{
					if(str == artifact.InternalName())
					{
						return artifact.Name();
					}
					if(str == artifact.InternalName() + "Desc")
					{
						return artifact.Description();
					}
				}
			}
			return orig;
		}

		private bool GetPlayerBoolHook(string name, bool orig)
		{
			if(name == "artifactsUnlocked")
			{
				return PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) > 0;
			}
			if(name.StartsWith("unlockedArtifact_"))
			{
				return Settings.unlocked[int.Parse(name.Split(new char[] { '_' })[1])];
			}
			return orig;
		}

		private bool SetPlayerBoolHook(string name, bool orig)
		{
			if(name.StartsWith("unlockedArtifact_"))
			{
				Settings.unlocked[int.Parse(name.Split(new char[] { '_' })[1])] = orig;
			}
			return orig;
		}

		public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
		{
			if(MenuRef == null)
			{
				MenuRef = new Menu("Lost Artifacts",
					new Element[]
					{
						new MenuButton("Unlock all artifacts", "",
						(_) => UnlockAllArtifacts(),
						Id: "UnlockButton")
					}
				);
			}
			return MenuRef.GetMenuScreen(modListMenu);
		}
	}
}