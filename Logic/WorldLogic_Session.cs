using HamstarHelpers.WorldHelpers;
using ResetMode.NetProtocol;
using Terraria;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void EngageWorldForCurrentSession( ResetModeMod mymod ) {
			mymod.Session.AllPlayedWorlds.Add( WorldHelpers.GetUniqueId() );

			mymod.Session.AwaitingNextWorld = false;
			mymod.SessionJson.SaveFile();

			this.WorldStatus = ResetModeStatus.Active;
			ServerPackets.SendWorldData( mymod, -1 );
		}

		public void CloseWorldForCurrentSession( ResetModeMod mymod ) {
			this.IsExiting = true;

			this.WorldStatus = ResetModeStatus.Expired;
			if( Main.netMode == 2 ) {
				ServerPackets.SendWorldData( mymod, -1 );
			}

			mymod.Session.AwaitingNextWorld = true;
			mymod.SessionJson.SaveFile();

			this.GoodExit();
		}

		////////////////

		public void ClearSessionWorlds( ResetModeMod mymod ) {
			if( !mymod.Logic.IsSessionStarted( mymod ) ) { return; }

			mymod.Session.AllPlayedWorlds.Clear();

			mymod.Session.AwaitingNextWorld = false;
			mymod.SessionJson.SaveFile();

			this.WorldStatus = ResetModeStatus.Normal;
			ServerPackets.SendWorldData( mymod, -1 );
		}
	}
}
