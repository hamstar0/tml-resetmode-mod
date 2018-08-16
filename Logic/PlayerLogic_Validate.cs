using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.UI.Elements.Dialogs;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Services.Promises;
using ResetMode.NetProtocols;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public void ValidatePlayer( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 1 ) { throw new Exception("Clients cannot call this."); }

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.PlayerLogic.ValidatePlayer - Validating "+player.name+"..." );
			}
			
			if( Main.netMode == 2 ) {
				PacketProtocol.QuickRequestToClient<PlayerResetProtocol>( player.whoAmI, -1 );
			} else if( Main.netMode == 0 ) {
				this.PromptReset( mymod, player );
			}
		}


		////////////////

		public void PromptReset( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 2 ) { throw new Exception( "Server cannot call this." ); }

			int player_who = player.whoAmI;
			string text = "Play reset mode? Your character will be reset (except Progress Points)." +
				"\nNote: Playing this character on another world will force it to reset here.";

			Action confirm_action = delegate () {
				Player replayer = Main.player[ player_who ];

				PlayerHelpers.FullVanillaReset( replayer );
				PlayerModHelpers.ModdedExtensionsReset( replayer );

				if( Main.netMode == 0 ) {
					this.BeginSessionForPlayer( mymod, replayer );
					this.RefundRewardsSpendings( mymod, replayer );
				} else if( Main.netMode == 1 ) {
					PacketProtocol.QuickRequestToServer<PlayerResetConfirmProtocol>();
				}

				this.IsPromptingForReset = false;
			};
			Action cancel_action = delegate () {
				this.Boot( mymod, player, "choose not to play" );
			};

			////

			this.IsPromptingForReset = true;

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirm_action, cancel_action );
			prompt.Open();
		}


		////////////////

		public void BeginSessionForPlayer( ResetModeMod mymod, Player player ) {
			mymod.Session.AddPlayer( mymod, player );

			if( Main.netMode == 2 ) {
				int who = player.whoAmI;

				Promises.AddWorldInPlayOncePromise( () => {
					PacketProtocol.QuickSendToClient<SessionProtocol>( who, -1 );
				} );
			}
		}

		////////////////

		public void RefundRewardsSpendings( ResetModeMod mymod, Player player ) {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );
			if( rewards_mod == null ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.PlayerLogic.RefundRewardsSpendings - No Rewards mod." );
				}
				return;
			}
			
			bool success;
			var pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "!ResetMode.PlayerLogic.RefundRewardsSpendings - Could not reset player Rewards; no UID for player " + player.name );
				return;
			}

			if( mymod.Config.ResetRewardsSpendings ) {
				if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( pid ) ) {
					float pp_spent = mymod.Session.Data.PlayerPPSpendings[pid];

					rewards_mod.Call( "AddPoints", player, pp_spent );

					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "ResetMode.PlayerLogic.RefundRewardsSpendings - '" + player.name + "' PP spendings of " + pp_spent + " returned" );
					}
				} else {
					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "!ResetMode.PlayerLogic.RefundRewardsSpendings - '" + player.name + "' PP could not be set" );
					}
				}
			}

			mymod.Session.Data.PlayerPPSpendings[pid] = 0;
			if( Main.netMode != 1 ) {
				mymod.Session.Save( mymod );
			}

			if( mymod.Config.ResetRewardsKills ) {
				rewards_mod.Call( "ResetKills", player );
			}
		}
	}
}
