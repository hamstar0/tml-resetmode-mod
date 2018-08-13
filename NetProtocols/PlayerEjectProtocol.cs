using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocols {
	class PlayerEjectProtocol : PacketProtocol {
		public static void Eject( Player player ) {
			var mymod = ResetModeMod.Instance;

			if( mymod.Session.HasWorldEverBeenPlayed( WorldHelpers.GetUniqueIdWithSeed() ) ) {
				ErrorLogger.Log( "Ejecting player " + Main.LocalPlayer.name + " via good exit..." );
				mymod.Session.GoodExit( mymod );
			} else {
				ErrorLogger.Log( "Ejecting player " + Main.LocalPlayer.name + " via bad exit..." );
				mymod.Session.BadExit( mymod );
			}
		}


		////////////////

		private PlayerEjectProtocol( PacketProtocolDataConstructorLock ctor_lock ) { }

		protected override void SetClientDefaults() { }


		////////////////

		protected override bool ReceiveRequestWithClient() {
			PlayerEjectProtocol.Eject( Main.LocalPlayer );
			return true;
		}
	}
}
