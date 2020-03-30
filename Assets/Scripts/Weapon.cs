using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public float fireRate = 0f;
	public int Damage = 10;
	public LayerMask whatToHit;
	public Transform BulletTrailPrefab;
    public Transform HitPrefab;
    public Transform MuzzleFlashPrefab;
    public float effectSpawnRate = 10f;

    // Handle camera shaking
    public float camShakeAmt = 0.05f;
    public float camShakeLength = 0.1f;
    CameraShake camShake;

	private float timeToFire = 0;
	private Transform firePoint;
	private float timeToSpawnEffect = 0f;

	// Use this for initialization
	void Awake ()
	{
		this.firePoint = transform.Find ("Fire Point");
		if (this.firePoint == null) {
			Debug.LogError ("No Fire Point?  WHAT?!");
		}
	}

    void Start()
    {
        camShake = GameMaster.gm.GetComponent<CameraShake>();
        if (camShake == null)
        {
            Debug.LogError("NO CameraShake script found on GM object.");
        }
    }

    // Update is called once per frame
    void Update ()
	{
		if (this.fireRate == 0) {
			if (Input.GetButtonDown ("Fire1")) {
				this.Shoot ();
			}
		} else {
			if (Input.GetButton ("Fire1") && Time.time > this.timeToFire) {
				this.timeToFire = Time.time + 1 / this.fireRate;
				this.Shoot ();
			}
		}
	}

	void Shoot ()
	{
		Vector2 screenToWorldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector2 mousePosition = new Vector2 (screenToWorldPoint.x, screenToWorldPoint.y);
		Vector2 firePointPosition = new Vector2 (firePoint.position.x, firePoint.position.y);
		RaycastHit2D hit = Physics2D.Raycast (firePointPosition, mousePosition - firePointPosition, 100, whatToHit);

		Debug.DrawLine (firePointPosition, (mousePosition - firePointPosition) * 100, Color.cyan);
		if (hit.collider != null) {
			Debug.DrawLine (firePointPosition, hit.point, Color.red);
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.DamageEnemy(Damage);
                Debug.Log("We hit " + hit.collider.name + " and did " + this.Damage + " damage.");
            }
		}

        if (Time.time >= this.timeToSpawnEffect)
        {
            Vector3 hitPos;
            Vector3 hitNormal;

            if (hit.collider == null)
            {
                hitPos = (mousePosition - firePointPosition) * 30;
                hitNormal = new Vector3(9999, 9999, 9999);
            }
            else
            {
                hitPos = hit.point;
                hitNormal = hit.normal;
            }

            this.Effect(hitPos, hitNormal);
            this.timeToSpawnEffect = Time.time + 1 / this.effectSpawnRate;
        }
    }

	void Effect (Vector3 hitPos, Vector3 hitNormal)
	{
        Transform trail = Instantiate (this.BulletTrailPrefab, this.firePoint.position, firePoint.rotation);
        LineRenderer lr = trail.GetComponent<LineRenderer>();

        if (lr != null)
        {
            //SET POSITIONS
            lr.SetPosition(0, firePoint.position);
            lr.SetPosition(1, hitPos);
        }

        Destroy(trail.gameObject, 0.04f);

        if (hitNormal != new Vector3(9999,9999,9999))
        {
            Transform hitParticle = Instantiate(HitPrefab, hitPos, Quaternion.FromToRotation(Vector3.right, hitNormal));
            Destroy(hitParticle.gameObject, 1f);
        }

        Transform clone = Instantiate (this.MuzzleFlashPrefab, this.firePoint.position, firePoint.rotation);
		clone.parent = this.firePoint;
		float size = Random.Range (0.6f, 0.9f);
		clone.localScale = new Vector3 (size, size, size);
		Destroy (clone.gameObject, 0.02f);

        //Shake the camera
        camShake.Shake(camShakeAmt, camShakeLength);
	}
}
