using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeStopCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 ) {
					return CommandType.World;
				}
				return CommandType.Console;
			}
		}
		public override string Command { get { return "resetmodestop"; } }
		public override string Usage { get { return "/resetmodestop"; } }
		public override string Description { get { return "Ends reset mode."; } }


		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			var mymod = (ResetModeMod)this.mod;
			
			if( ResetModeAPI.StopSession() ) {
				caller.Reply( "Reset mode ended!.", Color.YellowGreen );
			} else {
				caller.Reply( "Reset mode is not in session.", Color.Red );
			}
		}
	}
}
