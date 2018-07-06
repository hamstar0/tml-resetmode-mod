using HamstarHelpers.DebugHelpers;
using ResetMode.Data;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModeWorld : ModWorld {
		public ResetModeWorldData Data { get; private set; }


		////////////////

		public override void Initialize() {
			this.Data = new ResetModeWorldData();
		}

		////////////////

		public override void Load( TagCompound tags ) {
			var mymod = (ResetModeMod)this.mod;
			
			this.Data.Load( mymod, tags );
		}

		public override TagCompound Save() {
			return this.Data.Save( (ResetModeMod)this.mod );
		}
	}
}
