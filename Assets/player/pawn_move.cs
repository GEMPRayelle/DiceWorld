using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class pawn_move : MonoBehaviour
{
    public int player_ID; // �÷��̾� �ĺ���
    public Vector3 targetPosition; // �̵��� ��ǥ ��ġ
    public int move_point; // �̵��� ����Ʈ
    public int current_index; // ���� Ÿ�� �ε���
    public int current_row; // ���� ��
    public GameObject gameBoard; // ���� ���� ������Ʈ
    public GameObject gameManager; // ���� �Ŵ��� ������Ʈ
    public float movementSpeed = 2.0f; // �̵� �ӵ�
    public int maxTile_num; // �ִ� Ÿ�� ����
    bool isMoved = false; // �̵� ���� �÷���
    public float next_turn_count = 0;
    bool is_not_start = false;
    void Start()
    {
        gameManager = GameObject.Find("game_manager");
        gameBoard = GameObject.Find("boardCenter");
    }

    void Update()
    {
        maxTile_num = gameManager.GetComponent<tile_manager>().max_tile_num;
        current_row = current_index / 8;

        if (current_index >= 0)
        {
            current_index = current_index % (maxTile_num + 1);
        }
        else
        {
            current_index = (maxTile_num + 1) + current_index;
        }

        // ���� Ÿ���� Transform�� ã�� ��ǥ ��ġ ����
        Transform now_tile_trans = gameBoard.transform.Find("tile_" + current_index);
        targetPosition = new Vector3(now_tile_trans.position.x, transform.position.y, now_tile_trans.position.z);

        // ���� ��ġ���� ��ǥ ��ġ�� �����Ͽ� �̵�
        transform.position = Vector3.Lerp(transform.position, targetPosition, movementSpeed);
        if (Vector3.Distance(transform.position, targetPosition) < 0.0001f)
        {
            transform.position = targetPosition;
        }

        // ��߽���+�̵� ����Ʈ�� ���� �ְ� ��ǥ ��ġ�� �������� ��
        if (move_point > 0 && transform.position == targetPosition)
        {

            isMoved = true;
            current_index += 1;
            move_point -= 1;
            string event_tag = now_tile_trans.gameObject.GetComponent<tile_event_manager>().event_tag;
            if (event_tag != "nothing" && is_not_start == true)
            {
                now_tile_trans.gameObject.GetComponent<tile_event_manager>().passing_event();
            }
            else
            {
                ;
            }
            is_not_start = true;
        }

        // �̵��� �Ϸ�Ǿ��� �̵� ����Ʈ�� ���� ��
        if (isMoved && move_point <= 0 && (Vector3.Distance(transform.position, targetPosition) <= 0.01f))
        {
            transform.position = targetPosition;
            // ���� �÷��̾��� ������ ���
            if (gameManager.GetComponent<game_manager>().currentPlayerIndex == player_ID)
            {
                is_not_start = false;
                next_turn_count += Time.deltaTime;
                if (next_turn_count >= 0.3f)
                {
                    next_turn_count = 0f;
                    isMoved = false;
                    string event_tag = now_tile_trans.gameObject.GetComponent<tile_event_manager>().event_tag;

                    // Ư�� ��ġ�� �ٸ� �÷��̾ �ִ��� Ȯ��
                    int otherPlayerID = GetOtherPlayerIDAtLocation(current_index);
                    if (otherPlayerID != -1)
                    {
                        Debug.Log(player_ID + "/" + otherPlayerID);
                        // �÷��̾� �� �ο�
                        now_tile_trans.gameObject.GetComponent<tile_event_manager>().Fight(player_ID, otherPlayerID);
                    }
                    else if (event_tag != "nothing")
                    {
                        now_tile_trans.gameObject.GetComponent<tile_event_manager>().active_event();
                    }
                    else
                    {
                        gameManager.GetComponent<game_manager>().nextTurn(); // ���� �� ����
                    }
                }
            }
        }
    }

    int GetOtherPlayerIDAtLocation(int currentLocation)
    {
        foreach (var player in gameManager.GetComponent<game_manager>().players)
        {
            if(player != null){
                pawn_move playerPawn = player.GetComponent<pawn_move>();
                if (playerPawn.current_index == currentLocation && playerPawn.player_ID != player_ID && playerPawn != null)
                {
                    return playerPawn.player_ID;
                }
            }else{
                continue;
            }
        }
        return -1; // �ش� ��ġ�� �ٸ� �÷��̾ ����
    }

    // �ֻ��� ����� ���� ���� �̵���Ű�� �޼���
    public void move_pawn(int diceResult)
    {
        move_point += diceResult;
    }
}

