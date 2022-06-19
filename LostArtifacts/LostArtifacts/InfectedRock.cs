using System;
using System.Collections.Generic;
using Modding;

namespace LostArtifacts
{
	public class InfectedRock : Artifact
	{
		public override int ID() => 16;
		public override string Name() => "Infected Rock";
		public override string Description() => "The Broken Vessel had to make friends where it could.";
		public override string Levels() => "3, 7, 11 extra SOUL";
		public override string TraitName() => "Forgotten";
		public override string TraitDescription() => "Increases SOUL gain from striking enemies with the nail";

		public override void Activate()
		{
			base.Activate();

			ModHooks.SoulGainHook += SoulGainHook;
		}

		private int SoulGainHook(int soul)
		{
			return soul + 4 * level - 1;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.SoulGainHook -= SoulGainHook;
		}
	}
}
