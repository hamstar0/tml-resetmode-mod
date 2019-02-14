using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModeWorld : ModWorld {
		public override TagCompound Save() {
			var mymod = ResetModeMod.Instance;
			mymod.Session.Save();

			return base.Save();
		}
	}
}
