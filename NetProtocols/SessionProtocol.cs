using HamstarHelpers.Components.Network;
using HamstarHelpers.DebugHelpers;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class SessionProtocol : PacketProtocol {
		public ResetModeSessionData Data;


		////////////////

		public override void SetServerDefaults() {
			var mymod = ResetModeMod.Instance;

			this.Data = mymod.Session.Data;
		}

		////////////////

		protected override void ReceiveWithClient() {
			var mymod = ResetModeMod.Instance;

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.NetProtocols.SessionProtocol.ReceiveWithClient - "+this.Data.ToString() );
			}

			mymod.Session.SetData( mymod, this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishSessionSync();
		}
	}
}
