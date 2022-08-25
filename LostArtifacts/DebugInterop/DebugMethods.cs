using DebugMod;

namespace LostArtifacts.Debug
{
	public class DebugMethods
	{
		[BindableMethod(name = "Unlock all artifacts", category = "Lost Artifacts")]
		public static void UnlockArtifacts()
		{
			for(int i = 0; i < LostArtifacts.Settings.unlocked.Length; i++)
			{
				LostArtifacts.Settings.unlocked[i] = true;
			}
		}

		[BindableMethod(name = "Toggle unlock all slots", category = "Lost Artifacts")]
		public static void UnlockSlots()
		{
			LostArtifacts.Settings.unlockedSlots = !LostArtifacts.Settings.unlockedSlots;
		}
	}
}
