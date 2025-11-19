extends Node

var collected_keys: Dictionary = {}
func add_key(key: Key) -> void:
    if key.keyName not in collected_keys:
        collected_keys[key.keyID] = key
        print_debug("Key collected: %s" % key.keyName)

func has_key(keyName: String) -> bool:
    return collected_keys.has(keyName)