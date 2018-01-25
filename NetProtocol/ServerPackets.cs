using HamstarHelpers.DebugHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocol {
	static class ServerPackets {
		public static void HandlePacket( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			ResetModeProtocolTypes protocol = (ResetModeProtocolTypes)reader.ReadByte();
			
			switch( protocol ) {
			case ResetModeProtocolTypes.RequestModSettings:
				ServerPackets.ReceiveModSettingsRequest( mymod, reader, player_who );
				break;
			default:
				LogHelpers.Log( "Invalid packet protocol: " + protocol );
				break;
			}
		}


		
		////////////////
		// Server Senders
		////////////////

		public static void SendModSettings( ResetModeMod mymod, int to_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.ModSettings );
			packet.Write( (string)mymod.JsonConfig.SerializeMe() );

			packet.Send( to_who );
		}

		public static void SendWorldStatus( ResetModeMod mymod ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }
			
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.WorldStatus );
			packet.Write( (int)myworld.Logic.WorldStatus );

			packet.Send( -1 );
		}



		////////////////
		// Server Receivers
		////////////////

		private static void ReceiveModSettingsRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( Main.netMode != 2 ) { throw new Exception( "Not server" ); }

			ServerPackets.SendModSettings( mymod, player_who );
		}
	}
}
