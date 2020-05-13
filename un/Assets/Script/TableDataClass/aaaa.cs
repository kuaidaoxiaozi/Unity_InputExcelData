using System.IO;
public class aaaa : TDBase {
	public int a2 {
		get;
		private set;
	}
	public int a3 {
		get;
		private set;
	}
	public int a4 {
		get;
		private set;
	}
	public int a5 {
		get;
		private set;
	}
	public int a6 {
		get;
		private set;
	}
	public override void updateData(BinaryReader br) {
		if (canInit == false)
			return;
		canInit = false;
		id = br.ReadInt32();
		a2 = br.ReadInt32();
		a3 = br.ReadInt32();
		a4 = br.ReadInt32();
		a5 = br.ReadInt32();
		a6 = br.ReadInt32();
	}
}
