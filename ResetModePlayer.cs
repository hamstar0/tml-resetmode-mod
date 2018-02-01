using ResetMode.Data;
using ResetMode.Logic;
using ResetMode.NetProtocol;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;


namespace ResetMode {
	class ResetModePlayer : ModPlayer {
		public PlayerLogic Logic;

		private bool HasSyncedModSettings = false;
		private bool HasSyncedWorldData = false;
		private bool HasSyncedSessionData = false;


		////////////////

		public override void Initialize() {
			this.Logic = new PlayerLogic();
		}


		public override void Load( TagCompound tags ) {
			this.Logic.LoadLocal( (ResetModeMod)this.mod, this.player, tags );
		}

		public override TagCompound Save() {
			return this.Logic.SaveLocal( (ResetModeMod)this.mod, this.player );
		}


		public void NetSend( BinaryWriter writer ) {
			this.Logic.NetSend( (ResetModeMod)this.mod, this.player, writer );
		}

		public void NetReceive( BinaryReader reader ) {
			this.Logic.NetReceive( (ResetModeMod)this.mod, this.player, reader );
		}


		////////////////

		public override void clientClone( ModPlayer client_clone ) {
			var clone = (ResetModePlayer)client_clone;
			clone.Logic = this.Logic;
			clone.HasSyncedModSettings = this.HasSyncedModSettings;
			clone.HasSyncedWorldData = this.HasSyncedWorldData;
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

		private void OnClientConnect() {
			var mymod = (ResetModeMod)this.mod;

			mymod.Logic.OnEnterGame();
			mymod.Session.EndSession();
		}

		private void OnServerConnect() { }


		////////////////

		public override void OnEnterWorld( Player player ) {
			var mymod = (ResetModeMod)this.mod;

			if( player.whoAmI == this.player.whoAmI ) {
				if( Main.netMode == 0 ) {
					if( !mymod.ConfigJson.LoadFile() ) {
						mymod.ConfigJson.SaveFile();
						ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
					}

					mymod.Logic.OnEnterGame();
					mymod.Logic.LoadPlayerData( mymod, player );
				}

				if( mymod.Logic.NetMode == 1 ) {
					this.Logic.SyncClient( mymod, this.player );
					ClientPackets.RequestModSettings( mymod );
					ClientPackets.RequestWorldData( mymod );
					ClientPackets.RequestSessionData( mymod );
				}
				if( mymod.Logic.NetMode != 1 ) {	// NOT client
					this.HasSyncedModSettings = true;
					this.HasSyncedWorldData = true;
					this.HasSyncedSessionData = true;
				}
			}
		}

		////////////////

		public bool IsSynced() {
			return this.HasSyncedModSettings && this.HasSyncedWorldData && this.HasSyncedSessionData;
		}

		public void FinishModSettingsSync() {
			this.HasSyncedModSettings = true;
		}
		public void FinishWorldDataSync() {
			this.HasSyncedWorldData = true;
		}
		public void FinishSessionDataSync() {
			this.HasSyncedSessionData = true;
		}


		////////////////

		public override void PreUpdate() {
			var mymod = (ResetModeMod)this.mod;
			bool is_me = this.player.whoAmI == Main.myPlayer;

			if( (is_me && this.IsSynced()) || mymod.Logic.NetMode == 2 ) {
				this.Logic.Update( (ResetModeMod)this.mod, this.player );
			}
		}
	}
}
