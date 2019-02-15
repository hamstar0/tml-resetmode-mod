using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using ResetMode.Logic;
using Terraria;
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

			if( Main.netMode == 0 ) {
				if( !mymod.ConfigJson.LoadFile() ) {
					mymod.ConfigJson.SaveFile();
					LogHelpers.Alert( "Reset Mode config "+mymod.Version.ToString()+" created." );
				}
			}

			if( mymod.Config.DebugModeInfo ) {
				string uid = PlayerIdentityHelpers.GetProperUniqueId( player );

				LogHelpers.Alert( player.name+" joined ("+uid+")" );
			}
			
			if( Main.netMode == 0 ) {
				this.OnSingleEnterWorld();
			}
			if( Main.netMode == 1 ) {
				this.OnCurrentClientEnterWorld();
			}
		}


		////////////////

		public override void PreUpdate() {
			if( Main.netMode != 2 ) {
				if( this.player.whoAmI != Main.myPlayer ) { return; }
			}

			var mymod = (ResetModeMod)this.mod;

			if( this.IsSynced ) {
				if( Main.netMode == 0 ) {
					this.Logic.PreUpdateSyncedSingle();
				} else if( Main.netMode == 1 ) {
					this.Logic.PreUpdateSyncedCurrentClient();
				} else {
					this.Logic.PreUpdateSyncedServerForPlayer( this.player );
				}
			} else {
				if( Main.netMode != 2 ) {
					this.Logic.PreUpdateUnsyncedLocal();
				}
			}
		}
	}
}
