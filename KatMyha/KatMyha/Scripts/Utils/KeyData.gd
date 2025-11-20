extends Resource
class_name KeyData

var keyName: String
var keyPhase: int
var keyID: int

func _init(name: String = "", phase: int = 0, id: int = 0) -> void:
    keyName = name
    keyPhase = phase
    keyID = id