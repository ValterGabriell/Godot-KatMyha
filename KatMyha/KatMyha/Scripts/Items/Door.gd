extends StaticBody2D
class_name Door

var is_locked: bool = true
var required_key_name: String = "GoldenKey"
var is_player_on_door: bool = false
var required_key_phase: int = 0

func _on_area_2d_body_entered(body: Node2D) -> void:
	if body.is_in_group("player"):
		is_player_on_door = true
		body.SetIsEnemyOnDoor(self);
		
