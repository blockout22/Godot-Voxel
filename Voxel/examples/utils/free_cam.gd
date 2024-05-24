extends Camera3D

var mouse_captured = false
var yaw = 0
var pitch = 0
@export var speed = 0.5
@export var mouse_sensitivity = 0.005

@export_category("Camera Keybinds")
@export var FORWARD = KEY_W
@export var BACKWARD = KEY_S
@export var LEFT = KEY_A
@export var RIGHT = KEY_D
@export var UP = KEY_E
@export var DOWN = KEY_Q


@export var isSinglePlayer: bool = false

func _ready() -> void:
	current = is_multiplayer_authority()
	pass

func _enter_tree():
	set_multiplayer_authority(name.to_int())

@rpc("any_peer")
func _process(_delta: float) -> void:
	if not isSinglePlayer and not is_multiplayer_authority():
		return;
	var input_vector = Vector3.ZERO
	if Input.is_key_pressed(FORWARD):
		input_vector.z = speed * get_process_delta_time()
	elif Input.is_key_pressed(BACKWARD):
		input_vector.z = -speed * get_process_delta_time()
		
	if Input.is_key_pressed(LEFT):
		input_vector.x = -speed * get_process_delta_time()
	elif Input.is_key_pressed(RIGHT):
		input_vector.x = speed * get_process_delta_time()
	
	if Input.is_key_pressed(UP):
		input_vector.y = speed * get_process_delta_time()
	elif Input.is_key_pressed(DOWN):
		input_vector.y = -speed * get_process_delta_time()
	
	var forward = -global_transform.basis.z.normalized()
	var up = global_transform.basis.y.normalized()
	var right = global_transform.basis.x.normalized()
	
	var direction = (forward * input_vector.z + up * input_vector.y + right * input_vector.x).normalized()
	global_position.x += direction.x * speed
	global_position.y += direction.y * speed
	global_position.z += direction.z * speed

func _input(event):
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_RIGHT:
			if event.pressed:
				Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
				mouse_captured = true
			else:
				Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
				mouse_captured = false

	if event is InputEventMouseMotion and mouse_captured:
		yaw -= event.relative.x * mouse_sensitivity
		pitch -= event.relative.y * mouse_sensitivity
		pitch = clamp(pitch, -PI/2, PI/2) # Prevent over-rotation
		var rot = Transform3D()
		rot.basis = Basis(Vector3(0, 1, 0), yaw)
		rot.basis = rot.basis.rotated(rot.basis.x, pitch)
		global_transform = Transform3D(rot.basis, global_transform.origin)	
