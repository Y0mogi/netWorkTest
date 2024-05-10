using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    [SerializeField] float m_moveSpeed = 2;
    private Rigidbody m_rigidBody;
    private Vector2 m_moveInput = Vector2.zero;

    //コインのプレハブ
    [SerializeField] private GameObject m_coinCountPrefabs;
    private CoinCount m_coinCount;

    //コイン取得数
    private NetworkVariable<int> m_coinNum;

    private NetworkVariable<int> hp;

    // 弾発射用

    [SerializeField] private float rotationSpeed = 20.0f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField,FormerlySerializedAs("bulletOffsetPosition")] public float bulletSpawnOffsetPosition = 1f;
    private bool isKeySpace;
    public int recastSecond = 5;
    private float firedTime;

    public int ID;
    void Awake()
    {
        m_coinNum = new NetworkVariable<int>(0);
        hp = new NetworkVariable<int>(10);
    }

    void Start()
    {
        // Rigidbody を取得
        m_rigidBody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        //コイン取得数変化通知
        m_coinNum.OnValueChanged += OnCoinNumChanged;

        //コインカウントUI生成
        var canvas = GameObject.Find("Canvas").transform;
        m_coinCount = Instantiate(m_coinCountPrefabs, canvas).GetComponent<CoinCount>();
        m_coinCount.SetTarget(transform);
        m_coinCount.SetNumber(m_coinNum.Value);
    }

    //コインUI更新
    void OnCoinNumChanged(int prevValue, int newValue)
    {
        m_coinCount.SetNumber(newValue);
    }

    private void Update()
    {
        //ownerの場合
        if (IsOwner)
        {
            // 移動入力を設定
            SetMoveInputServerRpc(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical"),
                    Input.GetKey(KeyCode.Space)

                    );
        }

        //サーバー（ホスト）の場合
        if (IsServer)
        {
            ServerUpdate();
        }
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        if (IsServer) 
        {
            var moveVector = new Vector3(m_moveInput.x, 0, m_moveInput.y);

            //var coefficient = (m_moveSpeed * moveVector.magnitude - m_rigidBody.velocity.magnitude) / Time.fixedDeltaTime;

            m_rigidBody.AddForce((moveVector * m_moveSpeed) * Time.deltaTime);

            if (isKeySpace)
            {
                var time = Time.time;
               
                if (firedTime == 0 || firedTime + recastSecond <= time)
                {
                    SpawnBulletPrefab();
                    firedTime = time;
                }
            }
        }

        //Debug.DrawRay(transform.position, new Vector3(m_moveInput.x, 0, m_moveInput.y),Color.red);
    }

    //=================================================================
    //RPC
    //=================================================================
    // 移動入力をセットするRPC
    [ServerRpc]
    private void SetMoveInputServerRpc(float x, float y, bool space)
    {
        m_moveInput = new Vector2(x, y);
        isKeySpace = space;
    }

    //=================================================================
    //サーバー側で行う処理
    //=================================================================
    // サーバー側で呼ばれるUpdate
    private void ServerUpdate()
    {
        //移動
        //var velocity = Vector3.zero;
        //velocity.x = m_moveSpeed * m_moveInput.normalized.x;
        //velocity.z = m_moveSpeed * m_moveInput.normalized.y;
        ////移動処理
        //m_rigidBody.AddForce(velocity * Time.deltaTime);
    }

    [System.Obsolete]
    void OnTriggerEnter(Collider other)
    {
        if (IsServer == false) { return; }
        if (other.gameObject.CompareTag("Coin"))
        {
            //取得処理
            m_coinNum.Value += 1;
            //コイン削除処理（CoinManagerの処理を呼ぶ）
            CoinManager.Instance.DeleteCoin(other.gameObject);
        }

        if (other.gameObject.CompareTag("Bullet"))
        {
            hp.Value -= 1;
            BulletManager.Instance.DeleteBullet(other.gameObject);
        }
    }

    [System.Obsolete]
    private void SpawnBulletPrefab()
    {

        var moveVector = new Vector3(m_moveInput.normalized.x, 0, m_moveInput.normalized.y);


        Debug.Log(moveVector.ToString());
        var gmo = GameObject.Instantiate(bulletPrefab, transform.position + new Vector3(0, 0.6f, 0) + (moveVector * this.bulletSpawnOffsetPosition), Quaternion.identity);

        var bullet = gmo.GetComponent<Bullet>();
        bullet.direction = moveVector;

        var netObject = gmo.GetComponent<NetworkObject>();
        BulletManager.Instance.GenerateBullet(gmo);
        netObject.Spawn(true);
        
    }
}
