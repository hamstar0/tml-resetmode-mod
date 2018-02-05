﻿using ResetMode.Logic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModeWorld : ModWorld {
		public WorldLogic Logic { get; private set; }
		
		private bool IsworldStarted = false;


		////////////////

		public override void Initialize() {
			this.Logic = new WorldLogic();

			if( Main.netMode != 1 ) {
				var mymod = (ResetModeMod)this.mod;
				mymod.SessionJson.LoadFile();
			}
		}

		////////////////

		public override void Load( TagCompound tags ) {
			var mymod = (ResetModeMod)this.mod;

			this.Logic.Load( mymod, tags );

			if( !this.IsworldStarted ) {
				this.IsworldStarted = true;
				this.Logic.OnWorldStart( mymod );
			}
		}

		public override TagCompound Save() {
			return this.Logic.Save( (ResetModeMod)this.mod );
		}


		////////////////

		public override void PreUpdate() {
			var mymod = (ResetModeMod)this.mod;
			
			if( !this.IsworldStarted ) {
				this.IsworldStarted = true;
				this.Logic.OnWorldStart( mymod );
			}

			this.Logic.Update( mymod );
		}
	}
}
