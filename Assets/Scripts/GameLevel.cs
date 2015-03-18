using System;
//Mainly making this to make writing and reading game levels very easy.
public struct GameLevel {
	private string name_;
	public string Name {
		get { return name_; }
		set { name_ = value; }
	}
	private TerrainType[,] layout_;
	public TerrainType[,] Layout {
		get { return layout_; }
		set { layout_ = value; }
	}
	private TerrainType[] faces_;
	public TerrainType[] Faces {
		get { return faces_; }
		set { faces_ = value; }
	}
}