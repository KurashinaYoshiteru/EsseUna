using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //�L�[�R�[�h��ϐ������Č�ŕς�����悤�ɂ��Ă���
    private KeyCode KEYCODE_MOVE_LEFT = KeyCode.LeftArrow;
    private KeyCode KEYCODE_MOVE_UP = KeyCode.UpArrow;
    private KeyCode KEYCODE_MOVE_RIGHT = KeyCode.RightArrow;
    private KeyCode KEYCODE_MOVE_DOWN = KeyCode.DownArrow;
    private KeyCode KEYCODE_ACTION = KeyCode.Space;
    private static float MOVE_TIME = 0.2f;       //1�}�X�ړ�����̂ɂ����鎞��

    private bool isMove = false;                 //�����Ă��鎞true
    private bool isActive;                       //���쒆�̂Ƃ���true
    private SoundManager soundManager;           //�T�E���h�}�l�[�W���[�A�A�N�e�B�u�v���C���[�؂�ւ�����SE�Đ��Ɏg�p

    public MapStatus mapStatus;                  //�}�b�v�X�e�[�^�X�A�v���C���[�������ɓn�����
    public int playerPosX;                       //���̃v���C���[��X���W
    public int playerPosY;                       //���̃v���C���[��Y���W

    // Start is called before the first frame update
    void Start()
    {
        //����v���C���[������
        SetController();

        //soundManager�̏�����
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //����v���C���[�؂�ւ�
        SwitchController();

        //���쒆�̂Ƃ��͈ړ�����
        if (isActive)
        {
            Move();
        }
    }

    //����v���C���[������
    private void SetController()
    {
        //MainPlayer���A�N�e�B�u�ɂ���
        if (transform.name == "MainPlayer")
        {
            isActive = true;
            SetColorAlpha(1.0f);
        }
        //SubPlayer���A�N�e�B�u�ɂ���
        else if (transform.name == "SubPlayer")
        {
            isActive = false;
            SetColorAlpha(0.5f);
        }
    }

    //����v���C���[�؂�ւ�
    private void SwitchController()
    {
        if (Input.GetKeyDown(KEYCODE_ACTION))
        {
            if (isActive)
            {
                isActive = false;
                SetColorAlpha(0.5f);
                soundManager.PlaySound("switchControllerSE");
            }
            else
            {
                isActive = true;
                SetColorAlpha(1.0f);
            }
        }
    }

    //����v���C���[�؂�ւ����ɐF��ς���
    private void SetColorAlpha(float alpha)
    {
        float r = transform.GetComponent<SpriteRenderer>().color.r;
        float g = transform.GetComponent<SpriteRenderer>().color.g;
        float b = transform.GetComponent<SpriteRenderer>().color.b;
        transform.GetComponent<SpriteRenderer>().color = new Color(r, g, b, alpha);
    }

    //�ړ�����
    private void Move()
    {
        //�����Ă��Ȃ��Ƃ��ɓ��͂��󂯕t����
        if (!isMove)
        {
            //�ړ��L�[�������ݎ��ɓ���
            if (Input.GetKeyDown(KEYCODE_MOVE_LEFT))
            {
                //���͕����Ɉړ��ł��邩�m�F
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Left, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Left));
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_UP))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Up, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Up));
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_RIGHT))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Right, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Right));
                }
            }
            else if (Input.GetKeyDown(KEYCODE_MOVE_DOWN))
            {
                if (mapStatus.CheckMovable(transform.gameObject, MapStatus.Direction.Down, playerPosX, playerPosY))
                {
                    isMove = true;
                    StartCoroutine(MovingPosition(MapStatus.Direction.Down));
                }
            }
        }
    }

    //MapData�̍X�V�ƍ��W�ړ��A�R���[�`���ő��̓��͂��X���[������
    IEnumerator MovingPosition(MapStatus.Direction dir)
    {
        //���Ƃ��Ƃ����ʒu��null��
        mapStatus.UpdateMapData(playerPosX, playerPosY, null);

        //�ꎞ�I�ȕϐ��錾
        float timer = 0;
        Vector3 moveDir = Vector3.zero;

        //���͕����ɉ�����playerPos�̍X�V�ƈʒu�̈ړ�
        switch (dir)
        {
            case MapStatus.Direction.Left:
                //playerPos�̍X�V
                playerPosX -= 1;
                //�ʒu�̈ړ�
                moveDir = Vector3.left;
                break;

            case MapStatus.Direction.Up:
                playerPosY += 1;
                moveDir = Vector3.up;
                break;

            case MapStatus.Direction.Right:
                playerPosX += 1;
                moveDir = Vector3.right;
                break;

            case MapStatus.Direction.Down:
                playerPosY -= 1;
                moveDir = Vector3.down;
                break;
        }

        while (timer <= MOVE_TIME)
        {
            transform.position += moveDir / (MOVE_TIME / Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        //�V�����ړ������ʒu�����̃I�u�W�F�N�g��
        mapStatus.UpdateMapData(playerPosX, playerPosY, transform.gameObject);

        //�S�[���`�F�b�N�ƃX�C�b�`�`�F�b�N
        mapStatus.CheckGoal();
        mapStatus.CheckSwitch(playerPosX, playerPosY);

        //�ړ��I��
        isMove = false;
    }

}
