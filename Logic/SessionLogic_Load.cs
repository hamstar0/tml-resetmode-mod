using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Services.Promises;
using System;
using Terraria;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void OnModLoad() {
			Promises.AddPostModLoadPromise( () => {
				var mymod = ResetModeMod.Instance;

				this.Load( ResetModeMod.Instance );

				var hook = new CustomTimerAction( delegate () {
					if( Main.netMode == 1 ) { return; }

					this.ExpireCurrentWorldInSession( mymod );
				} );

				TimeLimitAPI.AddCustomAction( "reset", hook );

				this.LoadRewards( mymod );
			} );

			Promises.AddWorldLoadEachPromise( delegate {
				var mymod = ResetModeMod.Instance;

				if( mymod.Config.AutoStartSession ) {
					if( Main.netMode == 0 || Main.netMode == 2 ) {
						this.StartSession( mymod );
					}
				}
			} );

			Promises.AddPostWorldUnloadEachPromise( () => {
				var mymod = ResetModeMod.Instance;

				this.IsExiting = false;

				if( mymod.Config.DeleteAllWorldsBetweenGames ) {
					if( this.SessionData.AwaitingNextWorld ) {
						if( mymod.CurrentNetMode == 0 || mymod.CurrentNetMode == 2 ) {
							this.ClearAllWorlds();
						}
					}
				}
				if( mymod.CurrentNetMode == 0 || mymod.CurrentNetMode == 2 ) {
					this.Save( mymod );
				}
			} );
		}


		private void LoadRewards( ResetModeMod mymod ) {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );

			if( rewards_mod == null || rewards_mod.Version < new Version( 1, 5, 0 ) ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Mod.LoadRewards - No Rewards mod found." );
				}
				return;
			}

			Action<Player, string, float, Item[]> func = ( plr, pack_name, rewards, items ) => {
				if( rewards == 0 ) { return; }

				var mymod2 = ResetModeMod.Instance;

				mymod2.Session.LogRewardsPPSpending( plr, rewards );

				if( mymod2.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Mod.LoadRewards - Refundable PP added: " + rewards );
				}
			};

			try {
				rewards_mod.Call( "OnPointsSpent", func );

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Mod.LoadRewards - Success." );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "ResetMode.Mod.LoadRewards - Could not hook Rewards: " + e.ToString() );
			}
		}
	}
}
