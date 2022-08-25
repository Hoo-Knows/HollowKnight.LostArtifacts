namespace LostArtifacts.Debug
{
	public static class ArtifactDebug
	{
		public static void AddToDebug()
		{
			DebugMod.DebugMod.AddToKeyBindList(typeof(DebugMethods));
		}
	}
}
