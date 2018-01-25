using Microsoft.Xna.Framework;
using ResetMode.Logic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Commands {
	class ResetModeBeginCommand : ModCommand {
		public override CommandType Type {
			get {
				if( Main.netMode == 0 ) {
					return CommandType.World;
				}
				return CommandType.Console;
			}
		}
		public override string Command { get { return "resetmodebegin"; } }
		public override string Usage { get { return "/resetmodebegin"; } }
		public override string Description { get { return "Begins reset mode."; } }
		

		////////////////

		public override void Action( CommandCaller caller, string input, string[] args ) {
			var mymod = (ResetModeMod)this.mod;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( myworld.Logic.WorldStatus == ResetModeStatus.Normal ) {
				myworld.Logic.Start( mymod );
				caller.Reply( "Reset mode begun! This will continue until for each new world /resetmodeend is called.", Color.YellowGreen );
			} else {
				caller.Reply( "Reset mode is already in session for this world.", Color.Red );
			}
		}
	}
}
