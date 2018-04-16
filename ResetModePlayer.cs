using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using ResetMode.Logic;
using ResetMode.NetProtocols;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	class ResetModePlayer : ModPlayer {
		public PlayerLogic Logic;

		private bool HasSyncedModSettings = false;



		////////////////

		public override void Initialize() {
			this.Logic = new PlayerLogic();
		}

		////////////////

		public override void clientClone( ModPlayer client_clone ) {
			var clone = (ResetModePlayer)client_clone;
			clone.HasSyncedModSettings = this.HasSyncedModSettings;
		}

		public override void SyncPlayer( int to_who, int from_who, bool new_player ) {
			var mymod = (ResetModeMod)this.mod;

			if( Main.netMode == 1 ) {
				if( new_player ) {
					this.OnClientConnect();
					return;
				}
			} else {
				if( to_who == -1 && from_who == this.player.whoAmI ) {
					this.OnServerConnect();
				}
			}
		}

		private void OnClientConnect() { }

		private void OnServerConnect() {
			var mymod = (ResetModeMod)this.mod;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Session.IsRunning ) {
				if( myworld.Logic.WorldStatus != ResetModeStatus.Normal ) {
					this.Logic.ValidatePlayer( mymod, this.player );
				}
			}
		}


		////////////////

		public override void OnEnterWorld( Player player ) {
			var mymod = (ResetModeMod)this.mod;

			if( player.whoAmI == this.player.whoAmI ) {
				if( Main.netMode == 0 ) {
					if( !mymod.ConfigJson.LoadFile() ) {
						mymod.ConfigJson.SaveFile();
						ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
					}
				}

				if( Main.netMode == 1 ) {
					PacketProtocol.QuickSendRequest<ResetModeModSettingsProtocol>( -1, -1 );
				}
				if( Main.netMode != 1 ) {	// NOT client
					this.FinishModSettingsSync();
				}
				if( Main.netMode == 0 ) {
					this.OnClientConnect();
					this.OnServerConnect();
				}
			}
		}

		////////////////

		public bool IsSynced() {
			return this.HasSyncedModSettings;
		}

		public void FinishModSettingsSync() {
			this.HasSyncedModSettings = true;

			if( this.IsSynced() ) {
				this.Logic.OnEnterWorld( (ResetModeMod)this.mod, this.player );
			}
		}


		////////////////

		public override void PreUpdate() {
			var mymod = (ResetModeMod)this.mod;
			bool is_me = this.player.whoAmI == Main.myPlayer;

			if( is_me && this.IsSynced() ) {
				this.Logic.Update( mymod, this.player );
			}
		}
	}
}
