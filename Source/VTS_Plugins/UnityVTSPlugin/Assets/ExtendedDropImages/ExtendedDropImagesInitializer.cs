using System.Diagnostics;

namespace Assets.ExtendedDropImages
{
	public class ExtendedDropImagesInitializer
	{
		[VTSExtension_ExecuteAtApiStart]
		public static void Initialize()
		{
			VTSPluginExternals.LogMessage("This works");
		}
	}
}
