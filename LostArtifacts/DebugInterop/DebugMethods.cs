using DebugMod;

namespace LostArtifacts.Debug
{
	public class DebugMethods
	{
		[BindableMethod(name = "Unlock All Artifacts", category = "Lost Artifacts")]
		public static void UnlockArtifacts()
		{
			for(int i = 0; i < LostArtifacts.Settings.unlocked.Length; i++)
			{
				LostArtifacts.Settings.unlocked[i] = true;
			}
		}

		[BindableMethod(name = "Toggle Unlock All Slots", category = "Lost Artifacts")]
		public static void UnlockSlots()
		{
			LostArtifacts.Settings.unlockedSlots = !LostArtifacts.Settings.unlockedSlots;
		}
	}
}
