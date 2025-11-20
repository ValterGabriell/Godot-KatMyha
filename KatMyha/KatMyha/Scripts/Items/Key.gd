extends Node2D
class_name Key

@export var keyName: String = "GoldenKey";
@export var keyPhase: int = 1;
@export var keyID: int = 1


func _on_area_2d_body_entered(body: Node2D) -> void:	
	if(body.is_in_group("player")):
		KeyManager.add_key(self)
		queue_free()

static func EmptyKey() -> Key:
	var empty_key = Key.new()
	return empty_key