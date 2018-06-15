using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.UIHelpers.Elements.Dialogs;
using HamstarHelpers.Utilities.Errors;
using HamstarHelpers.Utilities.Network;
using ResetMode.NetProtocols;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public static void ValidateAll( ResetModeMod mymod ) {
			for( int i=0; i<Main.player.Length; i++ ) {
				Player player = Main.player[i];
				if( player == null || !player.active ) { continue; }

				var myplayer = player.GetModPlayer<ResetModePlayer>();
				myplayer.Logic.ValidatePlayer( mymod, player );
			}
		}



		////////////////

		public void ValidatePlayer( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 1 ) { throw new Exception("Clients cannot call this."); }

			var myworld = mymod.GetModWorld<ResetModeWorld>();
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			
			if( !has_uid ) { throw new HamstarException( "ValidatePlayer - Player has no uid." ); }
			
			if( !myworld.Data.IsPlaying(mymod, player) ) {
				if( Main.netMode == 2 ) {
					PacketProtocol.QuickRequestToClient<PlayerResetProtocol>( player.whoAmI, -1 );
				} else if( Main.netMode == 0 ) {
					this.PromptReset( mymod, player );
				}
			}
		}


		////////////////

		public void PromptReset( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 2 ) { throw new Exception( "Server cannot call this." ); }
			
			this.IsPromptingForReset = true;

			int player_who = player.whoAmI;
			string text = "Play reset mode? Your character will be reset (except Progress Points)." +
				"\nNote: Playing this character on another world will force it to reset here.";

			Action confirm_action = delegate () {
				Player replayer = Main.player[ player_who ];

				PlayerHelpers.FullVanillaReset( replayer );

				if( Main.netMode == 1 ) {
					PacketProtocol.QuickRequestToServer<PlayerResetConfirmProtocol>();
				} else if( Main.netMode == 0 ) {
					this.BeginSession( mymod, replayer );
					this.RefundRewardsSpendings( mymod, replayer );
				}

				this.IsPromptingForReset = false;
			};
			Action cancel_action = delegate () {
				this.Boot( mymod, player, "choose not to play" );
			};

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirm_action, cancel_action );
			prompt.Open();
		}


		////////////////

		public void BeginSession( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			myworld.Data.AddPlayer( mymod, player );
		}


		public void RefundRewardsSpendings( ResetModeMod mymod, Player player ) {
			Mod rewards_mod = ModLoader.GetMod( "Rewards" );
			if( rewards_mod == null ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode - PlayerLogic.ResetRewards - No Rewards mod." );
				}
				return;
			}

			var myworld = mymod.GetModWorld<ResetModeWorld>();

			bool success;
			var pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "ResetMode - PlayerLogic.ResetRewards - Could not reset player Rewards; no UID for player "+player.name );
				return;
			}

			if( mymod.Config.ResetRewardsSpendings ) {
				if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( pid ) ) {
					float pp_spent = mymod.Session.Data.PlayerPPSpendings[pid];

					rewards_mod.Call( "AddPoints", player, pp_spent );

					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "ResetMode - PlayerLogic.ResetRewards - '" + player.name + "' PP spendings of " + pp_spent + " returned" );
					}
				} else {
					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "ResetMode - PlayerLogic.ResetRewards - '" + player.name + "' PP could not be set" );
					}
				}
			}

			mymod.Session.Data.PlayerPPSpendings[pid] = 0;

			if( mymod.Config.ResetRewardsKills ) {
				rewards_mod.Call( "ResetKills", player );
			}
		}
	}
}
