﻿using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Services.Promises;
using ResetMode.Data;
using ResetMode.Logic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		public PlayerLogic Logic;
		
		public bool HasModSettings { get; private set; }
		public bool HasSessionData { get; private set; }
		private bool HasEnteredWorld = false;



		////////////////

		public override bool CloneNewInstances { get { return false; } }

		public override void Initialize() {
			this.Logic = new PlayerLogic();
			this.HasModSettings = false;
			this.HasSessionData = false;
			this.HasEnteredWorld = false;
		}

		public override void clientClone( ModPlayer client_clone ) {
			var clone = (ResetModePlayer)client_clone;
			clone.HasModSettings = this.HasModSettings;
			clone.HasSessionData = this.HasSessionData;
			clone.HasEnteredWorld = this.HasEnteredWorld;
		}


		////////////////

		public override void SyncPlayer( int to_who, int from_who, bool new_player ) {
			var mymod = (ResetModeMod)this.mod;

			if( Main.netMode == 2 ) {
				if( to_who == -1 && from_who == this.player.whoAmI ) {
					Promises.AddSafeWorldLoadOncePromise( () => {
						this.OnServerConnect( Main.player[from_who] );
					} );
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
					ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
				}
			}

			if( mymod.Config.DebugModeInfo ) {
				bool success;
				string uid = PlayerIdentityHelpers.GetUniqueId( player, out success );

				ErrorLogger.Log( "ResetMode.ResetModePlayer.OnEnterWorld - " + player.name + " joined ("+uid+"; "+success+")" );
			}

			if( Main.netMode == 0 ) {
				Promises.AddCurrentPlayerLoadOncePromise( () => {
					this.OnSingleConnect();
				} );
			}
			if( Main.netMode == 1 ) {
				Promises.AddCurrentPlayerLoadOncePromise( () => {
					this.OnCurrentClientConnect();
				} );
			}
		}


		////////////////

		public override void PreUpdate() {
			if( Main.netMode != 2 ) {
				if( this.player.whoAmI != Main.myPlayer ) { return; }
			}

			var mymod = (ResetModeMod)this.mod;

			if( this.IsSynced() ) {
				if( Main.netMode == 0 ) {
					this.Logic.PreUpdateSyncedSingle( mymod );
				} else if( Main.netMode == 1 ) {
					this.Logic.PreUpdateSyncedClient( mymod, this.player );
				} else {
					this.Logic.PreUpdateSyncedServer( mymod, this.player );
				}
			} else {
				if( Main.netMode != 2 ) {
					this.Logic.PreUpdateUnsyncedLocal( mymod, this.player );
				}
			}
		}
	}
}
