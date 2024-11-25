using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class SwingingDone : MonoBehaviour
 {
     [Header("References")]
     public LineRenderer lr;
     public Transform gunTip, cam, player;
     public LayerMask whatIsGrappleable;
     public PlayerMovement pm;

     [Header("Swinging")]
     private float _maxSwingDistance = 25f;
     private Vector3 _swingPoint;
     private SpringJoint _joint;

     [Header("OdmGear")]
     public Transform orientation;
     public Rigidbody rb;
     public float horizontalThrustForce;
     public float forwardThrustForce;
     public float extendCableSpeed;

     [Header("Prediction")]
     private RaycastHit _predictionHit;
     public float predictionSphereCastRadius;
     public Transform predictionPoint;

     [Header("Input")]
     public KeyCode swingKey = KeyCode.Mouse0;


     private void Update()
     {
         if (Input.GetKeyDown(swingKey)) StartSwing();
         if (Input.GetKeyUp(swingKey)) StopSwing();
         if(pm.grappleGunActive) CheckForSwingPoints();
         if (_joint != null) OdmGearMovement();
     }

     private void LateUpdate()
     {
         DrawRope();
     }

     private void CheckForSwingPoints()
     {
         if (_joint != null) return;

         RaycastHit sphereCastHit;
         Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, 
                             out sphereCastHit, _maxSwingDistance, whatIsGrappleable);

         RaycastHit raycastHit;
         Physics.Raycast(cam.position, cam.forward, 
                             out raycastHit, _maxSwingDistance, whatIsGrappleable);

         Vector3 realHitPoint;
         if (raycastHit.point != Vector3.zero)
             realHitPoint = raycastHit.point;
         
         else if (sphereCastHit.point != Vector3.zero)
             realHitPoint = sphereCastHit.point;
         
         else
             realHitPoint = Vector3.zero;

         if (realHitPoint != Vector3.zero)
         {
             predictionPoint.gameObject.SetActive(true);
             predictionPoint.position = realHitPoint;
         }
         
         else
         {
             predictionPoint.gameObject.SetActive(false);
         }

         _predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
     }


     private void StartSwing()
     {
         if(!pm.grappleGunActive) return;

         if (_predictionHit.point == Vector3.zero) return;

         if(GetComponent<Grappling>() != null)
             GetComponent<Grappling>().StopGrapple();
         pm.ResetRestrictions();

         pm.swinging = true;

         _swingPoint = _predictionHit.point;
         _joint = player.gameObject.AddComponent<SpringJoint>();
         _joint.autoConfigureConnectedAnchor = false;
         _joint.connectedAnchor = _swingPoint;

         float distanceFromPoint = Vector3.Distance(player.position, _swingPoint);

         _joint.maxDistance = distanceFromPoint * 0.8f;
         _joint.minDistance = distanceFromPoint * 0.25f;

         _joint.spring = 4.5f;
         _joint.damper = 7f;
         _joint.massScale = 4.5f;

         lr.positionCount = 2;
         _currentGrapplePosition = gunTip.position;
     }

     public void StopSwing()
     {
         pm.swinging = false;

         lr.positionCount = 0;

         Destroy(_joint);
     }

     private void OdmGearMovement()
     {
         if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
         if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
         if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * horizontalThrustForce * Time.deltaTime);
         if (Input.GetKey(KeyCode.Space))
         {
             Vector3 directionToPoint = _swingPoint - transform.position;
             rb.AddForce(directionToPoint.normalized * forwardThrustForce * Time.deltaTime);

             float distanceFromPoint = Vector3.Distance(transform.position, _swingPoint);

             _joint.maxDistance = distanceFromPoint * 0.8f;
             _joint.minDistance = distanceFromPoint * 0.25f;
         }

         if (Input.GetKey(KeyCode.S))
         {
             float extendedDistanceFromPoint = Vector3.Distance(transform.position, _swingPoint) + extendCableSpeed;

             _joint.maxDistance = extendedDistanceFromPoint * 0.8f;
             _joint.minDistance = extendedDistanceFromPoint * 0.25f;
         }
     }

     private Vector3 _currentGrapplePosition;

     private void DrawRope()
     {
         if (!_joint) return;

         _currentGrapplePosition = 
             Vector3.Lerp(_currentGrapplePosition, _swingPoint, Time.deltaTime * 8f);

         lr.SetPosition(0, gunTip.position);
         lr.SetPosition(1, _currentGrapplePosition);
     }
}
