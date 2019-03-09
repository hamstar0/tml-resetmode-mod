using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using HamstarHelpers.Services.Timers;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocols {
	class PlayerEjectProtocol : PacketProtocolRequestToClient {
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
