#pragma strict

var doorClip : AnimationClip;

	function OnTriggerEnter (player : Collider) {
	if (player.tag=="Player")
	GameObject.Find("SlidingDoor").animation.Play("SlidingDoorOpen");
	}