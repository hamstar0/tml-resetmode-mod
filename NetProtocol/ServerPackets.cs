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
			case ResetModeProtocolTypes.RequestSessionData:
				ServerPackets.ReceiveSessionDataRequest( mymod, reader, player_who );
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
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }
			
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.ModSettings );
			packet.Write( (string)mymod.ConfigJson.SerializeMe() );

			packet.Send( to_who );
			
			if( to_who == -1 ) {
				for( int i = 0; i < Main.player.Length; i++ ) {
					var player = Main.player[i];
					if( player == null || !player.active ) { continue; }
					var myplayer = player.GetModPlayer<ResetModePlayer>();

					myplayer.FinishModSettingsSync();
				}
			} else {
				var myplayer = Main.player[to_who].GetModPlayer<ResetModePlayer>();
				myplayer.FinishModSettingsSync();
			}

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( ">Send ModSettings" );
			}
		}

		public static void SendWorldData( ResetModeMod mymod, int to_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }
			
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.WorldData );
			packet.Write( (int)myworld.Logic.WorldStatus );

			packet.Send( to_who );

			if( to_who == -1 ) {
				for( int i = 0; i < Main.player.Length; i++ ) {
					var player = Main.player[i];
					if( player == null || !player.active ) { continue; }
					var myplayer = player.GetModPlayer<ResetModePlayer>();

					myplayer.FinishWorldDataSync();
				}
			} else {
				var myplayer = Main.player[to_who].GetModPlayer<ResetModePlayer>();
				myplayer.FinishWorldDataSync();
			}

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( ">Send WorldData" );
			}
		}


		public static void SendSessionData( ResetModeMod mymod, int to_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }

			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.SessionData );
			mymod.Session.NetSend( packet );

			packet.Send( to_who );

			if( to_who == -1 ) {
				for( int i = 0; i < Main.player.Length; i++ ) {
					var player = Main.player[i];
					if( player == null || !player.active ) { continue; }
					var myplayer = player.GetModPlayer<ResetModePlayer>();

					myplayer.FinishSessionDataSync();
				}
			} else {
				var myplayer = Main.player[to_who].GetModPlayer<ResetModePlayer>();
				myplayer.FinishSessionDataSync();
			}

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( ">Send SendSesssionData" );
			}
		}

		public static void SendPromptForReset( ResetModeMod mymod, int to_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }
			
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.PromptForReset );

			packet.Send( to_who );

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( ">Send PromptForReset" );
			}
		}
		
		public static void SendPlayerSessionJoinAcknowledgement( ResetModeMod mymod, int to_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }

			var myplayer = Main.player[to_who].GetModPlayer<ResetModePlayer>();
			ModPacket packet = mymod.GetPacket();

			packet.Write( (byte)ResetModeProtocolTypes.PlayerSessionJoinAck );

			packet.Send( to_who );

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( ">Send PlayerSessionJoinAck" );
			}
		}



		////////////////
		// Receivers
		////////////////

		private static void ReceiveModSettingsRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( "<Receive ReceiveModSettingsRequest" );
			}

			ServerPackets.SendModSettings( mymod, player_who );
		}
		
		private static void ReceiveWorldDataRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( "<Receive ReceiveWorldDataRequest" );
			}

			ServerPackets.SendWorldData( mymod, player_who );
		}
		
		private static void ReceiveSessionDataRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( "<Receive ReceiveSessionDataRequest" );
			}

			ServerPackets.SendSessionData( mymod, player_who );
		}

		private static void ReceivePlayerSessionJoinAcknowledgeRequest( ResetModeMod mymod, BinaryReader reader, int player_who ) {
			if( mymod.Logic.NetMode != 2 ) { throw new Exception( "Not server" ); }

			if( mymod.Config.DebugModeNetwork ) {
				LogHelpers.Log( "<Receive ReceivePlayerSessionJoinAcknowledgeRequest" );
			}

			Player player = Main.player[player_who];
			var myplayer = Main.player[ player_who ].GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSession( mymod, player );

			ServerPackets.SendPlayerSessionJoinAcknowledgement( mymod, player_who );
		}
	}
}
