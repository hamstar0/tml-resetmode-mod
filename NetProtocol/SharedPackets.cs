using HamstarHelpers.DebugHelpers;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.NetProtocol {
	static class SharedPackets {
		public static bool HandlePacket( ResetModeMod mymod, ResetModeProtocolTypes protocol, BinaryReader reader, int player_who ) {
			switch( protocol ) {
			case ResetModeProtocolTypes.PlayerData:
				SharedPackets.ReceivePlayerData( mymod, reader, player_who );
				return true;
			}
			return false;
		}



		////////////////
		// Senders
		////////////////

		public static void SendPlayerData( ResetModeMod mymod, Player player, int to_who=-1, int ignore_who=-1 ) {
			if( mymod.Logic.NetMode == 0 ) { throw new Exception( "Not single player mode." ); }

			var myplayer = player.GetModPlayer<ResetModePlayer>();
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.PlayerData );
			packet.Write( (int)player.whoAmI );
			myplayer.NetSend( packet );

			packet.Send( to_who, ignore_who );

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( ">Send PlayerData" );
			}
		}



		////////////////
		// Receivers
		////////////////
		
		private static void ReceivePlayerData( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( mymod.Logic.NetMode == 0 ) { throw new Exception("Not single player mode."); }

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( "<Receive ReceivePlayerData" );
			}

			int data_of_who = reader.ReadInt32();
			Player player = Main.player[ data_of_who ];
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.NetReceive( reader );
		}
	}
}
