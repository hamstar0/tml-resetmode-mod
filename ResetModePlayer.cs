using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Services.Timers;
using Microsoft.Xna.Framework;
using ResetMode.Logic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		public PlayerLogic Logic;

		public bool HasModSettings { get; private set; }
		public bool HasSessionData { get; private set; }
		internal bool IsSynced = false;



		////////////////

		public override bool CloneNewInstances => false;

		public override void Initialize() {
			this.Logic = new PlayerLogic();
			this.HasModSettings = false;
			this.HasSessionData = false;
			this.IsSynced = false;
		}

		public override void clientClone( ModPlayer clientClone ) {
			var clone = (ResetModePlayer)clientClone;
			clone.HasModSettings = this.HasModSettings;
			clone.HasSessionData = this.HasSessionData;
			clone.IsSynced = this.IsSynced;
		}


		////////////////

		public override void SyncPlayer( int toWho, int fromWho, bool newPlayer ) {
			var mymod = (ResetModeMod)this.mod;

			if( Main.netMode == 2 ) {
				if( toWho == -1 && fromWho == this.player.whoAmI ) {
					this.OnServerConnect( Main.player[fromWho] );
				}
			}
		}

		public override void OnEnterWorld( Player player ) {
			if( player.whoAmI != Main.myPlayer ) { return; }
			if( this.player.whoAmI != Main.myPlayer ) { return; }

			var mymod = (ResetModeMod)this.mod;

			if( mymod.Config.DebugModeInfo ) {
				string uid = PlayerIdentityHelpers.GetUniqueId( player );

				LogHelpers.Alert( player.name + " joined (" + uid + ")" );
			}

			if( Main.netMode == 0 ) {
				this.OnSingleEnterWorld();
			}
			if( Main.netMode == 1 ) {
				this.OnCurrentClientEnterWorld();
			}

			Timers.SetTimer( "ResetModeDebugSync", 10 * 60, () => {
				if( !this.HasModSettings ) {
					Main.NewText( "Failed to sync mod settings", Color.Red );
					LogHelpers.Warn( "Failed to sync mod settings (so far?)" );
				}
				if( !this.HasSessionData ) {
					Main.NewText( "Failed to sync session data", Color.Red );
					LogHelpers.Warn( "Failed to sync session data (so far?)" );
				}
				if( !this.IsSynced ) {
					Main.NewText( "Failed to sync", Color.Red );
					LogHelpers.Warn( "Failed to sync (so far?)" );
				}
				return false;
			} );
		}


		////////////////

		public override void PreUpdate() {
			switch( Main.netMode ) {
			case NetmodeID.SinglePlayer:
				this.PreUpdateSingle();
				break;
			case NetmodeID.MultiplayerClient:
				this.PreUpdateClient();
				break;
			case NetmodeID.Server:
				this.PreUpdateServer();
				break;
			}
		}

		////

		private void PreUpdateSingle() {
			var mymod = (ResetModeMod)this.mod;

			if( this.IsSynced ) {
				this.Logic.PreUpdateSyncedSingle();
			} else {
				this.Logic.PreUpdateUnsyncedLocal();
			}
		}

		private void PreUpdateClient() {
			if( this.player.whoAmI != Main.myPlayer ) { return; }

			var mymod = (ResetModeMod)this.mod;

			if( this.IsSynced ) {
				this.Logic.PreUpdateSyncedCurrentClient();
			} else {
				this.Logic.PreUpdateUnsyncedLocal();
			}
		}

		private void PreUpdateServer() {
			var mymod = (ResetModeMod)this.mod;

			if( this.IsSynced ) {
				this.Logic.PreUpdateSyncedServerForPlayer( this.player );
			}
		}
	}
}
