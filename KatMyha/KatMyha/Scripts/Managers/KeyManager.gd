extends Node

var collected_keys: Dictionary = {}



func add_key(key: Key) -> void:
	if key.keyID not in collected_keys:
		var keyData = KeyData.new(key.keyName, key.keyPhase, key.keyID)
		collected_keys[key.keyID] = keyData


func has_key(keyID: int) -> bool:
	return collected_keys.has(keyID)

func get_key_phase(keyID: int) -> int:
	if has_key(keyID):
		return collected_keys[keyID].keyPhase
	return 0

func get_key_name(keyID: int) -> String:
	if has_key(keyID):
		return collected_keys[keyID].keyName
	return ""

func remove_key(keyID: int) -> void:
	if has_key(keyID):
		collected_keys.erase(keyID)
