using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using ResetMode.NetProtocols;
using System.Collections.Generic;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static string DataFileNameOnly { get { return "Session"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }



		////////////////

		public ResetModeSessionData Data { get; private set; }

		internal IDictionary<string, float> Rewards = new Dictionary<string, float>();



		////////////////

		internal SessionLogic() {
			this.Data = new ResetModeSessionData();

			Main.OnTick += SessionLogic._Update;
		}

		~SessionLogic() {
			Main.OnTick -= SessionLogic._Update;
		}


		////////////////

		public void Load( ResetModeMod mymod ) {
			bool success;

			var data = DataFileHelpers.LoadJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, out success );
			if( success ) {
				this.Data = data;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.Load - Success? "+success );
			}
		}

		public void Save( ResetModeMod mymod ) {
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.Data );
		}
		
		////////////////

		internal void SetData( ResetModeMod mymod, ResetModeSessionData data, ResetModeStatus world_status ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			this.Data = data;
			myworld.Data.WorldStatus = world_status;
		}


		////////////////

		private static void _Update() {
			var mymod = ResetModeMod.Instance;
			if( mymod == null ) { return; }

			mymod.Session.Update();
		}

		internal void Update() {
			if( !this.Data.IsRunning ) { return; }

			var mymod = ResetModeMod.Instance;

			if( TmlLoadHelpers.IsWorldLoaded() ) {
				var myworld = mymod.GetModWorld<ResetModeWorld>();

				switch( myworld.Data.WorldStatus ) {
				case ResetModeStatus.Normal:
					break;

				case ResetModeStatus.Active:
					break;

				case ResetModeStatus.Expired:
					if( !myworld.Data.IsExiting ) {
						this.GoodExit( mymod );
					}
					break;
				}
			}
		}


		////////////////

		public void LogRewardsPPSpending( Player player, float pp ) {
			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "ResetMode.Logic.SessionLogic.LogRewardsPPSpending - Invalid player UID for " + player.name );
				return;
			}

			if( this.Data.PlayerPPSpendings.ContainsKey( pid ) ) {
				this.Data.PlayerPPSpendings[pid] += pp;
			} else {
				this.Data.PlayerPPSpendings[pid] = pp;
			}

			if( Main.netMode == 1 ) {
				PacketProtocol.QuickSendToServer<RewardsSpendingProtocol>();
			}
		}
	}
}
