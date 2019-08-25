using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal bool IsPlaying( Player player ) {
			string uid = PlayerIdentityHelpers.GetUniqueId( player );

			return this.Data.PlayersValidated.Contains( uid );
		}


		internal void AddPlayer( Player player ) {
			string uid = PlayerIdentityHelpers.GetUniqueId( player );

			this.Data.PlayersValidated.Add( uid );
			if( Main.netMode != 1 ) {
				this.Save();
			}
		}
	}
}
