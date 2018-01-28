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


		////////////////

		public override void Initialize() {
			this.Logic = new PlayerLogic();
		}


		public override void Load( TagCompound tags ) {
			this.Logic.Load( this.player, tags );
		}

		public override TagCompound Save() {
			return this.Logic.Save( (ResetModeMod)this.mod, this.player );
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

			if( new_player ) {
				if( Main.netMode == 1 ) {
					SharedPackets.SendPlayerData( mymod, this.player );
					return;
				}
			}
		}


		public override void OnEnterWorld( Player player ) {
			var mymod = (ResetModeMod)this.mod;

			if( player.whoAmI == this.player.whoAmI ) {
				if( Main.netMode == 0 ) {	// Not server
					if( !mymod.ConfigJson.LoadFile() ) {
						mymod.ConfigJson.SaveFile();
						ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
					}
				}

				if( Main.netMode == 1 ) {
					ClientPackets.RequestModSettings( mymod );
					ClientPackets.RequestWorldData( mymod );
				}
				if( Main.netMode != 1 ) {	// NOT client
					this.HasSyncedModSettings = true;
					this.HasSyncedWorldData = true;
				}
			}
		}

		public bool IsSynced() {
			return this.HasSyncedModSettings && this.HasSyncedWorldData;
		}

		public void FinishModSettingsSync() {
			this.HasSyncedModSettings = true;
		}
		public void FinishWorldDataSync() {
			this.HasSyncedWorldData = true;
		}


		////////////////

		public override void PreUpdate() {
			if( this.player.whoAmI == Main.myPlayer ) {
				if( this.IsSynced() ) {
					var mymod = (ResetModeMod)this.mod;
					
					this.Logic.Update( mymod, this.player );
				}
			}
		}
	}
}
