using TMPro;
using UnityEngine;

public class CoinCount : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_text;
    private Transform m_target;

    private void Update()
    {
        //�^�[�Q�b�g�����Ȃ��Ȃ��Ă�����폜����
        if (m_target == null)
        {
            Destroy(gameObject);
            return;
        }

        //���[���h���W���X�N���[�����W�ɕϊ�
        transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, m_target.position + Vector3.up);
    }

    //���l�ݒ�
    public void SetNumber(int count)
    {
        m_text.text = count.ToString();
    }


    //�\������Ώہi�v���C���[�j��ݒ�
    public void SetTarget(Transform target)
    {
        m_target = target;
    }
}
