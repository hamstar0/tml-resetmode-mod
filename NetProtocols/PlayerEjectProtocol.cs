using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocols {
	class PlayerEjectProtocol : PacketProtocolRequestToClient { //PacketProtocolRequestToClient {	TODO
		public static void Eject( Player player ) {
			var mymod = ResetModeMod.Instance;

			if( mymod.Session.HasWorldEverBeenPlayed( WorldHelpers.GetUniqueId(true) ) ) {
				ErrorLogger.Log( "Ejecting player " + Main.LocalPlayer.name + " via good exit..." );
				mymod.Session.GoodExit();
			} else {
				ErrorLogger.Log( "Ejecting player " + Main.LocalPlayer.name + " via bad exit..." );
				mymod.Session.BadExit();
			}
		}


		////////////////

		private PlayerEjectProtocol() { }

		protected override void InitializeClientSendData() { }
		

		////////////////

		//protected override bool ReceiveRequestWithClient() {
		//	PlayerEjectProtocol.Eject( Main.LocalPlayer );
		//	return true;
		//}

		protected override void ReceiveReply( int fromWho ) {
			PlayerEjectProtocol.Eject( Main.player[fromWho] );
		}
	}
}
