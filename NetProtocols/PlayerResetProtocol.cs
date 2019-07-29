using HamstarHelpers.Components.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.TModLoader;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetProtocol : PacketProtocolRequestToClient {
		public static void QuickRequest( int clientWho ) {
			PacketProtocolRequestToClient.QuickRequest<PlayerResetProtocol>( clientWho, -1, -1 );
		}



		////////////////

		private PlayerResetProtocol() { }

		protected override void InitializeClientSendData() { }

		////////////////

		protected override bool ReceiveRequestWithClient() {
			Player player = Main.LocalPlayer;
			var myplayer = (ResetModePlayer)TmlHelpers.SafelyGetModPlayer( player, ResetModeMod.Instance, "ResetModePlayer" );

			myplayer.Logic.PromptReset( player );

			return false;
		}

		////

		protected override void ReceiveReply( int fromWho ) { }
	}
}
