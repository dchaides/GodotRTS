using Godot;

public static class GameExtensions {
    public static GameManager GetGameManager(this Node node){
        return node.GetNode("/root/GameManager") as GameManager;    
    }

    public static BuildManager GetBuildManager(this Node node){
        return node.GetNode("/root/BuildManager") as BuildManager;    
    }

    public static GameManager GetGameManager(this StaticBody3D node){
        return node.GetNode("/root/GameManager") as GameManager;    
    }

    public static BuildManager GetBuildManager(this StaticBody3D node){
        return node.GetNode("/root/BuildManager") as BuildManager;    
    }
 
}