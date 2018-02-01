using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.NetProtocol;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public static int GetHash( Player player ) {
			return PlayerIdentityHelpers.GetVanillaSnapshotHash( player, true, false );
		}


		////////////////

		public string ActiveForWorldUid = "";


		////////////////

		public PlayerLogic() { }

		public void LoadLocal( ResetModeMod mymod, Player player, TagCompound tags ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.LoadLocal Loading player " + player.name + " (" + player.whoAmI + ")" );
			}

			if( tags.ContainsKey( "active_world_uid" ) ) {
				this.ActiveForWorldUid = tags.GetString( "active_world_uid" );
			}

			if( Main.netMode == 0 ) {
				if( !mymod.SessionJson.LoadFile() ) {
					mymod.SessionJson.SaveFile();
				}
			}
		}

		public TagCompound SaveLocal( ResetModeMod mymod, Player player ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.SaveLocal Saving player " + player.name + " ("+player.whoAmI+")" );
			}

			return new TagCompound {
				{ "active_world_uid", this.ActiveForWorldUid }
			};
		}

		////////////////

		public void SyncClient( ResetModeMod mymod, Player player ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.SyncClient player: " + player.whoAmI );
			}

			//if( this.ActiveForWorldUid == WorldHelpers.GetUniqueId() ) {
			SharedPackets.SendPlayerData( mymod, player );
		}

		////////////////

		public void NetSend( ResetModeMod mymod, Player player, BinaryWriter writer ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.NetSend player: " + player.whoAmI+ ", this.ActiveForWorldUid: "+ this.ActiveForWorldUid );
			}

			writer.Write( this.ActiveForWorldUid );

			mymod.Logic.NetSendPlayerData( mymod, player, writer );
		}

		public void NetReceive( ResetModeMod mymod, Player player, BinaryReader reader ) {
			this.ActiveForWorldUid = reader.ReadString();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.NetReceive player: " + player.whoAmI + ", this.ActiveForWorldUid: " + this.ActiveForWorldUid );
			}

			mymod.Logic.NetReceivePlayerData( mymod, player, reader );
		}
	}
}
