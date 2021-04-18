using Photon.Pun;
using UnityEngine;

namespace Gisha.Pushers.Core
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float pushMagnitude;

        Vector3 _angularInput;
        Vector3 _pushInput;

        Rigidbody _rb;
        PhotonView _pv;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _pv = GetComponent<PhotonView>();
        }

        private void Update()
        {
            if (!_pv.IsMine)
                return;

            Move();
            PushInput();
        }

        private void Move()
        {
            _angularInput = new Vector3(-Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
            _rb.angularVelocity = _angularInput * movementSpeed;
        }

        private void PushInput()
        {
            _pushInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            _pushInput *= -1f;
        }

        [PunRPC]
        private void PushMe(Vector3 pushForce)
        {
            _rb.AddForce(pushForce);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                var pv = collision.collider.GetComponent<PhotonView>();

                if (!pv.IsMine)
                {
                    var pushForce = _pushInput * (_rb.velocity.magnitude + pushMagnitude);
                    pv.RPC("PushMe", RpcTarget.All, pushForce);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _pushInput);
        }
    }
}