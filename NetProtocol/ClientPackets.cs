using HamstarHelpers.DebugHelpers;
using ResetMode.Logic;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocol {
	static class ClientPackets {
		public static void HandlePacket( ResetModeMod mymod, ResetModeProtocolTypes protocol, BinaryReader reader ) {
			switch( protocol ) {
			case ResetModeProtocolTypes.ModSettings:
				ClientPackets.ReceiveModSettings( mymod, reader );
				break;
			case ResetModeProtocolTypes.WorldData:
				ClientPackets.ReceiveWorldStatus( mymod, reader );
				break;
			case ResetModeProtocolTypes.PromptForReset:
				ClientPackets.ReceivePromptForReset( mymod, reader );
				break;
			case ResetModeProtocolTypes.PlayerSessionJoinAck:
				ClientPackets.ReceivePlayerSessionJoinAcknowledge( mymod, reader );
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

		public static void RequestWorldData( ResetModeMod mymod ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.RequestWorldData );

			packet.Send();
		}
		
		public static void RequestPlayerSessionJoinAcknowledge( ResetModeMod mymod ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.RequestPlayerSessionJoinAck );

			packet.Send();
		}




		////////////////
		// Client Receivers
		////////////////

		private static void ReceiveModSettings( ResetModeMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			mymod.ConfigJson.DeserializeMe( reader.ReadString() );

			var myplayer = Main.LocalPlayer.GetModPlayer<ResetModePlayer>();
			myplayer.FinishModSettingsSync();
		}

		private static void ReceiveWorldStatus( ResetModeMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			var myplayer = Main.LocalPlayer.GetModPlayer<ResetModePlayer>();
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			var status = (ResetModeStatus)reader.ReadInt32();

			myworld.Logic.WorldStatus = status;

			myplayer.FinishWorldDataSync();
		}
		
		private static void ReceivePromptForReset( ResetModeMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.PromptToBeginPlaying( mymod, player );
		}
		
		private static void ReceivePlayerSessionJoinAcknowledge( ResetModeMod mymod, BinaryReader reader ) {
			if( Main.netMode != 1 ) { throw new Exception( "Not client" ); }

			var myplayer = Main.LocalPlayer.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.FinishPlayerSessionJoin( mymod, Main.LocalPlayer );
		}
	}
}
