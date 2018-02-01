using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.NetProtocol;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.AllPlayedWorlds.Contains( world_uid );
		}


		////////////////

		public void EngageWorldForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EngageWorldForCurrentSession "+WorldHelpers.GetUniqueId() );
			}

			mymod.Session.AllPlayedWorlds.Add( WorldHelpers.GetUniqueId() );
			mymod.Session.AwaitingNextWorld = false;

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			this.WorldStatus = ResetModeStatus.Active;

			if( mymod.Logic.NetMode == 2 ) {
				ServerPackets.SendWorldData( mymod, -1 );
			}
		}

		public void CloseWorldForCurrentSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.CloseWorldForCurrentSession" );
			}

			this.WorldStatus = ResetModeStatus.Expired;

			if( mymod.Logic.NetMode == 2 ) {
				ServerPackets.SendWorldData( mymod, -1 );
			}

			mymod.Session.AwaitingNextWorld = true;

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			this.GoodExit( mymod );
		}

		////////////////

		public void ClearAllSessionWorlds( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.ClearAllSessionWorlds" );
			}

			if( !mymod.Logic.IsSessionStarted( mymod ) ) { return; }

			mymod.Session.AllPlayedWorlds.Clear();
			mymod.Session.AwaitingNextWorld = false;

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}

			this.WorldStatus = ResetModeStatus.Normal;

			if( mymod.Logic.NetMode == 2 ) {
				ServerPackets.SendWorldData( mymod, -1 );
			}
		}
	}
}
