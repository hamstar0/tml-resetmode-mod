using ResetMode.Logic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModeWorld : ModWorld {
		public WorldLogic Logic { get; private set; }

		private bool IsLoaded = false;


		////////////////

		public override void Initialize() {
			this.Logic = new WorldLogic();
			this.IsLoaded = false;
		}

		////////////////

		public override void Load( TagCompound tags ) {
			this.Logic.Load( (ResetModeMod)this.mod, tags );
			this.IsLoaded = true;
		}

		public override TagCompound Save() {
			return this.Logic.Save();
		}


		////////////////

		public override void PreUpdate() {
			if( this.IsLoaded ) {
				this.Logic.Update( (ResetModeMod)this.mod );
			}
		}
	}
}
