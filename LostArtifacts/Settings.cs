﻿using Modding;

namespace LostArtifacts
{
	public class Settings : ModHooksGlobalSettings
	{
		public int slotHandle { get; set; } = -1;
		public int slotBladeL { get; set; } = -1;
		public int slotBladeR { get; set; } = -1;
		public int slotHead { get; set; } = -1;
	}
}