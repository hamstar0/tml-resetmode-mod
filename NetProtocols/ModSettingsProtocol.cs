using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class ModSettingsProtocol : PacketProtocolRequestToServer {
		public ResetModeConfigData Data;


		////////////////

		protected ModSettingsProtocol( PacketProtocolDataConstructorLock ctor_lock ) : base( ctor_lock ) { }

		protected override void InitializeServerSendData( int to_who ) {
			this.Data = ResetModeMod.Instance.Config;
		}

		////////////////

		protected override void ReceiveReply() {
			var mymod = ResetModeMod.Instance;

			mymod.ConfigJson.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishModSettingsSync();
		}
	}
}
