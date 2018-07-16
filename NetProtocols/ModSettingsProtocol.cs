﻿using HamstarHelpers.Components.Network;
using HamstarHelpers.DebugHelpers;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class ModSettingsProtocol : PacketProtocol {
		public ResetModeConfigData Data;


		////////////////

		public override void SetServerDefaults() {
			this.Data = ResetModeMod.Instance.Config;
		}

		protected override void ReceiveWithClient() {
			var mymod = ResetModeMod.Instance;

			mymod.ConfigJson.SetData( this.Data );

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.NetProtocols.ModSettingsProtocol.ReceiveWithClient - " + this.Data.ToString() );
			}

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishModSettingsSync();
		}
	}
}
