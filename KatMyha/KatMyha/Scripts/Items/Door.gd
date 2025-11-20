extends StaticBody2D
class_name Door

var is_locked: bool = true
var required_key_id: int = 1
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
	if is_locked and KeyManager.has_key(required_key_id):
		print_debug("Door unlocked with key ID: %d" % required_key_id)
		KeyManager.remove_key(required_key_id)
		is_locked = false
		queue_free()
		return true
	return false
