using HamstarHelpers.Classes.UI.Elements.Dialogs;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using ResetMode.NetProtocols;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public void ValidatePlayerOnHost( Player player ) {
			var mymod = ResetModeMod.Instance;
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "Validating "+player.name+"..." );
			}
			
			if( Main.netMode == 2 ) {
				PlayerResetProtocol.QuickRequest( player.whoAmI );
			} else if( Main.netMode == 0 ) {
				this.PromptReset( player );
			}
		}


		////////////////

		public void PromptReset( Player player ) {
			if( Main.netMode == 2 ) { throw new Exception( "Server cannot call this." ); }

			int playerWho = player.whoAmI;
			string text = "Play reset mode? Your character will be reset (except Progress Points)." +
				"\nNote: Playing this character on another world will force it to reset here.";

			Action<bool> confirmAction = ( confirm ) => {
				if( confirm ) {
					this.ResetPlayer( Main.player[playerWho] );
					this.IsPromptingForResetOnLocal = false;
				} else {
					this.Boot( player, "choose not to play" );
				}
			};

			////

			this.IsPromptingForResetOnLocal = true;

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirmAction );
			prompt.Open();
		}


		////////////////

		private void ResetPlayer( Player replayer ) {
			PlayerHelpers.FullVanillaReset( replayer );
			PlayerModHelpers.ModdedExtensionsReset( replayer, new HashSet<string>() { "Rewards" } );

			if( Main.netMode == 0 ) {
				this.BeginSessionForPlayer( replayer );
				this.RefundRewardsSpendings( replayer );
			} else if( Main.netMode == 1 ) {
				PlayerResetConfirmProtocol.QuickRequest();
			}
		}


		////////////////

		public void BeginSessionForPlayer( Player player ) {
			var mymod = ResetModeMod.Instance;
			mymod.Session.AddPlayer( player );

			if( Main.netMode == 2 ) {
				SessionProtocol.SyncToClient( player.whoAmI );
			}
		}

		////////////////

		public void RefundRewardsSpendings( Player player ) {
			var mymod = ResetModeMod.Instance;
			Mod rewardsMod = ModLoader.GetMod( "Rewards" );
			if( rewardsMod == null ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "No Rewards mod." );
				}
				return;
			}
			
			var uid = PlayerIdentityHelpers.GetUniqueId( player );

			if( mymod.Config.ResetRewardsSpendings ) {
				if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( uid ) ) {
					float ppSpent = mymod.Session.Data.PlayerPPSpendings[uid];

					rewardsMod.Call( "AddPoints", player, ppSpent );

					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Alert( player.name + "' PP spendings of " + ppSpent + " returned" );
					}
				} else {
					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Warn( player.name + "' PP could not be set" );
					}
				}
			}

			mymod.Session.Data.PlayerPPSpendings[uid] = 0;
			if( Main.netMode != 1 ) {
				mymod.Session.Save();
			}

			if( mymod.Config.ResetRewardsKills ) {
				rewardsMod.Call( "ResetKills", player );
			}
		}
	}
}
