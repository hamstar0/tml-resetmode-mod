using HamstarHelpers.DebugHelpers;
using HamstarHelpers.TmlHelpers;
using ResetMode.Data;
using ResetMode.Logic;
using System;
using Terraria;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode {
	partial class ResetModeMod : Mod {
		private void LoadConfigs() {
			if( !this.ConfigJson.LoadFile() ) {
				this.ConfigJson.SaveFile();
			}

			if( this.Config.UpdateToLatestVersion() ) {
				ErrorLogger.Log( "Reset Mode updated to " + ResetModeConfigData.ConfigVersion.ToString() );
				this.ConfigJson.SaveFile();
			}
		}


		private void LoadStages() {
			TmlLoadHelpers.AddPostModLoadPromise( () => {
				this.Session.Load( this );

				var hook = new CustomTimerAction( delegate () {
					if( Main.netMode == 1 ) { return; }
					
					this.Session.ExpireWorldInSession( ResetModeMod.Instance );
				} );

				TimeLimitAPI.AddCustomAction( "reset", hook );

				this.LoadRewards();
			} );

			TmlLoadHelpers.AddWorldLoadEachPromise( delegate {
				this.CurrentNetMode = Main.netMode;

				if( this.Config.AutoStartSession ) {
					if( Main.netMode == 0 || Main.netMode == 2 ) {
						this.Session.StartSession( this );
					}
				}
			} );
			
			TmlLoadHelpers.AddPostWorldUnloadEachPromise( () => {
				if( this.Config.DeleteAllWorldsBetweenGames ) {
					if( this.Session.Data.AwaitingNextWorld ) {
						if( this.CurrentNetMode == 0 || this.CurrentNetMode == 2 ) {
							SessionLogic.ClearAllWorlds();
						}
					}
				}
			} );
		}


		private void LoadRewards() {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );
			if( rewards_mod == null || rewards_mod.Version < new Version( 1, 5, 0 ) ) {
				if( this.Config.DebugModeInfo ) {
					LogHelpers.Log( "Reset Mode - Mod.LoadRewards - No Rewards mod found." );
				}
				return;
			}
			
			Action<Player, string, float, Item[]> func = ( plr, pack_name, rewards, items ) => {
				if( rewards == 0 ) { return; }

				var mymod = ResetModeMod.Instance;

				mymod.Session.LogRewardsPPSpending( plr, rewards );

				if( this.Config.DebugModeInfo ) {
					LogHelpers.Log( "Reset Mode - Mod.LoadRewards - Refundable PP added: "+rewards );
				}
			};

			try {
				rewards_mod.Call( "OnPointsSpent", func );

				if( this.Config.DebugModeInfo ) {
					LogHelpers.Log( "Reset Mode - Mod.LoadRewards - Success." );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "Reset Mode - Mod.LoadRewards - Could not hook Rewards: " + e.ToString() );
			}
		}
	}
}
