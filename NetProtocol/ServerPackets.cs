using HamstarHelpers.DebugHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocol {
	static class ServerPackets {
		public static void HandlePacket( ResetModeMod mymod, ResetModeProtocolTypes protocol, BinaryReader reader, int player_who ) {
			switch( protocol ) {
			case ResetModeProtocolTypes.RequestModSettings:
				ServerPackets.ReceiveModSettingsRequest( mymod, reader, player_who );
				break;
			case ResetModeProtocolTypes.RequestWorldData:
				ServerPackets.ReceiveWorldDataRequest( mymod, reader, player_who );
				break;
			case ResetModeProtocolTypes.RequestPlayerSessionJoinAck:
				ServerPackets.ReceivePlayerSessionJoinAcknowledgeRequest( mymod, reader, player_who );
				break;
			default:
				LogHelpers.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}


		
		////////////////
		// Senders
		////////////////

		public static void SendModSettings( ResetModeMod mymod, int to_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.ModSettings );
			packet.Write( (string)mymod.ConfigJson.SerializeMe() );

			packet.Send( to_who );
		}

		public static void SendWorldData( ResetModeMod mymod, int to_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }
			
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.WorldData );
			packet.Write( (int)myworld.Logic.WorldStatus );

			packet.Send( to_who );
		}
		
		public static void SendPromptForReset( ResetModeMod mymod, int to_who ) {
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.PromptForReset );

			packet.Send( to_who );
		}
		
		public static void SendPlayerSessionJoinAcknowledgement( ResetModeMod mymod, int to_who ) {
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.PlayerSessionJoinAck );

			packet.Send( to_who );
		}



		////////////////
		// Receivers
		////////////////

		private static void ReceiveModSettingsRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }
			
			ServerPackets.SendModSettings( mymod, player_who );
		}
		
		private static void ReceiveWorldDataRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }

			ServerPackets.SendWorldData( mymod, player_who );
		}
		
		private static void ReceivePlayerSessionJoinAcknowledgeRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }

			Player player = Main.player[player_who];
			var myplayer = Main.player[ player_who ].GetModPlayer<ResetModePlayer>();

			myplayer.Logic.AddPlayerToSession( mymod, player );

			ServerPackets.SendPlayerSessionJoinAcknowledgement( mymod, player_who );
		}
	}
}
