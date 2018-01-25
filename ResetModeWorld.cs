using System.IO;
using ResetMode.Logic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModeWorld : ModWorld {
		public WorldLogic Logic;


		////////////////

		public override void Initialize() {
			this.Logic = new WorldLogic();
		}

		////////////////

		public override void Load( TagCompound tags ) {
			this.Logic.Load( (ResetModeMod)this.mod, tags );
		}

		public override TagCompound Save() {
			return this.Logic.Save();
		}

		////////////////

		public override void NetReceive( BinaryReader reader ) {
			this.Logic.NetReceive( reader );
		}

		public override void NetSend( BinaryWriter writer ) {
			this.Logic.NetSend( writer );
		}
	}
}
