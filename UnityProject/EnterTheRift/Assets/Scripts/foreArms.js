#pragma strict

var tracker : GameObject;
function Update () {
	
	transform.localEulerAngles.y = tracker.transform.eulerAngles.z;
}