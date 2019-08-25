using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Helpers.TModLoader;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetConfirmProtocol : PacketProtocolRequestToServer {
		public static void QuickRequest() {
			PacketProtocolRequestToServer.QuickRequestToServer<PlayerResetConfirmProtocol>( -1 );
		}



		////////////////

		private PlayerResetConfirmProtocol() { }
		
		protected override void InitializeServerSendData( int toWho ) { }


		////////////////

		protected override bool ReceiveRequestWithServer( int fromWho ) {
			Player player = Main.player[ fromWho ];
			if( player == null ) {
				LogHelpers.Warn( "Invalid player (who:"+fromWho+")" );
				return true;
			}

			string uid = PlayerIdentityHelpers.GetUniqueId( player );
			if( uid == null ) {
				LogHelpers.Warn( "Missing player UID for player "+player.name+" ("+fromWho+")" );
				return true;
			}

			var mymod = (ResetModeMod)ResetModeMod.Instance;
			var myplayer = player.GetModPlayer<ResetModePlayer>();
			
			myplayer.Logic.BeginSessionForPlayer( player );
			myplayer.Logic.RefundRewardsSpendings( player );

			return false;
		}

		protected override void ReceiveReply() {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.LocalPlayer;
			var myplayer = (ResetModePlayer)TmlHelpers.SafelyGetModPlayer( player, mymod, "ResetModePlayer" );

			myplayer.Logic.BeginSessionForPlayer( player );
			myplayer.Logic.RefundRewardsSpendings( player );
		}
	}
}
