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
				var hook = new CustomTimerAction( delegate () {
					if( Main.netMode == 1 ) { return; }
					this.ExpireCurrentWorldInSession( ResetModeMod.Instance );
				} );

				TimeLimitAPI.AddCustomAction( "reset", hook );

				this.Load( mymod );
				this.LoadRewards( mymod );
			} );


			Promises.AddPostWorldLoadEachPromise( delegate {
				var mymod = ResetModeMod.Instance;

				if( mymod.Config.AutoStartSession ) {
					if( Main.netMode == 0 || Main.netMode == 2 ) {
						this.StartSession( mymod );
					}
				}

				this.IsWorldInPlay = true;
			} );


			Promises.AddWorldUnloadEachPromise( () => {
				this.IsWorldInPlay = false;
			} );


			Promises.AddPostWorldUnloadEachPromise( () => {
				var mymod = ResetModeMod.Instance;

				this.IsExiting = false;
				
				if( mymod.CurrentNetMode == 0 || mymod.CurrentNetMode == 2 ) {
					if( mymod.Config.DeleteAllWorldsBetweenGames ) {
						if( this.Data.AwaitingNextWorld ) {
							this.ClearAllWorlds();
						}
					}

					this.Save( mymod );
				}

				Promises.TriggerCustomPromise( "ResetModeWorldExited" );
			} );
		}


		private void LoadRewards( ResetModeMod mymod ) {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );

			if( rewards_mod == null || rewards_mod.Version < new Version( 1, 5, 0 ) ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.LoadRewards - No Rewards mod found." );
				}
				return;
			}

			Action<Player, string, float, Item[]> func = ( plr, pack_name, rewards, items ) => {
				if( rewards == 0 ) { return; }

				var mymod2 = ResetModeMod.Instance;

				mymod2.Session.LogRewardsPPSpending( plr, rewards );

				if( mymod2.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.LoadRewards.Action - Refundable PP added for "+plr.name+": " + rewards );
				}
			};

			try {
				rewards_mod.Call( "OnPointsSpent", func );

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.LoadRewards - Success." );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "ResetMode.Logic.SessionLogic.LoadRewards - Could not hook Rewards: " + e.ToString() );
			}
		}
	}
}
