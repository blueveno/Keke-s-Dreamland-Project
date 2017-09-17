using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// A falling platform is a platform that fall after time when the player mounts it. 
    /// </summary>
    public class FallingPlatform : MonoBehaviour
    {

        public float timeBeforeFalling = 0.75f;
        public SpriteRenderer sprite;

        private bool isFalling = false;
        private Rigidbody2D rgbd;
        private Animator anim;

        private void Awake()
        {
            rgbd = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player" && !isFalling)
            {
                foreach (ContactPoint2D cp in collision.contacts)
                {
                    if (cp.normal.y < 0)
                    {
                        StartCoroutine(Fall());
                    }
                }
            }

            else if (collision.gameObject.tag != "Player")
            {
                StartCoroutine(BlinkAndDestroy());
            }

        }

        private IEnumerator Fall()
        {
            isFalling = true;

            yield return new WaitForSeconds(timeBeforeFalling);

            // Activate gravity and trigger animation.
            rgbd.gravityScale = 0.4f;
            rgbd.freezeRotation = true;
            rgbd.bodyType = RigidbodyType2D.Dynamic;

            anim.SetTrigger("Fall");
        }

        private IEnumerator BlinkAndDestroy()
        {
            // Desactivate collision and prevent to fall through the ground.
            rgbd.bodyType = RigidbodyType2D.Static;
            GetComponent<BoxCollider2D>().enabled = false;

            anim.SetTrigger("OnGround");

            // Blink effect.
            for (int i = 0; i < 4; i++)
            {
                if (i % 2 == 0)
                    sprite.color = Color.gray;
                else
                    sprite.color = Color.white;

                yield return new WaitForSeconds(0.15f);
            }

            // Destroy it.
            Destroy(gameObject);
        }
    }

}
