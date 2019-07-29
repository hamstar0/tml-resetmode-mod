using HamstarHelpers.Components.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.World;
using HamstarHelpers.Services.Timers;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerEjectProtocol : PacketProtocolRequestToClient {
		public static void QuickRequest( int targetClientWho ) {
			PacketProtocolRequestToClient.QuickRequest<PlayerEjectProtocol>( targetClientWho, -1, -1 );
		}


		////

		public static void Eject( Player player ) {
			var mymod = ResetModeMod.Instance;

			if( mymod.Session.HasWorldEverBeenPlayed( WorldHelpers.GetUniqueIdForCurrentWorld(true) ) ) {
				LogHelpers.Log( "Ejecting player " + Main.LocalPlayer.name + " via good exit..." );
				mymod.Session.GoodExit();
			} else {
				LogHelpers.Log( "Ejecting player " + Main.LocalPlayer.name + " via bad exit..." );
				mymod.Session.BadExit();
			}
		}


		////////////////

		private PlayerEjectProtocol() { }

		protected override void InitializeClientSendData() { }


		////////////////

		protected override bool ReceiveRequestWithClient() {
			Timers.SetTimer( "PlayerEjectProtocolDelay", 3, () => {
				PlayerEjectProtocol.Eject( Main.LocalPlayer );
				return false;
			} );
			return false;
		}

		protected override void ReceiveReply( int fromWho ) {
			//PlayerEjectProtocol.Eject( Main.player[fromWho] );
		}
	}
}
