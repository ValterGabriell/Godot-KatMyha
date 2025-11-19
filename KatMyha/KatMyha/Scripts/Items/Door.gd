extends StaticBody2D
class_name Door

var is_locked: bool = true
var required_key_name: String = "GoldenKey"
var is_player_on_door: bool = false
@export var required_key_phase: int = 1

func _on_area_2d_body_entered(body: Node2D) -> void:
	if body.is_in_group("player"):
		is_player_on_door = true
		body.SetIsEnemyOnDoor(self);
		
func _on_area_2d_body_exited(body: Node2D) -> void:
	if body.is_in_group("player"):
		is_player_on_door = false
		body.RemoveEnemyOnDoor();

func try_unlock() -> bool:
	if is_locked and KeyManager.has_key(required_key_name):
		var key: Key = KeyManager.get_key(required_key_name)
		if key.keyPhase >= required_key_phase:
			is_locked = false
			print_debug("Door unlocked with key: %s" % required_key_name)
			return true
	return false
