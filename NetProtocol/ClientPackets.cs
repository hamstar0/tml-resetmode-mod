using HamstarHelpers.DebugHelpers;
using ResetMode.Logic;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocol {
	static class ClientPackets {
		public static void HandlePacket( ResetModeMod mymod, BinaryReader reader ) {
			ResetModeProtocolTypes protocol = (ResetModeProtocolTypes)reader.ReadByte();
			
			switch( protocol ) {
			case ResetModeProtocolTypes.ModSettings:
				ClientPackets.ReceiveModSettings( mymod, reader );
				break;
			case ResetModeProtocolTypes.WorldStatus:
				ClientPackets.ReceiveWorldStatus( mymod, reader );
				break;
			default:
				LogHelpers.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}



		////////////////
		// Client Senders
		////////////////

		public static void RequestModSettings( ResetModeMod mymod ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.RequestModSettings );

			packet.Send();
		}




		////////////////
		// Client Receivers
		////////////////

		private static void ReceiveModSettings( ResetModeMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			mymod.JsonConfig.DeserializeMe( reader.ReadString() );

			var myplayer = Main.LocalPlayer.GetModPlayer<ResetModePlayer>();
			myplayer.FinishModSettingsSync();
		}

		private static void ReceiveWorldStatus( ResetModeMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			var myworld = mymod.GetModWorld<ResetModeWorld>();
			var status = (ResetModeStatus)reader.ReadInt32();

			myworld.Logic.WorldStatus = status;
		}
	}
}
