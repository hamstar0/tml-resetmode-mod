using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Services.Hooks.LoadHooks;
using System;
using Terraria;
using Terraria.ModLoader;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void OnModLoad() {
			LoadHooks.AddPostModLoadHook( () => {
				var hook = new CustomTimerAction( delegate () {
					if( Main.netMode == 1 ) { return; }
					this.ExpireCurrentWorldInSession( ResetModeMod.Instance );
				} );

				TimeLimitAPI.AddCustomAction( "reset", hook );

				this.Load();
				this.LoadRewards();
			} );


			LoadHooks.AddPostWorldLoadEachHook( delegate {
				var mymod = ResetModeMod.Instance;

				if( mymod.Config.AutoStartSession ) {
					if( Main.netMode == 0 || Main.netMode == 2 ) {
						this.StartSession();
					}
				}

				this.IsWorldInPlay = true;
			} );


			LoadHooks.AddWorldUnloadEachHook( () => {
				this.IsWorldInPlay = false;
			} );


			LoadHooks.AddPostWorldUnloadEachHook( () => {
				var mymod = ResetModeMod.Instance;

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "(In promise) - Unloading world..." );
				}

				this.IsExiting = false;
				
				if( mymod.CurrentNetMode == 0 || mymod.CurrentNetMode == 2 ) {
					if( mymod.Config.DeleteAllWorldsBetweenGames ) {
						if( this.Data.AwaitingNextWorld ) {
							this.ClearAllWorlds();
						}
					}

					this.Save();
				}

				CustomLoadHooks.TriggerHook( ResetModeMod.WorldExitValidator, ResetModeMod.MyValidatorKey );
			} );
		}


		private void LoadRewards() {
			var mymod = ResetModeMod.Instance;
			Mod rewardsMod = ModLoader.GetMod( "Rewards" );

			if( rewardsMod == null || rewardsMod.Version < new Version( 1, 5, 0 ) ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "No Rewards mod found." );
				}
				return;
			}

			Action<Player, string, float, Item[]> func = ( plr, packName, rewards, items ) => {
				if( rewards == 0 ) { return; }

				var mymod2 = ResetModeMod.Instance;

				mymod2.Session.LogRewardsPPSpending( plr, rewards );

				if( mymod2.Config.DebugModeInfo ) {
					LogHelpers.Alert( "Refundable PP added for "+plr.name+": " + rewards );
				}
			};

			try {
				rewardsMod.Call( "OnPointsSpent", func );

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "Success." );
				}
			} catch( Exception e ) {
				LogHelpers.Warn( "Could not hook Rewards: " + e.ToString() );
			}
		}
	}
}
