using UnityEngine;
using Unity.Netcode;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    [SerializeField] float m_moveSpeed = 2;
    private Rigidbody m_rigidBody;
    private Vector2 m_moveInput = Vector2.zero;

    //�R�C���̃v���n�u
    [SerializeField] private GameObject m_coinCountPrefabs;
    private CoinCount m_coinCount;

    //�R�C���擾��
    private NetworkVariable<int> m_coinNum;

    private NetworkVariable<int> hp;

    // �e���˗p

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
        // Rigidbody ���擾
        m_rigidBody = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        //�R�C���擾���ω��ʒm
        m_coinNum.OnValueChanged += OnCoinNumChanged;

        //�R�C���J�E���gUI����
        var canvas = GameObject.Find("Canvas").transform;
        m_coinCount = Instantiate(m_coinCountPrefabs, canvas).GetComponent<CoinCount>();
        m_coinCount.SetTarget(transform);
        m_coinCount.SetNumber(m_coinNum.Value);
    }

    //�R�C��UI�X�V
    void OnCoinNumChanged(int prevValue, int newValue)
    {
        m_coinCount.SetNumber(newValue);
    }

    private void Update()
    {
        //owner�̏ꍇ
        if (IsOwner)
        {
            // �ړ����͂�ݒ�
            SetMoveInputServerRpc(
                    Input.GetAxisRaw("Horizontal"),
                    Input.GetAxisRaw("Vertical"),
                    Input.GetKey(KeyCode.Space)

                    );
        }

        //�T�[�o�[�i�z�X�g�j�̏ꍇ
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
    // �ړ����͂��Z�b�g����RPC
    [ServerRpc]
    private void SetMoveInputServerRpc(float x, float y, bool space)
    {
        m_moveInput = new Vector2(x, y);
        isKeySpace = space;
    }

    //=================================================================
    //�T�[�o�[���ōs������
    //=================================================================
    // �T�[�o�[���ŌĂ΂��Update
    private void ServerUpdate()
    {
        //�ړ�
        //var velocity = Vector3.zero;
        //velocity.x = m_moveSpeed * m_moveInput.normalized.x;
        //velocity.z = m_moveSpeed * m_moveInput.normalized.y;
        ////�ړ�����
        //m_rigidBody.AddForce(velocity * Time.deltaTime);
    }

    [System.Obsolete]
    void OnTriggerEnter(Collider other)
    {
        if (IsServer == false) { return; }
        if (other.gameObject.CompareTag("Coin"))
        {
            //�擾����
            m_coinNum.Value += 1;
            //�R�C���폜�����iCoinManager�̏������Ăԁj
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
