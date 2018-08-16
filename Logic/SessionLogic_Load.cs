using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Services.Promises;
using Newtonsoft.Json;
using System;
using Terraria;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void OnModLoad() {
LogHelpers.Log("3a");
			Promises.AddPostModLoadPromise( () => {
LogHelpers.Log(" Aa");
				var mymod = ResetModeMod.Instance;
				var hook = new CustomTimerAction( delegate () {
					if( Main.netMode == 1 ) { return; }
					this.ExpireCurrentWorldInSession( ResetModeMod.Instance );
				} );

				TimeLimitAPI.AddCustomAction( "reset", hook );

				this.Load( mymod );
				this.LoadRewards( mymod );
LogHelpers.Log(" Ab");
			} );


			Promises.AddPostWorldLoadEachPromise( delegate {
LogHelpers.Log(" Ba");
				var mymod = ResetModeMod.Instance;

				if( mymod.Config.AutoStartSession ) {
					if( Main.netMode == 0 || Main.netMode == 2 ) {
						this.StartSession( mymod );
					}
				}

				this.IsWorldInPlay = true;
LogHelpers.Log(" Bb");
			} );


			Promises.AddWorldUnloadEachPromise( () => {
LogHelpers.Log(" C "+JsonConvert.SerializeObject(this.Data));
				this.IsWorldInPlay = false;
			} );


			Promises.AddPostWorldUnloadEachPromise( () => {
LogHelpers.Log(" Da "+JsonConvert.SerializeObject(this.Data));
				var mymod = ResetModeMod.Instance;

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.OnModLoad (in promise) - Unloading world..." );
				}

				this.IsExiting = false;
				
				if( mymod.CurrentNetMode == 0 || mymod.CurrentNetMode == 2 ) {
					if( mymod.Config.DeleteAllWorldsBetweenGames ) {
						if( this.Data.AwaitingNextWorld ) {
							this.ClearAllWorlds();
						}
					}

					this.Save( mymod );
				}

				Promises.TriggerValidatedPromise( ResetModeMod.WorldExitValidator, ResetModeMod.MyValidatorKey );
LogHelpers.Log(" Db "+JsonConvert.SerializeObject(this.Data));
			} );
LogHelpers.Log("3b");
		}


		private void LoadRewards( ResetModeMod mymod ) {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );

			if( rewards_mod == null || rewards_mod.Version < new Version( 1, 5, 0 ) ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.LoadRewards - No Rewards mod found." );
				}
				return;
			}

			Action<Player, string, float, Item[]> func = ( plr, pack_name, rewards, items ) => {
				if( rewards == 0 ) { return; }

				var mymod2 = ResetModeMod.Instance;

				mymod2.Session.LogRewardsPPSpending( plr, rewards );

				if( mymod2.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.LoadRewards.Action - Refundable PP added for "+plr.name+": " + rewards );
				}
			};

			try {
				rewards_mod.Call( "OnPointsSpent", func );

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.LoadRewards - Success." );
				}
			} catch( Exception e ) {
				LogHelpers.Log( "!ResetMode.SessionLogic.LoadRewards - Could not hook Rewards: " + e.ToString() );
			}
		}
	}
}
