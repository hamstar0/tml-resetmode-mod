using ResetMode.Logic;
using ResetMode.NetProtocol;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModePlayer : ModPlayer {
		private PlayerLogic Logic;

		private bool HasSyncedModSettings = false;
		private bool HasShownWelcome = false;


		////////////////

		public override void Initialize() {
			this.Logic = new PlayerLogic();
		}


		public override void Load( TagCompound tags ) {
			this.Logic.Load( this.player, tags );
		}

		public override TagCompound Save() {
			return this.Logic.Save( this.player );
		}


		////////////////

		public override void clientClone( ModPlayer client_clone ) {
			var clone = (ResetModePlayer)client_clone;
			clone.Logic = this.Logic;
			clone.HasSyncedModSettings = this.HasSyncedModSettings;
			clone.HasShownWelcome = this.HasShownWelcome;
		}


		public override void OnEnterWorld( Player player ) {
			var mymod = (ResetModeMod)this.mod;

			if( player.whoAmI == this.player.whoAmI ) {
				if( Main.netMode == 0 ) {   // Not server
					if( !mymod.JsonConfig.LoadFile() ) {
						mymod.JsonConfig.SaveFile();
						ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
					}
				}

				if( Main.netMode == 1 ) {
					ClientPackets.RequestModSettings( mymod );
				}
				if( Main.netMode != 1 ) {   // NOT client
					this.HasSyncedModSettings = true;
				}
			}
		}

		public void FinishModSettingsSync() {
			this.HasSyncedModSettings = true;
		}


		////////////////

		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				var mymod = (ResetModeMod)this.mod;

				if( this.HasSyncedModSettings && !this.HasShownWelcome ) {
					this.HasShownWelcome = true;
					this.Logic.Welcome( mymod, this.player );
				}

				this.Logic.Update( mymod, this.player );
			}
		}
	}
}
